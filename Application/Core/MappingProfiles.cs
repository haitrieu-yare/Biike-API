using System;
using Application.Bikes.DTOs;
using Application.Feedbacks.DTOs;
using Application.Intimacies.DTOs;
using Application.Redemptions.DTOs;
using Application.Routes.DTOs;
using Application.Stations.DTOs;
using Application.Trips.DTOs;
using Application.TripTransactions.DTOs;
using Application.Users.DTOs;
using Application.VoucherCategories.DTOs;
using Application.Vouchers.DTOs;
using Application.Wallets.DTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.Core
{
	public class MappingProfiles : Profile
	{
		public MappingProfiles()
		{
			#region Fix Type Conversion
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

			// Tương tự int?, chúng ta tạo map cho double?
			CreateMap<double?, double>().ConvertUsing(
				(src, dest) =>
				{
					if (src.HasValue) return src.Value;
					return dest;
				}
			);
			#endregion

			#region Station
			// List, Detail 
			CreateMap<Station, StationDto>();
			// Edit
			CreateMap<StationDto, Station>()
				.ForMember(s => s.StationId, opt => opt.Ignore())
				.ForMember(s => s.CreatedDate, opt => opt.Ignore())
				.ForMember(s => s.IsDeleted, opt => opt.Ignore())
				.ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
			// Create
			CreateMap<StationCreateDto, Station>();
			#endregion

			#region Route
			// List, Detail 
			CreateMap<Route, RouteDto>();
			// Edit 
			CreateMap<RouteDto, Route>()
				.ForMember(r => r.RouteId, opt => opt.Ignore())
				.ForMember(r => r.CreatedDate, opt => opt.Ignore())
				.ForMember(r => r.IsDeleted, opt => opt.Ignore())
				.ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
			// Create
			CreateMap<RouteCreateDto, Route>();
			#endregion

			#region User
			// List, Detail
			CreateMap<User, UserDto>()
				.ForMember(u => u.UserStar, o => o.MapFrom(u => u.Star));
			// Create
			CreateMap<UserCreateDto, User>()
				.ForMember(u => u.PasswordHash, o => o.MapFrom(u => u.Password));
			// Edit Profile
			CreateMap<UserProfileEditDto, User>()
				.ForMember(u => u.FullName, o => o.MapFrom(u => u.UserFullname))
				.ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
			// Edit LoginDevice
			CreateMap<UserLoginDeviceDto, User>()
				.ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
			#endregion

			#region Trip
			// History Trips & Upcoming Trips
			bool isKeer = true;
			CreateMap<Trip, TripDto>()
				.ForMember(t => t.UserId, o =>
					o.MapFrom(t => isKeer ? t.BikerId : t.KeerId))
				.ForMember(t => t.Avatar, o =>
					o.MapFrom(t =>
						(t.Biker == null && isKeer) ? null :
						(t.Biker == null && !isKeer) ? t.Keer.Avatar :
						(t.Biker != null && isKeer) ? t.Biker.Avatar : t.Keer.Avatar))
				.ForMember(t => t.UserFullname, o =>
					o.MapFrom(t =>
						(t.Biker == null && isKeer) ? null :
						(t.Biker == null && !isKeer) ? t.Keer.FullName :
						(t.Biker != null && isKeer) ? t.Biker.FullName : t.Keer.FullName))
				.ForMember(t => t.UserPhoneNumber, o =>
					o.MapFrom(t =>
						(t.Biker == null && isKeer) ? null :
						(t.Biker == null && !isKeer) ? t.Keer.PhoneNumber :
						(t.Biker != null && isKeer) ? t.Biker.PhoneNumber : t.Keer.PhoneNumber))
				.ForMember(t => t.TimeBook, o => o.MapFrom(t => t.BookTime))
				.ForMember(t => t.TripStatus, o => o.MapFrom(t => t.Status))
				.ForMember(t => t.StartingPointName, o => o.MapFrom(t => t.Route.Departure.Name))
				.ForMember(t => t.DestinationName, o => o.MapFrom(t => t.Route.Destination.Name));

			// TripHistoryPair
			int userTwoId = 0;
			CreateMap<Trip, TripPairDto>()
				.ForMember(t => t.UserId, o =>
					o.MapFrom(t => userTwoId == t.BikerId ? t.BikerId : t.KeerId))
				.ForMember(t => t.Avatar, o =>
					o.MapFrom(t => userTwoId == t.BikerId ? t.Biker!.Avatar : t.Keer.Avatar))
				.ForMember(t => t.UserFullname, o =>
					o.MapFrom(t => userTwoId == t.BikerId ? t.Biker!.FullName : t.Keer.FullName))
				.ForMember(t => t.TimeBook, o => o.MapFrom(t => t.BookTime))
				.ForMember(t => t.TripStatus, o => o.MapFrom(t => t.Status))
				.ForMember(t => t.StartingPointName, o => o.MapFrom(t => t.Route.Departure.Name))
				.ForMember(t => t.DestinationName, o => o.MapFrom(t => t.Route.Destination.Name));

			// Detail
			int role = 0;
			CreateMap<Trip, TripDetailInfoDto>()
				.ForMember(t => t.UserId, o =>
					o.MapFrom(t => (role == (int)RoleStatus.Keer) ? t.BikerId : t.KeerId))
				.ForMember(t => t.Avatar, o =>
					o.MapFrom(t => (t.Biker == null) ? null :
						(role == (int)RoleStatus.Keer) ? t.Biker.Avatar : t.Keer.Avatar))
				.ForMember(t => t.UserFullname, o =>
					o.MapFrom(t => (t.Biker == null) ? null :
						(role == (int)RoleStatus.Keer) ? t.Biker.FullName : t.Keer.FullName))
				.ForMember(t => t.UserPhoneNumber, o =>
					o.MapFrom(t => (t.Biker == null) ? null :
						(role == (int)RoleStatus.Keer) ? t.Biker.PhoneNumber : t.Keer.PhoneNumber))
				.ForMember(t => t.UserStar, o => o.MapFrom(t => (t.Biker == null) ? new Nullable<double>() :
					(role == (int)RoleStatus.Keer) ? t.Biker.Star : t.Keer.Star))
				.ForMember(t => t.TimeBook, o => o.MapFrom(t => t.BookTime))
				.ForMember(t => t.CreatedTime, o => o.MapFrom(t => t.CreatedDate))
				.ForMember(t => t.TimeFinished, o => o.MapFrom(t => t.FinishedTime))
				.ForMember(t => t.TripStatus, o => o.MapFrom(t => t.Status))
				.ForMember(t => t.StartingPointName, o => o.MapFrom(t => t.Route.Departure.Name))
				.ForMember(t => t.DestinationName, o => o.MapFrom(t => t.Route.Destination.Name))
				.ForMember(t => t.Feedbacks, o => o.MapFrom(t => t.FeedbackList));
			// Detail Info
			CreateMap<Trip, TripDetailDto>();
			// Create
			CreateMap<TripCreateDto, Trip>();
			// Edit BikerInfo
			CreateMap<TripBikerInfoDto, Trip>()
				.ForMember(t => t.PlateNumber, o => o.MapFrom(t => t.NumberPlate));
			// Cancel Trip
			CreateMap<TripCancellationDto, Trip>()
				.ForMember(t => t.FinishedTime, o => o.MapFrom(t => t.TimeFinished));
			#endregion

			#region Feedback
			// ListAll, List
			CreateMap<Feedback, FeedbackDto>()
				.ForMember(f => f.TripStar, o => o.MapFrom(f => f.Star));
			// Create
			CreateMap<FeedbackCreateDto, Feedback>()
				.ForMember(f => f.Star, o => o.MapFrom(f => f.TripStar));
			#endregion

			#region Bike
			// List, Detail
			CreateMap<Bike, BikeDto>()
				.ForMember(b => b.NumberPlate, o => o.MapFrom(b => b.PlateNumber));
			// Create
			CreateMap<BikeCreateDto, Bike>()
				.ForMember(b => b.PlateNumber, o => o.MapFrom(b => b.NumberPlate));
			#endregion

			#region Trip Transaction
			// List, Detail, DetailTrip
			CreateMap<TripTransaction, TripTransactionDto>()
				.ForMember(t => t.AmountPoint, o => o.MapFrom(t => t.AmountOfPoint));
			#endregion

			#region Intimacy
			// List, Detail
			CreateMap<Intimacy, IntimacyDto>();
			// Edit, Create
			CreateMap<IntimacyCreateEditDto, Intimacy>();
			#endregion

			#region Voucher's Category
			// List, Detail
			CreateMap<VoucherCategory, VoucherCategoryDto>();
			// Edit
			CreateMap<VoucherCategoryDto, VoucherCategory>()
				.ForMember(v => v.VoucherCategoryId, o => o.Ignore())
				.ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
			// Create
			CreateMap<VoucherCategoryCreateDto, VoucherCategory>();
			#endregion

			#region Voucher
			// List, Detail
			CreateMap<Voucher, VoucherDto>();
			// Edit
			CreateMap<VoucherEditDto, Voucher>()
				.ForMember(v => v.VoucherId, o => o.Ignore())
				.ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
			// Create
			CreateMap<VoucherCreateDto, Voucher>();
			#endregion

			#region Redemption
			// ListUserRedemption, ListRedemption
			CreateMap<Redemption, RedemptionDto>();
			// ListUserRedemptionAndVoucher
			CreateMap<Redemption, RedemptionAndVoucherDto>()
				.ForMember(r => r.VoucherCategoryId, o => o.MapFrom(u => u.Voucher.VoucherCategoryId))
				.ForMember(r => r.VoucherName, o => o.MapFrom(u => u.Voucher.VoucherName))
				.ForMember(r => r.Brand, o => o.MapFrom(u => u.Voucher.Brand))
				.ForMember(r => r.StartDate, o => o.MapFrom(u => u.Voucher.StartDate))
				.ForMember(r => r.EndDate, o => o.MapFrom(u => u.Voucher.EndDate))
				.ForMember(r => r.Description, o => o.MapFrom(u => u.Voucher.Description))
				.ForMember(r => r.TermsAndConditions, o => o.MapFrom(u => u.Voucher.TermsAndConditions));
			// Create
			CreateMap<RedemptionCreateDto, Redemption>();
			#endregion

			#region Wallet
			// List, Detail
			CreateMap<Wallet, WalletDto>();
			// Edit
			CreateMap<WalletDto, Wallet>()
				.ForMember(w => w.WalletId, o => o.Ignore())
				.ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
			// Create
			CreateMap<WalletCreateDto, Wallet>();
			#endregion
		}
	}
}