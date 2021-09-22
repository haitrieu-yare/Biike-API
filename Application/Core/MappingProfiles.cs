using Application.AppUsers.DTOs;
using Application.Feedbacks.DTOs;
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

			#region user all info
			CreateMap<AppUser, AppUserInfoDTO>();
			#endregion

			#region user profile
			CreateMap<AppUser, AppUserSelfProfileDTO>()
				.ForMember(u => u.UserId, o => o.MapFrom(u => u.Id))
				.ForMember(u => u.UserPhoneNumber, o => o.MapFrom(u => u.PhoneNumber))
				.ForMember(u => u.UserEmail, o => o.MapFrom(u => u.Email))
				.ForMember(u => u.UserFullname, o => o.MapFrom(u => u.FullName))
				.ForMember(u => u.UserStar, o => o.MapFrom(u => u.Star))
				.ForMember(u => u.Point, o => o.MapFrom(u => u.Wallet.Point));

			CreateMap<AppUser, AppUserProfileDTO>()
				.ForMember(u => u.UserId, o => o.MapFrom(u => u.Id))
				.ForMember(u => u.UserPhoneNumber, o => o.MapFrom(u => u.PhoneNumber))
				.ForMember(u => u.UserFullname, o => o.MapFrom(u => u.FullName))
				.ForMember(u => u.UserStar, o => o.MapFrom(u => u.Star));
			#endregion

			#region user profile that editable
			CreateMap<AppUserProfileEditDTO, AppUser>()
				.ForMember(u => u.FullName, o => o.MapFrom(u => u.UserFullname));
			#endregion

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

			CreateMap<TripCreateDTO, Trip>();
			CreateMap<Trip, TripDetailDTO>();
			CreateMap<TripBikerInfoDTO, Trip>()
				.ForMember(t => t.PlateNumber, o => o.MapFrom(t => t.NumberPlate));
			CreateMap<TripCancellationDTO, Trip>()
				.ForMember(t => t.FinishedTime, o => o.MapFrom(t => t.TimeFinished));

			CreateMap<FeedbackDTO, Feedback>()
				.ForMember(f => f.AppUserId, o => o.MapFrom(f => f.UserId))
				.ForMember(f => f.Star, o => o.MapFrom(f => f.TripStar));
			CreateMap<Feedback, FeedbackDTO>()
				.ForMember(f => f.UserId, o => o.MapFrom(f => f.AppUserId))
				.ForMember(f => f.TripStar, o => o.MapFrom(f => f.Star));
		}
	}
}