using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Configurations.DTOs;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Configurations
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ConfigurationCreation
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Command(ConfigurationCreationDto configurationCreationDto, int userId)
            {
                ConfigurationCreationDto = configurationCreationDto;
                UserId = userId;
            }

            public ConfigurationCreationDto ConfigurationCreationDto { get; }
            public int UserId { get; }
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

                    Configuration newConfiguration = new();

                    _mapper.Map(request.ConfigurationCreationDto, newConfiguration);

                    newConfiguration.UserId = request.UserId;

                    await _context.Configuration.AddAsync(newConfiguration, cancellationToken);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to create new configuration");
                        return Result<Unit>.Failure("Failed to create new configuration.");
                    }

                    _logger.LogInformation("Successfully created configuration");
                    return Result<Unit>.Success(Unit.Value, "Successfully created configuration.",
                        newConfiguration.ConfigurationId.ToString());
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