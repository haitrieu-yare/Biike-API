using Application.Routes;
using Application.Stations;
using Application.Trips.DTOs;
using AutoMapper;
using Domain;

namespace Application.Core
{
	public class MappingProfiles : Profile
	{
		public MappingProfiles()
		{
			CreateMap<Station, StationDTO>();
			CreateMap<StationDTO, Station>()
				.ForMember(s => s.Id, opt => opt.Ignore());

			CreateMap<Route, RouteDTO>();
			CreateMap<RouteDTO, Route>()
				.ForMember(r => r.Id, opt => opt.Ignore());

			#region history trips
			CreateMap<Trip, KeerHistoryTripDTO>()
				.ForMember(t => t.Avatar, o => o.MapFrom(t => t.Biker.Avatar))
				.ForMember(t => t.UserFullname, o => o.MapFrom(t => t.Biker.FullName))
				.ForMember(t => t.DepartureName, o => o.MapFrom(t => t.Route.Departure.Name))
				.ForMember(t => t.DestinationName, o => o.MapFrom(t => t.Route.Destination.Name));
			CreateMap<Trip, BikerHistoryTripDTO>()
				.ForMember(t => t.Avatar, o => o.MapFrom(t => t.Keer.Avatar))
				.ForMember(t => t.UserFullname, o => o.MapFrom(t => t.Keer.FullName))
				.ForMember(t => t.DepartureName, o => o.MapFrom(t => t.Route.Departure.Name))
				.ForMember(t => t.DestinationName, o => o.MapFrom(t => t.Route.Destination.Name));
			CreateMap<KeerHistoryTripDTO, TripHistoryDTO>()
				.ForMember(t => t.TripId, o => o.MapFrom(t => t.Id))
				.ForMember(t => t.UserId, o => o.MapFrom(t => t.BikerId))
				.ForMember(t => t.TimeBook, o => o.MapFrom(t => t.BookTime))
				.ForMember(t => t.TripStatus, o => o.MapFrom(t => t.Status))
				.ForMember(t => t.StartingPointName, o => o.MapFrom(t => t.DepartureName));
			CreateMap<BikerHistoryTripDTO, TripHistoryDTO>()
				.ForMember(t => t.TripId, o => o.MapFrom(t => t.Id))
				.ForMember(t => t.UserId, o => o.MapFrom(t => t.KeerId))
				.ForMember(t => t.TimeBook, o => o.MapFrom(t => t.BookTime))
				.ForMember(t => t.TripStatus, o => o.MapFrom(t => t.Status))
				.ForMember(t => t.StartingPointName, o => o.MapFrom(t => t.DepartureName));
			#endregion

			#region upcoming trips
			CreateMap<Trip, KeerUpcomingTripDTO>()
				.ForMember(t => t.PhoneNumber, o => o.MapFrom(t => t.Biker.PhoneNumber))
				.ForMember(t => t.Avatar, o => o.MapFrom(t => t.Biker.Avatar))
				.ForMember(t => t.UserFullname, o => o.MapFrom(t => t.Biker.FullName))
				.ForMember(t => t.DepartureName, o => o.MapFrom(t => t.Route.Departure.Name))
				.ForMember(t => t.DestinationName, o => o.MapFrom(t => t.Route.Destination.Name));
			CreateMap<Trip, BikerUpcomingTripDTO>()
				.ForMember(t => t.PhoneNumber, o => o.MapFrom(t => t.Keer.PhoneNumber))
				.ForMember(t => t.Avatar, o => o.MapFrom(t => t.Keer.Avatar))
				.ForMember(t => t.UserFullname, o => o.MapFrom(t => t.Keer.FullName))
				.ForMember(t => t.DepartureName, o => o.MapFrom(t => t.Route.Departure.Name))
				.ForMember(t => t.DestinationName, o => o.MapFrom(t => t.Route.Destination.Name));
			CreateMap<KeerUpcomingTripDTO, TripUpcomingDTO>()
				.ForMember(t => t.TripId, o => o.MapFrom(t => t.Id))
				.ForMember(t => t.UserId, o => o.MapFrom(t => t.BikerId))
				.ForMember(t => t.UserPhoneNumber, o => o.MapFrom(t => t.PhoneNumber))
				.ForMember(t => t.TimeBook, o => o.MapFrom(t => t.BookTime))
				.ForMember(t => t.TripStatus, o => o.MapFrom(t => t.Status))
				.ForMember(t => t.StartingPointName, o => o.MapFrom(t => t.DepartureName));
			CreateMap<BikerUpcomingTripDTO, TripUpcomingDTO>()
				.ForMember(t => t.TripId, o => o.MapFrom(t => t.Id))
				.ForMember(t => t.UserId, o => o.MapFrom(t => t.KeerId))
				.ForMember(t => t.UserPhoneNumber, o => o.MapFrom(t => t.PhoneNumber))
				.ForMember(t => t.TimeBook, o => o.MapFrom(t => t.BookTime))
				.ForMember(t => t.TripStatus, o => o.MapFrom(t => t.Status))
				.ForMember(t => t.StartingPointName, o => o.MapFrom(t => t.DepartureName));
			#endregion

			CreateMap<Trip, TripHistoryDTO>();
			CreateMap<TripHistoryDTO, Trip>()
				.ForMember(t => t.Id, opt => opt.Ignore());
		}
	}
}