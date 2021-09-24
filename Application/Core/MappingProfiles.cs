using Application.AppUsers.DTOs;
using Application.Bikes;
using Application.Feedbacks.DTOs;
using Application.Intimacies;
using Application.Routes;
using Application.Stations;
using Application.Trips.DTOs;
using Application.TripTransactions;
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
				.ForMember(u => u.UserStar, o => o.MapFrom(u => u.Star));

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

			#region history/upcoming trips
			string role = null;
			CreateMap<Trip, TripDTO>()
				.ForMember(t => t.TripId, o => o.MapFrom(t => t.Id))
				.ForMember(t => t.UserId, o =>
					o.MapFrom(t => (role.Equals("0")) ? t.BikerId : t.KeerId))
				.ForMember(t => t.Avatar, o =>
					o.MapFrom(t => (role.Equals("0")) ? t.Biker.Avatar : t.Keer.Avatar))
				.ForMember(t => t.UserFullname,
					o => o.MapFrom(t => (role.Equals("0")) ? t.Biker.FullName : t.Keer.FullName))
				.ForMember(t => t.TimeBook, o => o.MapFrom(t => t.BookTime))
				.ForMember(t => t.TripStatus, o => o.MapFrom(t => t.Status))
				.ForMember(t => t.StartingPointName, o => o.MapFrom(t => t.Route.Departure.Name))
				.ForMember(t => t.DestinationName, o => o.MapFrom(t => t.Route.Destination.Name));
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

			CreateMap<Bike, BikeDTO>()
				.ForMember(b => b.BikeId, o => o.MapFrom(b => b.Id))
				.ForMember(b => b.UserId, o => o.MapFrom(b => b.AppUserId))
				.ForMember(b => b.NumberPlate, o => o.MapFrom(b => b.PlateNumber));
			CreateMap<BikeDTO, Bike>()
				.ForMember(b => b.Id, o => o.Ignore())
				.ForMember(b => b.AppUserId, o => o.MapFrom(b => b.UserId))
				.ForMember(b => b.PlateNumber, o => o.MapFrom(b => b.NumberPlate));

			CreateMap<TripTransaction, TripTransactionDTO>()
				.ForMember(t => t.TransactionId, o => o.MapFrom(t => t.Id))
				.ForMember(t => t.AmountPoint, o => o.MapFrom(t => t.AmountOfPoint));

			CreateMap<IntimacyDTO, Intimacy>();
			CreateMap<Intimacy, IntimacyDTO>();
		}
	}
}