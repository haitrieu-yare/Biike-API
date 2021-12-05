using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Users.DTOs;
using Domain;
using Domain.Enums;
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

            public UserLoginDto UserLoginDto { get; }
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

                    var user = await _context.User.FindAsync(new object[] {int.Parse(auth.User.LocalId)},
                        cancellationToken);

                    if (user == null || user.IsDeleted)
                    {
                        _logger.LogInformation("User with userId {UserId} doesn't exist", auth.User.LocalId);
                        return Result<UserLoginResponse>.Failure(
                            $"User with userId {auth.User.LocalId} doesn't exist.");
                    }

                    // TODO: Verify phone bằng cách lưu thông tin verified trên firebase
                    if (!user.IsEmailVerified)
                    {
                        if (auth.User.IsEmailVerified)
                        {
                            user.IsEmailVerified = true;
                            
                            var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                            if (!result)
                            {
                                _logger.LogInformation("Failed to update email verification");
                                return Result<UserLoginResponse>.Failure("Failed to update email verification.");
                            }
                        }
                        else
                        {
                            _logger.LogInformation("User with userId {UserId} hasn't verified email",
                                auth.User.LocalId);
                            return Result<UserLoginResponse>.Failure(
                                $"User with userId {auth.User.LocalId} hasn't verified email.");
                        }
                    }

                    if (request.UserLoginDto.IsAdmin!.Value && user.RoleId != (int) RoleStatus.Admin)
                    {
                        _logger.LogInformation("User with userId {UserId} is not an admin", auth.User.LocalId);
                        return Result<UserLoginResponse>.Failure(
                            $"User with userId {auth.User.LocalId} is not an admin.");
                    }

                    if (!request.UserLoginDto.IsAdmin!.Value && user.RoleId == (int) RoleStatus.Admin)
                    {
                        _logger.LogInformation(
                            "User with userId {UserId} is an admin but isAdmin in request body is set to false",
                            auth.User.LocalId);
                        return Result<UserLoginResponse>.Failure(
                            $"User with userId {auth.User.LocalId} is an admin but isAdmin in request body is set to false.");
                    }

                    user.LastTimeLogin = CurrentTime.GetCurrentTime();

                    var loginTimeResult = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!loginTimeResult)
                    {
                        _logger.LogInformation("Failed to save last login time");
                        return Result<UserLoginResponse>.Failure("Failed to save last login time.");
                    }

                    var response = new UserLoginResponse
                    {
                        UserId = auth.User.LocalId,
                        Email = auth.User.Email,
                        DisplayName = auth.User.DisplayName,
                        ProfilePicture = auth.User.PhotoUrl,
                        IdToken = auth.FirebaseToken,
                        RefreshToken = auth.RefreshToken,
                        Phone = auth.User.PhoneNumber,
                        ExpiresIn = auth.ExpiresIn.ToString(),
                        IsPhoneVerified = user.IsPhoneVerified
                    };
                    
                    authProvider.Dispose();

                    _logger.LogInformation("Successfully logged in");
                    return Result<UserLoginResponse>.Success(response, "Successfully logged in.");
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