using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Trips.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Trips
{
	public class HistoryList
	{
		public class Query : IRequest<Result<List<TripDto>>>
		{
			public int UserId { get; init; }
			public int Page { get; init; }
			public int Limit { get; init; }
		}

		public class Handler : IRequestHandler<Query, Result<List<TripDto>>>
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

			public async Task<Result<List<TripDto>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					if (request.Page <= 0)
					{
						_logger.LogInformation("Page must be larger than 0");
						return Result<List<TripDto>>.Failure("Page must be larger than 0.");
					}

					if (request.Limit <= 0)
					{
						_logger.LogInformation("Limit must be larger than 0");
						return Result<List<TripDto>>.Failure("Limit must be larger than 0.");
					}

					User user = await _context.User.FindAsync(new object[] { request.UserId }, cancellationToken);

					if (user == null)
					{
						_logger.LogInformation("User with UserId {request.UserId} doesn't exist", request.UserId);
						return Result<List<TripDto>>.NotFound($"User with UserId {request.UserId} doesn't exist.");
					}

					bool isKeer = user.Role == (int) RoleStatus.Keer;

					int totalRecord = await _context.Trip
						.Where(t =>
							isKeer ? t.KeerId == request.UserId : t.BikerId == request.UserId)
						.Where(t =>
							t.Status == (int) TripStatus.Finished ||
							t.Status == (int) TripStatus.Cancelled)
						.CountAsync(cancellationToken);

					int lastPage = Utils.CalculateLastPage(totalRecord, request.Limit);

					List<TripDto> trips = new();

					if (request.Page <= lastPage)
						trips = await _context.Trip
							.Where(t =>
								isKeer ? t.KeerId == request.UserId : t.BikerId == request.UserId)
							.Where(t =>
								t.Status == (int) TripStatus.Finished ||
								t.Status == (int) TripStatus.Cancelled)
							.OrderByDescending(t => t.BookTime)
							.Skip((request.Page - 1) * request.Limit)
							.Take(request.Limit)
							.ProjectTo<TripDto>(_mapper.ConfigurationProvider,
								new { isKeer })
							.ToListAsync(cancellationToken);

					PaginationDto paginationDto = new(
						request.Page, request.Limit, trips.Count, lastPage, totalRecord
					);

					_logger.LogInformation("Successfully retrieved list of all history trips");
					return Result<List<TripDto>>.Success(
						trips, "Successfully retrieved list of all history trips.", paginationDto);
				}
				catch (Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<TripDto>>.Failure("Request was cancelled.");
				}
			}
		}
	}
}