using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Users.DTOs;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Users
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UserProfileEdit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int UserId { get; init; }
            public UserProfileEditDto UserProfileEditDto { get; init; } = null!;
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

                    User user = await _context.User.FindAsync(new object[] {request.UserId}, cancellationToken);

                    if (user == null)
                    {
                        _logger.LogInformation("User not found");
                        return Result<Unit>.NotFound("User not found.");
                    }

                    if (user.IsDeleted)
                    {
                        _logger.LogInformation(
                            "User with UserId {request.UserId} has been deleted. " +
                            "Please reactivate it if you want to edit it", request.UserId);
                        return Result<Unit>.Failure($"User with UserId {request.UserId} has been deleted. " +
                                                    "Please reactivate it if you want to edit it.");
                    }

                    if (!string.IsNullOrEmpty(request.UserProfileEditDto.UserFullname))
                    {
                        string backgroundColor = Color.ColorList[new Random().Next(Color.ColorList.Count)];
                        user.Avatar = $"https://ui-avatars.com/api/?name={request.UserProfileEditDto.UserFullname}" +
                                      $"&background={backgroundColor}&color={Color.White}&rounded=true&size=128";
                    }

                    _mapper.Map(request.UserProfileEditDto, user);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to update user's profile by userId {request.UserId}",
                            request.UserId);
                        return Result<Unit>.Failure($"Failed to update user's profile by userId {request.UserId}.");
                    }

                    _logger.LogInformation("Successfully updated user's profile by userId {request.UserId}",
                        request.UserId);
                    return Result<Unit>.Success(Unit.Value,
                        $"Successfully updated user's profile by userId {request.UserId}.");
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<Unit>.Failure("Request was cancelled.");
                }
                catch (Exception ex) when (ex is DbUpdateException)
                {
                    _logger.LogInformation("{Error}", ex.InnerException?.Message ?? ex.Message);
                    return Result<Unit>.Failure(ex.InnerException?.Message ?? ex.Message);
                }
            }
        }
    }
}