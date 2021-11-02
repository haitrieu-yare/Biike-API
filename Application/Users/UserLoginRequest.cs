using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Users.DTOs;
using Firebase.Auth;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Users
{
    public class UserLoginRequest
    {
        // ReSharper disable once ClassNeverInstantiated.Global
        public class Command : IRequest<Result<UserLoginResponse>>
         {
             // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
             public UserLoginDto UserLoginDto { get; init; } = null!;
         }

         // ReSharper disable once UnusedType.Global
         public class Handler : IRequestHandler<Command, Result<UserLoginResponse>>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IConfiguration _config;

            public Handler(IConfiguration config, ILogger<Handler> logger)
            {
                _config = config;
                _logger = logger;
            }

            public async Task<Result<UserLoginResponse>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var authProvider = new FirebaseAuthProvider(new FirebaseConfig(_config["Firebase:WebApiKey"]));

                    var auth = await authProvider.SignInWithEmailAndPasswordAsync(request.UserLoginDto.Email, request.UserLoginDto.Password);

                    var response = new UserLoginResponse
                    {
                        UserId = auth.User.LocalId,
                        Token = auth.FirebaseToken
                    };

                    _logger.LogInformation("Successfully retrieved list of all users");
                    return Result<UserLoginResponse>.Success(
                        response, "Successfully retrieved list of all users.");
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