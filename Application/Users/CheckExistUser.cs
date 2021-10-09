using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Users.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Users
{
    public class CheckExistUser
    {
        public class Command : IRequest<Result<Unit>>
        {
            public UserExistDto UserExistDto { get; set; } = null!;
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly ILogger<CheckExistUser> _logger;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper, ILogger<CheckExistUser> logger)
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

                    var user = await _context.User
                        .Where(u => u.Email == request.UserExistDto.Email ||
                                    u.PhoneNumber == request.UserExistDto.PhoneNumber)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (user != null)
                    {
                        _logger.LogInformation("User with the same email or phone number has already existed.");
                        return Result<Unit>.Failure("User with the same email or phone number has already existed.");
                    }

                    _logger.LogInformation("User doesn't exist.");
                    return Result<Unit>.Success(Unit.Value, "User doesn't exist.");
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled.");
                    return Result<Unit>.Failure("Request was cancelled.");
                }
            }
        }
    }
}