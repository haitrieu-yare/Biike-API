using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Configurations
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ConfigurationDetails
    {
        public class Query : IRequest<Result<string>>
        {
            public Query(string configurationName)
            {
                ConfigurationName = configurationName;
            }

            public string ConfigurationName { get; }
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Query, Result<string>>
        {
            private readonly DataContext _context;
            private readonly ILogger<Handler> _logger;

            public Handler(DataContext context, ILogger<Handler> logger)
            {
                _context = context;
                _logger = logger;
            }

            public async Task<Result<string>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var configuration = await _context.Configuration
                        .Where(c => c.ConfigurationName.Equals(request.ConfigurationName))
                        .SingleOrDefaultAsync(cancellationToken);
                    
                    if (configuration == null)
                    {
                        _logger.LogInformation("Configuration doesn't exist");
                        return Result<string>.NotFound("Configuration doesn't exist.");
                    }

                    _logger.LogInformation("Successfully retrieved configuration value");
                    return Result<string>.Success(configuration.ConfigurationValue,
                        "Successfully retrieved configuration value.");
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<string>.Failure("Request was cancelled.");
                }
            }
        }
    }
}

