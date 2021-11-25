using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Configurations.DTOs;
using Application.Core;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Configurations
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ConfigurationEdit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Command(ConfigurationDto configurationCreationDto, int userId)
            {
                ConfigurationDto = configurationCreationDto;
                UserId = userId;
            }

            public ConfigurationDto ConfigurationDto { get; }
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

                    if (request.ConfigurationDto.ConfigurationName == null)
                    {
                        _logger.LogInformation("ConfigurationName field is required");
                        return Result<Unit>.Failure("ConfigurationName field is required.");
                    }
                    
                    if (request.ConfigurationDto.ConfigurationValue == null)
                    {
                        _logger.LogInformation("ConfigurationValue field is required");
                        return Result<Unit>.Failure("ConfigurationValue field is required.");
                    }

                    var configuration = await _context.Configuration.Where(c =>
                            c.ConfigurationName.Equals(request.ConfigurationDto.ConfigurationName))
                        .SingleOrDefaultAsync(cancellationToken);

                    _mapper.Map(request.ConfigurationDto, configuration);

                    configuration.UserId = request.UserId;
                    configuration.LastUpdatedDate = CurrentTime.GetCurrentTime();

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to update configuration");
                        return Result<Unit>.Failure("Failed to update configuration.");
                    }

                    _logger.LogInformation("Successfully updated configuration");
                    return Result<Unit>.Success(Unit.Value, "Successfully updated configuration.");
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