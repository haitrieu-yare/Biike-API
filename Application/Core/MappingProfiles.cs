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
			CreateMap<Station, StationDTO>();
			// Edit
			CreateMap<StationDTO, Station>()
				.ForMember(s => s.StationId, opt => opt.Ignore())
				.ForMember(s => s.CreatedDate, opt => opt.Ignore())
				.ForMember(s => s.IsDeleted, opt => opt.Ignore())
				.ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
			// Create
			CreateMap<StationCreateDTO, Station>();
			#endregion

			#region Route
			// List, Detail 
			CreateMap<Route, RouteDTO>();
			// Edit 
			CreateMap<RouteDTO, Route>()
				.ForMember(r => r.RouteId, opt => opt.Ignore())
				.ForMember(r => r.CreatedDate, opt => opt.Ignore())
				.ForMember(r => r.IsDeleted, opt => opt.Ignore())
				.ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
			// Create
			CreateMap<RouteCreateDTO, Route>();
			#endregion

			#region User
			// List, Detail
			CreateMap<User, UserDTO>()
				.ForMember(u => u.UserStar, o => o.MapFrom(u => u.Star));
			// Create
			CreateMap<UserCreateDTO, User>()
				.ForMember(u => u.PasswordHash, o => o.MapFrom(u => u.Password));
			// Edit Profile
			CreateMap<UserProfileEditDTO, User>()
				.ForMember(u => u.FullName, o => o.MapFrom(u => u.UserFullname))
				.ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
			// Edit LoginDevice
			CreateMap<UserLoginDeviceDTO, User>()
				.ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
			#endregion

			#region Trip
			// History Trips & Upcoming Trips
			bool isKeer = true;
			CreateMap<Trip, TripDTO>()
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
			CreateMap<Trip, TripPairDTO>()
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
			CreateMap<Trip, TripDetailInfoDTO>()
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
			CreateMap<Trip, TripDetailDTO>();
			// Create
			CreateMap<TripCreateDTO, Trip>();
			// Edit BikerInfo
			CreateMap<TripBikerInfoDTO, Trip>()
				.ForMember(t => t.PlateNumber, o => o.MapFrom(t => t.NumberPlate));
			// Cancel Trip
			CreateMap<TripCancellationDTO, Trip>()
				.ForMember(t => t.FinishedTime, o => o.MapFrom(t => t.TimeFinished));
			#endregion

			#region Feedback
			// ListAll, List
			CreateMap<Feedback, FeedbackDTO>()
				.ForMember(f => f.TripStar, o => o.MapFrom(f => f.Star));
			// Create
			CreateMap<FeedbackCreateDTO, Feedback>()
				.ForMember(f => f.Star, o => o.MapFrom(f => f.TripStar));
			#endregion

			#region Bike
			// List, Detail
			CreateMap<Bike, BikeDTO>()
				.ForMember(b => b.NumberPlate, o => o.MapFrom(b => b.PlateNumber));
			// Create
			CreateMap<BikeCreateDTO, Bike>()
				.ForMember(b => b.PlateNumber, o => o.MapFrom(b => b.NumberPlate));
			#endregion

			#region Trip Transaction
			// List, Detail, DetailTrip
			CreateMap<TripTransaction, TripTransactionDTO>()
				.ForMember(t => t.AmountPoint, o => o.MapFrom(t => t.AmountOfPoint));
			#endregion

			#region Intimacy
			// List, Detail
			CreateMap<Intimacy, IntimacyDTO>();
			// Edit, Create
			CreateMap<IntimacyCreateEditDTO, Intimacy>();
			#endregion

			#region Voucher's Category
			// List, Detail
			CreateMap<VoucherCategory, VoucherCategoryDTO>();
			// Edit
			CreateMap<VoucherCategoryDTO, VoucherCategory>()
				.ForMember(v => v.VoucherCategoryId, o => o.Ignore())
				.ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
			// Create
			CreateMap<VoucherCategoryCreateDTO, VoucherCategory>();
			#endregion

			#region Voucher
			// List, Detail
			CreateMap<Voucher, VoucherDTO>();
			// Edit
			CreateMap<VoucherEditDTO, Voucher>()
				.ForMember(v => v.VoucherId, o => o.Ignore())
				.ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
			// Create
			CreateMap<VoucherCreateDTO, Voucher>();
			#endregion

			#region Redemption
			// ListUserRedemption, ListRedemption
			CreateMap<Redemption, RedemptionDTO>();
			// ListUserRedemptionAndVoucher
			CreateMap<Redemption, RedemptionAndVoucherDTO>()
				.ForMember(r => r.VoucherCategoryId, o => o.MapFrom(u => u.Voucher.VoucherCategoryId))
				.ForMember(r => r.VoucherName, o => o.MapFrom(u => u.Voucher.VoucherName))
				.ForMember(r => r.Brand, o => o.MapFrom(u => u.Voucher.Brand))
				.ForMember(r => r.StartDate, o => o.MapFrom(u => u.Voucher.StartDate))
				.ForMember(r => r.EndDate, o => o.MapFrom(u => u.Voucher.EndDate))
				.ForMember(r => r.Description, o => o.MapFrom(u => u.Voucher.Description))
				.ForMember(r => r.TermsAndConditions, o => o.MapFrom(u => u.Voucher.TermsAndConditions));
			// Create
			CreateMap<RedemptionCreateDTO, Redemption>();
			#endregion

			#region Wallet
			// List, Detail
			CreateMap<Wallet, WalletDTO>();
			// Edit
			CreateMap<WalletDTO, Wallet>()
				.ForMember(w => w.WalletId, o => o.Ignore())
				.ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
			// Create
			CreateMap<WalletCreateDTO, Wallet>();
			#endregion
		}
	}
}