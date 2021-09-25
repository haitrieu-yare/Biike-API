using System;
using Application.AppUsers.DTOs;
using Application.Bikes;
using Application.Feedbacks.DTOs;
using Application.Intimacies;
using Application.Redemptions.DTOs;
using Application.Routes.DTOs;
using Application.Stations;
using Application.Trips.DTOs;
using Application.TripTransactions;
using Application.VoucherCategories;
using Application.Vouchers.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Core
{
	public class MappingProfiles : Profile
	{
		public MappingProfiles()
		{
			// Mặc định khi kiểu int? có giá trị null thì 
			// map qua kiểu int sẽ bị chuyển thành 0.
			// Nhưng chúng ta thường sẽ muốn ignore biến int? nếu
			// biến int? có giá trị null, và để nguyên giá trị gốc của int.
			// Câu map ở dưới đây sinh ra nhằm tránh việc chuyển giá trị
			// int gốc thành 0 khi truyền vào biến int? với giá trị null.
			// Việc truyền biến int? với giá trị null thường xảy ra 
			// khi người dùng không truyền các optional field 
			// ở trong body request của EditDTO.
			CreateMap<int?, int>().ConvertUsing(
				(src, dest) =>
				{
					if (src.HasValue) return src.Value;
					return dest;
				}
			);

			// Tương tự int?, chúng ta tạo map cho DateTime?
			CreateMap<DateTime?, DateTime>().ConvertUsing(
				(src, dest) =>
				{
					if (src.HasValue) return src.Value;
					return dest;
				}
			);

			// Tương tự int?, chúng ta tạo map cho bool?
			CreateMap<bool?, bool>().ConvertUsing(
				(src, dest) =>
				{
					if (src.HasValue) return src.Value;
					return dest;
				}
			);

			CreateMap<Station, StationDTO>();
			CreateMap<StationDTO, Station>()
				.ForMember(s => s.Id, opt => opt.Ignore());

			// List, Detail Route
			CreateMap<Route, RouteDTO>();
			// Edit Route
			CreateMap<RouteDTO, Route>()
				.ForMember(r => r.Id, opt => opt.Ignore())
				.ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
			// Create Route
			CreateMap<RouteCreateDTO, Route>();

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
				.ForMember(u => u.Id, o => o.Ignore())
				.ForMember(u => u.FullName, o => o.MapFrom(u => u.UserFullname))
				.ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
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

			#region trip history pair
			int userTwoId = 0;
			CreateMap<Trip, TripPairDTO>()
				.ForMember(t => t.TripId, o => o.MapFrom(t => t.Id))
				.ForMember(t => t.UserId, o =>
					o.MapFrom(t => userTwoId == t.BikerId ? t.BikerId : t.KeerId))
				.ForMember(t => t.Avatar, o =>
					o.MapFrom(t => userTwoId == t.BikerId ? t.Biker.Avatar : t.Keer.Avatar))
				.ForMember(t => t.UserFullname, o =>
					o.MapFrom(t => userTwoId == t.BikerId ? t.Biker.FullName : t.Keer.FullName))
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

			CreateMap<VoucherCategory, VoucherCategoryDTO>();
			CreateMap<VoucherCategoryDTO, VoucherCategory>()
				.ForMember(v => v.VoucherCategoryId, o => o.Ignore());

			CreateMap<Voucher, VoucherDTO>();
			CreateMap<VoucherCreateDTO, Voucher>();
			CreateMap<VoucherEditDTO, Voucher>()
				.ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));

			CreateMap<Redemption, RedemptionDTO>();
			CreateMap<RedemptionCreateDTO, Redemption>();
		}
	}
}