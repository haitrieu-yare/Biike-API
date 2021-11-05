using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Users.DTOs;
using Firebase.Auth;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Users
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UserLoginRequest
    {
        // ReSharper disable once ClassNeverInstantiated.Global
        public class Command : IRequest<Result<UserLoginResponse>>
        {
            public Command(UserLoginDto userLoginDto)
            {
                UserLoginDto = userLoginDto;
            }
            public UserLoginDto UserLoginDto { get; init; }
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Command, Result<UserLoginResponse>>
        {
            private readonly ILogger<Handler> _logger;
            private readonly DataContext _context;
            private readonly IConfiguration _config;

            public Handler(DataContext context, IConfiguration config, ILogger<Handler> logger)
            {
                _context = context;
                _config = config;
                _logger = logger;
            }

            public async Task<Result<UserLoginResponse>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var authProvider = new FirebaseAuthProvider(new FirebaseConfig(_config["Firebase:WebApiKey"]));

                    var auth = await authProvider.SignInWithEmailAndPasswordAsync(request.UserLoginDto.Email,
                        request.UserLoginDto.Password);

                    var user = await _context.User.FindAsync(new object[] {int.Parse(auth.User.LocalId)}, cancellationToken);
                    
                    if (user == null)
                    {
                        _logger.LogInformation("User with userId {UserId} doesn't exist in database",
                            auth.User.LocalId);
                        return Result<UserLoginResponse>.Failure(
                            $"User with userId {auth.User.LocalId} doesn't exist in database.");
                    }

                    var response = new UserLoginResponse
                    {
                        UserId = auth.User.LocalId,
                        Email = auth.User.Email,
                        DisplayName = auth.User.DisplayName,
                        ProfilePicture = auth.User.PhotoUrl,
                        IdToken = auth.FirebaseToken,
                        RefreshToken = auth.RefreshToken,
                        ExpiresIn = auth.ExpiresIn.ToString()
                    };

                    _logger.LogInformation("Successfully retrieved list of all users");
                    return Result<UserLoginResponse>.Success(response, "Successfully retrieved list of all users.");
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<UserLoginResponse>.Failure("Request was cancelled.");
                }
                catch (Exception ex) when (ex is FirebaseAuthException)
                {
                    _logger.LogInformation("{Error}", ex.Message);
                    return Result<UserLoginResponse>.Failure($"{ex.Message}");
                }
                catch (Exception ex)
                {
                    _logger.LogInformation("{Error}", ex.InnerException?.Message ?? ex.Message);
                    return Result<UserLoginResponse>.Failure($"{ex.InnerException?.Message ?? ex.Message}");
                }
            }
        }
    }
}