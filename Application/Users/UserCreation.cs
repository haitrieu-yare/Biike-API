using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Users.DTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Firebase.Auth;
using FirebaseAdmin.Auth;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Persistence;
using FirebaseAuth = FirebaseAdmin.Auth.FirebaseAuth;
using FirebaseAuthException = FirebaseAdmin.Auth.FirebaseAuthException;
using User = Domain.Entities.User;

namespace Application.Users
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UserCreation
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Command(UserCreationDto userCreationDto)
            {
                UserCreationDto = userCreationDto;
            }

            public UserCreationDto UserCreationDto { get; }
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IConfiguration _config;
            private readonly ILogger<Handler> _logger;
            private readonly IMapper _mapper;
            private const string SavePoint = "beforeUserCreation";

            public Handler(DataContext context, IConfiguration config, IMapper mapper, ILogger<Handler> logger)
            {
                _context = context;
                _config = config;
                _mapper = mapper;
                _logger = logger;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                await using IDbContextTransaction transaction =
                    await _context.Database.BeginTransactionAsync(CancellationToken.None);
                await transaction.CreateSavepointAsync(SavePoint, CancellationToken.None);

                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    User userDb = await _context.User
                        .Where(u => u.Email == request.UserCreationDto.Email ||
                                    u.PhoneNumber == request.UserCreationDto.PhoneNumber)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (userDb != null)
                    {
                        _logger.LogInformation("User with the same email or phone number has already existed");
                        return Result<Unit>.Failure("User with the same email or phone number has already existed.");
                    }

                    #region Setup User Info

                    User newUser = new();

                    _mapper.Map(request.UserCreationDto, newUser);

                    // Setup email, avatar
                    newUser.Email = newUser.Email.ToLower();
                    string fullNameAbbreviation = ApplicationUtils.GetFullNameAbbreviation(newUser.FullName);
                    string backgroundColor = ApplicationUtils.GetRandomColor();
                    newUser.Avatar = $"https://ui-avatars.com/api/?name={fullNameAbbreviation}" +
                                     $"&background={backgroundColor}&color={Color.White}&rounded=true&size=128";

                    string password = newUser.PasswordHash;
                    // Hash password
                    newUser.PasswordHash = Hashing.HashPassword(newUser.PasswordHash);

                    #endregion

                    #region Setup Wallet Info

                    var currentTime = DateTime.UtcNow.AddHours(7);
                    var toDate = currentTime;

                    switch (currentTime.Month)
                    {
                        case >= 1 and <= 4:
                            toDate = DateTime.Parse($"{currentTime.Year}/04/30 23:59:59.9999999");
                            break;
                        case >= 5 and <= 8:
                            toDate = DateTime.Parse($"{currentTime.Year}/08/31 23:59:59.9999999");
                            break;
                        case >= 9 and <= 12:
                            toDate = DateTime.Parse($"{currentTime.Year}/12/31 23:59:59.9999999");
                            break;
                    }

                    Wallet newWallet = new() {User = newUser, ToDate = toDate};

                    #endregion

                    #region Insert User To Database
                    // These code will try to insert user to database but not yet committed.
                    // If resultUser or resultWallet is false,
                    // or an exception is thrown, transaction will be rollback.

                    await _context.User.AddAsync(newUser, cancellationToken);
                    var resultUser = await _context.SaveChangesAsync(cancellationToken) > 0;
                    await _context.Wallet.AddAsync(newWallet, cancellationToken);
                    var resultWallet = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!resultUser || !resultWallet)
                    {
                        await transaction.RollbackToSavepointAsync(SavePoint, CancellationToken.None);
                        
                        _logger.LogInformation("Failed to create new user");
                        return Result<Unit>.Failure("Failed to create new user.");
                    }

                    #endregion

                    try
                    {
                        #region Create user on Firebase

                        var userToCreate = new UserRecordArgs
                        {
                            Uid = newUser.UserId.ToString(),
                            Email = newUser.Email,
                            Password = request.UserCreationDto.Password,
                            PhoneNumber = newUser.PhoneNumber,
                            DisplayName = newUser.FullName,
                            PhotoUrl = newUser.Avatar
                        };

                        await FirebaseAuth.DefaultInstance.CreateUserAsync(userToCreate, cancellationToken);

                        #endregion

                        #region Import user's role to Firebase

                        var claims = new Dictionary<string, object> {{"role", (int) RoleStatus.Keer}};

                        await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(userToCreate.Uid, claims,
                            cancellationToken);

                        #endregion

                        #region Send Email Verification

                        var authProvider = new FirebaseAuthProvider(new FirebaseConfig(_config["Firebase:WebApiKey"]));
                        var auth = await authProvider.SignInWithEmailAndPasswordAsync(newUser.Email, password);
                        await authProvider.SendEmailVerificationAsync(auth);

                        #endregion
                    }
                    catch (FirebaseAuthException e)
                    {
                        await transaction.RollbackToSavepointAsync(SavePoint, CancellationToken.None);

                        _logger.LogError("Error create user on Firebase. {Error}",
                            e.InnerException?.Message ?? e.Message);
                        return Result<Unit>.Failure("Error create user on Firebase. " +
                                                    $"{e.InnerException?.Message ?? e.Message}");
                    }
                    
                    // Commit change to database, at this point, everything must be ok and safe to be committed.
                    await transaction.CommitAsync(CancellationToken.None);

                    _logger.LogInformation("Successfully created user");
                    return Result<Unit>.Success(Unit.Value, "Successfully created user.", newUser.UserId.ToString());
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    await transaction.RollbackToSavepointAsync(SavePoint, CancellationToken.None);

                    _logger.LogInformation("Request was cancelled");
                    return Result<Unit>.Failure("Request was cancelled.");
                }
                catch (Exception ex) when (ex is DbUpdateException)
                {
                    await transaction.RollbackToSavepointAsync(SavePoint, CancellationToken.None);

                    _logger.LogInformation("{Error}", ex.InnerException?.Message ?? ex.Message);
                    return Result<Unit>.Failure(ex.InnerException?.Message ?? ex.Message);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackToSavepointAsync(SavePoint, CancellationToken.None);

                    _logger.LogInformation("{Error}", ex.InnerException?.Message ?? ex.Message);
                    return Result<Unit>.Failure(ex.InnerException?.Message ?? ex.Message);
                }
            }
        }
    }
}