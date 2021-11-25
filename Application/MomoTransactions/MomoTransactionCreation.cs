using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.MomoTransactions.DTOs;
using Application.PointHistory;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.MomoTransactions
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MomoTransactionCreation
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Command(MomoTransactionCreationDto momoTransactionCreationDto, int userId)
            {
                MomoTransactionCreationDto = momoTransactionCreationDto;
                UserId = userId;
            }

            public MomoTransactionCreationDto MomoTransactionCreationDto { get; }
            public int UserId { get; }
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly ILogger<Handler> _logger;
            private readonly IMapper _mapper;
            private readonly IConfiguration _configuration;
            private readonly AutoPointHistoryCreation _historyCreation;

            public Handler(DataContext context, IMapper mapper, IConfiguration configuration,
                AutoPointHistoryCreation historyCreation, ILogger<Handler> logger)
            {
                _context = context;
                _mapper = mapper;
                _configuration = configuration;
                _historyCreation = historyCreation;
                _logger = logger;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    const string requestUrl = "https://test-payment.momo.vn/gw_payment/transactionProcessor";

                    var requestId = ApplicationUtils.RandomString(10);

                    string stringToBeHashed = $"partnerCode={_configuration["Momo:PartnerCode"]}&" +
                                              $"accessKey={_configuration["Momo:AccessKey"]}&" +
                                              $"requestId={requestId}&" +
                                              $"orderId={request.MomoTransactionCreationDto.OrderId!}&" +
                                              $"requestType={Constant.TransactionStatus}";

                    string signature =
                        ApplicationUtils.HmacSha256Digest(stringToBeHashed, _configuration["Momo:PrivateKey"]);

                    var momoTransactionCheckDto = new MomoTransactionCheckDto
                    {
                        PartnerCode = _configuration["Momo:PartnerCode"],
                        AccessKey = _configuration["Momo:AccessKey"],
                        RequestType = Constant.TransactionStatus,
                        OrderId = request.MomoTransactionCreationDto.OrderId!,
                        Signature = signature,
                        RequestId = requestId
                    };

                    var options = new JsonSerializerOptions {WriteIndented = true};
                    string requestBody = JsonSerializer.Serialize(momoTransactionCheckDto, options);
                    HttpContent stringContent = new StringContent(requestBody, Encoding.UTF8, "application/json");

                    HttpClient client = new();
                    HttpResponseMessage task =
                        await client.PostAsync(requestUrl, stringContent, CancellationToken.None);

                    if (!task.IsSuccessStatusCode)
                    {
                        _logger.LogInformation("Failed to check momo transaction on Momo server");
                        return Result<Unit>.Failure("Failed to check momo transaction on Momo server.");
                    }

                    Stream jsonStream = await task.Content.ReadAsStreamAsync(cancellationToken);
                    var momoTransactionCheckResultDto =
                        await JsonSerializer.DeserializeAsync<MomoTransactionCheckResultDto>(jsonStream,
                            cancellationToken: cancellationToken);

                    if (momoTransactionCheckResultDto == null || momoTransactionCheckResultDto.ErrorCode != 0)
                    {
                        _logger.LogInformation("Failed to check momo transaction on Momo server");
                        return Result<Unit>.Failure("Failed to check momo transaction on Momo server.");
                    }

                    var isAmountValid = int.TryParse(momoTransactionCheckResultDto.Amount, out var amount);
                    if (isAmountValid && amount != request.MomoTransactionCreationDto.Amount)
                    {
                        _logger.LogInformation("Amount value doesn't match with Momo server");
                        return Result<Unit>.Failure("Amount value doesn't match with Momo server.");
                    }

                    if (momoTransactionCheckResultDto.TransId != request.MomoTransactionCreationDto.TransactionId)
                    {
                        _logger.LogInformation("TransactionId value doesn't match with Momo server");
                        return Result<Unit>.Failure("TransactionId value doesn't match with Momo server.");
                    }

                    MomoTransaction momoTransaction = await _context.MomoTransaction
                        .Where(m => m.TransactionId == momoTransactionCheckResultDto.TransId ||
                                    m.OrderId == momoTransactionCheckResultDto.OrderId)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (momoTransaction != null)
                    {
                        _logger.LogInformation("Momo transaction with this TransactionId or OrderId is already exist");
                        return Result<Unit>.Failure(
                            "Momo transaction with this TransactionId or OrderId is already exist.");
                    }

                    MomoTransaction newMomoTransaction = new();

                    _mapper.Map(request.MomoTransactionCreationDto, newMomoTransaction);

                    newMomoTransaction.UserId = request.UserId;

                    var conversionRate = await _context.Configuration
                        .Where(c => c.ConfigurationName.Equals("ConversionRate"))
                        .SingleOrDefaultAsync(cancellationToken);

                    if (conversionRate == null)
                    {
                        _logger.LogInformation("Failed to find conversion rate");
                        return Result<Unit>.Failure("Failed to find conversion rate.");
                    }

                    var isValidValue = double.TryParse(conversionRate.ConfigurationValue, out var conversionRateValue);

                    if (!isValidValue)
                    {
                        _logger.LogInformation("Failed to convert money to point");
                        return Result<Unit>.Failure("Failed to convert money to point.");
                    }

                    newMomoTransaction.ConversionRate = conversionRateValue;
                    newMomoTransaction.Point = (int) (amount * newMomoTransaction.ConversionRate);

                    await _context.MomoTransaction.AddAsync(newMomoTransaction, cancellationToken);

                    var user = await _context.User.FindAsync(new object[] {request.UserId}, cancellationToken);

                    if (user == null)
                    {
                        _logger.LogInformation("User with UserId {UserId} doesn't exist", request.UserId);
                        return Result<Unit>.NotFound($"User with UserId {request.UserId} doesn't exist.");
                    }

                    user.TotalPoint += newMomoTransaction.Point;

                    var currentWallet = await _context.Wallet.Where(w => w.UserId == user.UserId)
                        .Where(w => w.Status == (int) WalletStatus.Current)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (currentWallet == null)
                    {
                        _logger.LogInformation("User with UserId {UserId} doesn't have wallet", request.UserId);
                        return Result<Unit>.NotFound($"User with UserId {request.UserId} doesn't have wallet.");
                    }

                    currentWallet.Point += newMomoTransaction.Point;

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to create new momo transaction");
                        return Result<Unit>.Failure("Failed to create new momo transaction.");
                    }

                    await _historyCreation.Run(request.UserId, (int) HistoryType.Momo,
                        newMomoTransaction.MomoTransactionId, newMomoTransaction.Point, user.TotalPoint,
                        Constant.MomoPoint, newMomoTransaction.CreatedDate);

                    _logger.LogInformation("Successfully created momo transaction");
                    return Result<Unit>.Success(Unit.Value, "Successfully created momo transaction.",
                        newMomoTransaction.MomoTransactionId.ToString());
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<Unit>.Failure("Request was cancelled.");
                }
                catch (Exception ex)
                {
                    _logger.LogInformation("{Error}", ex.InnerException?.Message ?? ex.Message);
                    return Result<Unit>.Failure(ex.InnerException?.Message ?? ex.Message);
                }
            }
        }
    }
}