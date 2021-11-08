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

namespace Application.Core
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            #region Fix Type Conversion
            // ReSharper disable CommentTypo
            
            // Mặc định khi kiểu int? có giá trị null thì 
            // map qua kiểu int sẽ bị chuyển thành 0.
            // Nhưng chúng ta thường sẽ muốn ignore biến int? nếu
            // biến int? có giá trị null, và để nguyên giá trị gốc của int.
            // Câu map ở dưới đây sinh ra nhằm tránh việc chuyển giá trị
            // int gốc thành 0 khi truyền vào biến int? với giá trị null.
            // Việc truyền biến int? với giá trị null thường xảy ra 
            // khi người dùng không truyền các optional field 
            // ở trong body request của EditDTO.
            CreateMap<int?, int>().ConvertUsing((src, dest) => src ?? dest);

            // Tương tự int?, chúng ta tạo map cho DateTime?
            CreateMap<DateTime?, DateTime>().ConvertUsing((src, dest) => src ?? dest);

            // Tương tự int?, chúng ta tạo map cho bool?
            CreateMap<bool?, bool>().ConvertUsing((src, dest) => src ?? dest);

            // Tương tự int?, chúng ta tạo map cho double?
            CreateMap<double?, double>().ConvertUsing((src, dest) => src ?? dest);

            // ReSharper restore CommentTypo
            #endregion

            #region Station

            // List, Detail 
            CreateMap<Station, StationDto>();
            // Edit
            CreateMap<StationDto, Station>()
                .ForMember(s => s.StationId, opt => opt.Ignore())
                .ForMember(s => s.CreatedDate, opt => opt.Ignore())
                .ForMember(s => s.IsDeleted, opt => opt.Ignore())
                .ForAllMembers(o => o.Condition((_, _, srcMember) => srcMember != null));
            // Create
            CreateMap<StationCreationDto, Station>();

            #endregion

            #region Route

            // List, Detail 
            CreateMap<Route, RouteDto>()
                .ForMember(r => r.DepartureName, o => o.MapFrom(r => r.Departure.Name))
                .ForMember(r => r.DestinationName, o => o.MapFrom(r => r.Destination.Name));
            // Edit 
            CreateMap<RouteDto, Route>()
                .ForMember(r => r.RouteId, opt => opt.Ignore())
                .ForMember(r => r.AreaId, opt => opt.Ignore())
                .ForMember(r => r.CreatedDate, opt => opt.Ignore())
                .ForMember(r => r.IsDeleted, opt => opt.Ignore())
                .ForAllMembers(o => o.Condition((_, _, srcMember) => srcMember != null));
            // Create
            CreateMap<RouteCreationDto, Route>();

            #endregion

            #region UserAddress

            // List, Details
            CreateMap<UserAddress, UserAddressDto>();
            // Create
            CreateMap<UserAddressCreationDto, UserAddress>()
                .ForMember(u => u.IsDefault, o => o.Ignore())
                .ForAllMembers(o => o.Condition((_, _, srcMember) => srcMember != null));
            // Edit
            CreateMap<UserAddressDto, UserAddress>()
                .ForMember(u => u.UserAddressId, o => o.Ignore())
                .ForMember(u => u.IsDefault, o => o.Ignore())
                .ForAllMembers(o => o.Condition((_, _, srcMember) => srcMember != null));
            
            #endregion

            #region User

            // List, Detail
            CreateMap<User, UserDto>()
                .ForMember(u => u.UserPhoneNumber, o => o.MapFrom(u => u.PhoneNumber))
                .ForMember(u => u.UserFullname, o => o.MapFrom(u => u.FullName))
                .ForMember(u => u.UserStar, o => o.MapFrom(u => u.Star));
            // Create
            CreateMap<UserCreationDto, User>()
                .ForMember(u => u.PasswordHash, o => o.MapFrom(u => u.Password));
            // Edit Profile
            CreateMap<UserProfileEditDto, User>()
                .ForMember(u => u.FullName, o => o.MapFrom(u => u.UserFullname))
                .ForAllMembers(o => o.Condition((_, _, srcMember) => srcMember != null));
            // Edit LoginDevice
            CreateMap<UserLoginDeviceDto, User>()
                .ForAllMembers(o => o.Condition((_, _, srcMember) => srcMember != null));

            #endregion

            #region Trip

            // History Trips & Upcoming Trips
            // ReSharper disable once ConvertToConstant.Local
            var isKeer = true;
            CreateMap<Trip, TripDto>()
                .ForMember(t => t.UserId, o => o.MapFrom(t => isKeer ? t.BikerId : t.KeerId))
                .ForMember(t => t.Avatar,
                    o => o.MapFrom(t =>
                        t.Biker == null && isKeer ? null :
                        t.Biker == null && !isKeer ? t.Keer.Avatar :
                        t.Biker != null && isKeer ? t.Biker.Avatar : t.Keer.Avatar))
                .ForMember(t => t.UserFullname,
                    o => o.MapFrom(t =>
                        t.Biker == null && isKeer ? null :
                        t.Biker == null && !isKeer ? t.Keer.FullName :
                        t.Biker != null && isKeer ? t.Biker.FullName : t.Keer.FullName))
                .ForMember(t => t.UserPhoneNumber,
                    o => o.MapFrom(t =>
                        t.Biker == null && isKeer ? null :
                        t.Biker == null && !isKeer ? t.Keer.PhoneNumber :
                        t.Biker != null && isKeer ? t.Biker.PhoneNumber : t.Keer.PhoneNumber))
                .ForMember(t => t.TripStatus, o => o.MapFrom(t => t.Status))
                .ForMember(t => t.DepartureName, o => o.MapFrom(t => t.Route.Departure.Name))
                .ForMember(t => t.DestinationName, o => o.MapFrom(t => t.Route.Destination.Name));

            // TripHistoryPair
            // ReSharper disable once ConvertToConstant.Local
            var userTwoId = 0;
            CreateMap<Trip, TripPairDto>()
                .ForMember(t => t.UserId, o => o.MapFrom(t => userTwoId == t.BikerId ? t.BikerId : t.KeerId))
                .ForMember(t => t.Avatar, o => o.MapFrom(t => userTwoId == t.BikerId ? t.Biker!.Avatar : t.Keer.Avatar))
                .ForMember(t => t.UserFullname,
                    o => o.MapFrom(t => userTwoId == t.BikerId ? t.Biker!.FullName : t.Keer.FullName))
                .ForMember(t => t.TripStatus, o => o.MapFrom(t => t.Status))
                .ForMember(t => t.DepartureName, o => o.MapFrom(t => t.Route.Departure.Name))
                .ForMember(t => t.DestinationName, o => o.MapFrom(t => t.Route.Destination.Name));

            // Detail Info
            CreateMap<Trip, TripDetailsFullDto>()
                //(src, dest, destMember, resContext) => dest.UserId = resContext.Items["isKeer"] ? src.BikerId : src.KeerId)
                .ForMember(t => t.UserId, o => o.MapFrom((src, _, _, context) => (bool) context.Items["isKeer"] ? src.BikerId : src.KeerId))
                .ForMember(t => t.KeerId, o => o.MapFrom(t => t.KeerId))
                .ForMember(t => t.Avatar,
                    o => o.MapFrom((src, _, _, context) => src.Biker == null ? null : (bool) context.Items["isKeer"] ? src.Biker.Avatar : src.Keer.Avatar))
                .ForMember(t => t.UserFullname,
                    o => o.MapFrom((src, _, _, context) => src.Biker == null ? null : (bool) context.Items["isKeer"] ? src.Biker.FullName : src.Keer.FullName))
                .ForMember(t => t.UserPhoneNumber,
                    o => o.MapFrom((src, _, _, context) => src.Biker == null ? null : (bool) context.Items["isKeer"] ? src.Biker.PhoneNumber : src.Keer.PhoneNumber))
                .ForMember(t => t.UserStar,
                    o => o.MapFrom((src, _, _, context) => src.Biker == null ? new double?() : (bool) context.Items["isKeer"] ? src.Biker.Star : src.Keer.Star))
                .ForMember(t => t.TripStatus, o => o.MapFrom(t => t.Status))
                .ForMember(t => t.DepartureName, o => o.MapFrom(t => t.Route.Departure.Name))
                .ForMember(t => t.DepartureCoordinate, o => o.MapFrom(t => t.Route.Departure.Coordinate))
                .ForMember(t => t.DestinationName, o => o.MapFrom(t => t.Route.Destination.Name))
                .ForMember(t => t.DestinationCoordinate, o => o.MapFrom(t => t.Route.Destination.Coordinate))
                .ForMember(t => t.Feedbacks, o => o.MapFrom(t => t.FeedbackList));

            // Detail 
            CreateMap<Trip, TripDetailsDto>()
                .ForMember(t => t.KeerFullname, o => o.MapFrom(t => t.Keer.FullName))
                .ForMember(t => t.BikerFullname, o => o.MapFrom(t => t.Biker == null ? null : t.Biker.FullName))
                .ForMember(t => t.CancelPersonFullname,
                    o => o.MapFrom(t =>
                        t.CancelPersonId == null ? null :
                        t.CancelPersonId == t.KeerId ? t.Keer.FullName :
                        t.Biker == null ? null : t.Biker.FullName))
                .ForMember(t => t.DepartureStationName, o => o.MapFrom(t => t.Route.Departure.Name))
                .ForMember(t => t.DestinationStationName, o => o.MapFrom(t => t.Route.Destination.Name));
            // Create
            CreateMap<TripCreationDto, Trip>();
            // Cancel Trip
            CreateMap<TripCancellationDto, Trip>();

            #endregion

            #region Feedback

            // ListAll, List
            CreateMap<Feedback, FeedbackDto>()
                .ForMember(f => f.UserFullname, o => o.MapFrom(f => f.User.FullName));
            // Create
            CreateMap<FeedbackCreationDto, Feedback>();

            #endregion

            #region Bike

            // List, Detail
            CreateMap<Bike, BikeDto>();
            // Create
            CreateMap<BikeCreationDto, Bike>();

            #endregion

            #region Trip Transaction

            // List, Detail, DetailTrip
            CreateMap<TripTransaction, TripTransactionDto>();

            #endregion

            #region Intimacy

            // List, Detail
            CreateMap<Intimacy, IntimacyDto>()
                .ForMember(i => i.UserName, o => o.MapFrom(i => i.UserTwo.FullName));
            // Edit, Create
            CreateMap<IntimacyModificationDto, Intimacy>();

            #endregion

            #region Voucher's Category

            // List, Detail
            CreateMap<VoucherCategory, VoucherCategoryDto>();
            // Edit
            CreateMap<VoucherCategoryDto, VoucherCategory>()
                .ForMember(v => v.VoucherCategoryId, o => o.Ignore())
                .ForAllMembers(o => o.Condition((_, _, srcMember) => srcMember != null));
            // Create
            CreateMap<VoucherCategoryCreationDto, VoucherCategory>();

            #endregion

            #region Voucher Address
            
            // List, Details
            CreateMap<VoucherAddress, VoucherAddressDto>();
            // Create
            CreateMap<VoucherAddressCreationDto, VoucherAddress>();
            // Edit
            CreateMap<VoucherAddressDto, VoucherAddress>()
                .ForMember(v => v.VoucherAddressId, o => o.Ignore())
                .ForMember(v => v.CreatedDate, o => o.Ignore())
                .ForAllMembers(o => o.Condition((_, _, srcMember) => srcMember != null));
            
            #endregion
            
            #region Voucher Image

            CreateMap<VoucherImage, VoucherImageDto>();

            #endregion

            #region Voucher

            // List, Detail
            CreateMap<Voucher, VoucherDto>()
                .ForMember(v => v.VoucherCategoryName, o => o.MapFrom(v => v.VoucherCategory.CategoryName));
            // Edit
            CreateMap<VoucherEditDto, Voucher>()
                .ForMember(v => v.VoucherId, o => o.Ignore())
                .ForMember(v => v.VoucherAddresses, o => o.Ignore())
                .ForMember(v => v.VoucherImages, o => o.Ignore())
                .ForAllMembers(o => o.Condition((_, _, srcMember) => srcMember != null));
            // Create
            CreateMap<VoucherCreationDto, Voucher>()
                .ForMember(v => v.VoucherAddresses, opt => opt.Ignore())
                .ForMember(v => v.VoucherImages, opt => opt.Ignore());

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
            CreateMap<RedemptionCreationDto, Redemption>();

            #endregion

            #region Wallet

            // List, Detail
            CreateMap<Wallet, WalletDto>();
            // Edit
            CreateMap<WalletDto, Wallet>()
                .ForMember(w => w.WalletId, o => o.Ignore())
                .ForAllMembers(o => o.Condition((_, _, srcMember) => srcMember != null));
            // Create
            CreateMap<WalletCreationDto, Wallet>();

            #endregion
        }
    }
}