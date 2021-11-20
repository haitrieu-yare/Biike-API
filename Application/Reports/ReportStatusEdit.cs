using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Reports
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ReportStatusEdit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Command(int reportId, int newStatus)
            {
                ReportId = reportId;
                NewStatus = newStatus;
            }

            public int ReportId { get; }
            public int NewStatus { get; }
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly ILogger<Handler> _logger;

            public Handler(DataContext context, ILogger<Handler> logger)
            {
                _context = context;
                _logger = logger;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    Report oldReport = await _context.Report.FindAsync(
                        new object[] {request.ReportId}, cancellationToken);

                    if (oldReport == null)
                    {
                        _logger.LogInformation("Report doesn't exist");
                        return Result<Unit>.NotFound("Report doesn't exist.");
                    }

                    oldReport.Status = request.NewStatus;

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to update status of report with ReportId {ReportId}",
                            request.ReportId);
                        return Result<Unit>.Failure(
                            $"Failed to update status of report with ReportId {request.ReportId}.");
                    }

                    _logger.LogInformation("Successfully updated status of report with ReportId {ReportId}",
                        request.ReportId);
                    return Result<Unit>.Success(Unit.Value,
                        $"Successfully updated status of report with ReportId {request.ReportId}.");
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