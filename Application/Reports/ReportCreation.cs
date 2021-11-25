using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Reports.DTOs;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Reports
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ReportCreation
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Command(ReportCreationDto reportCreationDto)
            {
                ReportCreationDto = reportCreationDto;
            }

            public ReportCreationDto ReportCreationDto { get; }
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly ILogger<Handler> _logger;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper, ILogger<Handler> logger)
            {
                _context = context;
                _mapper = mapper;
                _logger = logger;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    User userOne = await _context.User.Where(u => u.UserId == request.ReportCreationDto.UserOneId)
                        .Where(u => u.IsDeleted != true)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (userOne == null)
                    {
                        _logger.LogInformation("User one with UserId {UserId} doesn't exist",
                            request.ReportCreationDto.UserTwoId);
                        return Result<Unit>.NotFound(
                            $"User one with UserId {request.ReportCreationDto.UserTwoId} doesn't exist.");
                    }

                    User userTwo = await _context.User.Where(u => u.UserId == request.ReportCreationDto.UserTwoId)
                        .Where(u => u.IsDeleted != true)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (userTwo == null)
                    {
                        _logger.LogInformation("User two with UserId {UserId} doesn't exist",
                            request.ReportCreationDto.UserTwoId);
                        return Result<Unit>.NotFound(
                            $"User two with UserId {request.ReportCreationDto.UserTwoId} doesn't exist.");
                    }

                    Report newReport = new();

                    _mapper.Map(request.ReportCreationDto, newReport);

                    newReport.UserOneName = userOne.FullName;
                    newReport.UserTwoName = userTwo.FullName;

                    await _context.Report.AddAsync(newReport, cancellationToken);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to create new report");
                        return Result<Unit>.Failure("Failed to create new report.");
                    }

                    _logger.LogInformation("Successfully created report");
                    return Result<Unit>.Success(Unit.Value, "Successfully created report.",
                        newReport.ReportId.ToString());
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