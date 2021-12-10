class vi_VN{
    //Common error     
    //VanLNT
    static const String kException = "Đã có vấn đề xảy ra, xin thử lại sau!";
    static const String kCancelRequest = "Đã hủy yêu cầu";
    static const String kAdsPageEqualZero = "Số trang phải lớn hơn 0";
    static const String kAdsLimitEqualZero = "Giới hạn list phải lớn hơn 0";
    
        
    //Address 
    //VanLNT
    static const String kFailToCreate = "Không thể tạo địa chỉ mới";
    static const String kAddressNotFound = "Địa chỉ không tồn tại";
    static const String kFailedToDeleteAddress = "Không thể xóa địa chỉ này";
    static const String kFailedToUpdateAddress = "Cập nhật không thành công";
    

    //Advertisement
    //VanLNT
    static const String kAdsNotFound = "Quảng cáo này không tồn tại";
    static const String kFailedToUpdateClickCount = "Không thể cập nhật số lượt xem của quáng cáo này";
    static const String kEndDateSoonerThanStartDate = "Ngày kết thúc phải trễ hơn ngày bắt đầu";
    static const String kFailedToCreateAds = "Tạo quảng cáo không thành công";
    static const String kFailedToDeleteAds = "Đã có lỗi xảy ra khi xóa quảng cáo này";
    static const String kFailedToUpdateAds = "Đã xảy ra lỗi khi cập nhật quảng cáo này";
    static const String kAdsImageMissed = "Hình ảnh quảng cáo cần được cung cấp";
    static const String kAdsImageCreateFailed = "Đã có lỗi xảy ra khi thêm ảnh quảng cáo";
    static const String kNoImageId = "Yêu cầu này không có ID hình ảnh";
    static const String kNotMatchAdsImageId = "Ảnh quảng cáo với ID này không thuộc về quảng cáo với ID này";
    
    

    //Bike Availability 
    //VanLNT
    static const String kFailedToCreateBikeAvailability = "Đã có lỗi xảy ra khi tạo xe";
    static const String kCreateBikeSuccesfull = "Tạo xe thành công";
    static const String kBikeAvailabilityNotExist = "Tính khả dụng của xe này không tồn tại";
    static const String kBikeAvailabilityDoesNotBelongsToUser = "Tính khả dụng của xe này không thuộc về người dùng này";
    static const String kFailedToDeleteBikeAvaibility = "Đã có lỗi xảy ra khi xóa tính khả dụng của xe này";
    static const String kInvalidFromTime5AM = "Thời gian bắt đầu phải sau 5AM";
    static const String kInvalidFromTime9PM = "Thời gian bắt đầu phải trước 9 PM";
    static const String kInvalidFromToTime = "Thời gian bắt đầu phải sớm hơn thời gian kết thúc";
    static const String kFailedtToUpdateBikeAvailability = "Đã có lỗi xảy ra khi cập nhật khả dụng xe";
    

    //Bike
    //VanLNT
    static const String kBikeUserNotExist = "Người dùng này không tồn tại";
    static const String kUserHasABike = "Người dùng đã có xe";
    static const String kFailedToCreateBike = "Đã có lỗi xảy ra trong quá trình tạo xe";
    static const String kUserDeleteBikeNotExist = "Người dùng xóa xe không tồn tại";
    static const String kFailedToDeleteBike = "Đã có lỗi xảy ra trong quá trình xóa xe";
    static const String kErrorCreateUserFireBase = "Lỗi tạo người dùng ở FireBase";
    static const String kBikeNotFound = "Không tìm thấy xe";
    static const String kReplaceBikeFailed = "Thay thế thông tin xe không thành công";
    static const String kBikeNotVerified = "Cần có lý do vì sao xe không được xác minh";
    static const String kBikeVerifiedFailed = "Xác minh xe không thành công";
    

    //Configurations
    //VanLNT
    static const String kCreateConfigurationFailed = "Tạo cấu hình không thành công";
    static const String kConfigurationNotExisted = "Cấu hình không tồn tại";
    static const String kConfigurationName = "Trường Tên cấu hình là bắt buộc";
    static const String kConfigurationValue = "Trường Giá Trị cấu hình là bắt buộc";
    static const String kConfigurationUpdateFailed = "Cập nhật cấu hình không thành công";
    
    //Core
    //VanLNT 

    //Feedback
    //VanLNT
    static const String kTripNotExist = "Chuyến không tồn tại";
    static const String kFeedbackInTripID = "Nguời dùng gửi đánh giá chuyến phải ở trong chuyến này";
    static const String kTripCancelled = "Chuyến này đã bị hủy";
    static const String kFeedbackWhileInTrip = "Chưa thể đánh giá chuyến này vì chuyến đi chưa kết thúc";
    static const String kFeedbackExisted = "Chuyến này đã được đánh giá rồi";
    static const String kKeerNotExisted = "Người đi ké không tồn tại";
    static const String kTipRules = "Điểm Tip phải lớn hơn 1 và bé hơn tổng điểm của Keer";
    static const String kKeerNotHaveWallet = "Keer chưa có ví BiiKe";
    static const String kCreateNewFeedbackFailed = "Tạo đánh giá chuyến mới không thành công";
    static const String kCreateFeedbackFailed = "Tạo đánh giá chuyến không thành công";
    static const String kFeedbackNotExisted = "Đánh giá chuyến không tồn tại";
    static const String kUserNotInTrip = "Người dùng gửi yêu cầu phải ở trong chuyến xe này";
    static const String kTripHasNotFinished = "Chuyến này chưa kết thúc";
    

    //Intimacies
    //VanLNT
    static const String kIntimaciesExisted = "Quan hệ này đã tồn tại";
    static const String kUser2NotExisted = "Người dùng còn lại không tồn tại";
    static const String kCreateIntimacyFailed = "Tạo quan hệ không thành công";
    static const String kUpdateIntimacyFailed = "Chỉnh sửa quan hệ không thành công";
    

    //Momo Transaction 
    //VanLNT
    static const String kCheckMomoFailed = "Kiểm tra giao dịch momo không thành công";
    static const String kAmountNotMatch = "Giá trị không khớp với Momo server";
    static const String kTransactionIDNotMatch = "Giá trị ID giao dịch không khớp với Momo server";
    static const String kTransactionIDExisted = "Giao dịch đã tồn tại trước đó";
    static const String kFindConversionRateFailed = "Tìm giá trị quy đổi thất bại";
    static const String kConverVNDToPointFailed = "Quy đổi VND -> điểm BiiKe thất bại";
    static const String kUserNotExisted = "Người dùng không tồn tại";
    static const String kUserNotHaveWallet = "Người dùng không có ví";
    static const String kCreateMomoTransactionFailed = "Tạo giao dịch Momo không thành công";
    

    //Notification
    //VanLNT
    static const String kSentNotiSuccess = "Gửi thông báo đến người dùng chỉ định thành công";

    //Point History 
    //VanLNT
    static const String kCreatePointHistoryFailed = "Tạo lịch sử điểm mới không thành công";
    

    //Redemptions
    //VanLNT
    static const String kVoucherNotExisted = "Ưu đãi không tồn tại";
    static const String kVoucherExprired = "Ưu đãi đã hết hạn";
    static const String kVoucherNotForChangedYet = "Ưu đãi chưa được khả dụng";
    static const String kNoCodeForVoucher = "Không có mã code khả dụng cho voucher này";
    static const String kUserNotHaveEnoughPoint = "Người dùng không có đủ điểm";
    static const String kCreateRedemptionFailed = "Đổi ưu đãi không thành công";
    static const String kRedemptionNotExisted = "Đổi ưu đãi không tồn tại";
    static const String kCreateBikeSuccesfull = "";
    static const String kCreateBikeSuccesfull = "";
    static const String kCreateBikeSuccesfull = "";
    static const String kCreateBikeSuccesfull = "";

    //Report
    //VanLNT
    static const String kCreateReportFailed = "Đã có lỗi xảy ra khi tạo báo cáo "; 
    static const String kUserOneNotExist =   $"Người dùng đầu tiên với ID {request.ReportCreationDto.UserTwoId} không tồn tại."; 
    static const String kUserTwoNotExist = $"Người dùng thứ hai với ID {request.ReportCreationDto.UserTwoId} không tồn tại."; 
    static const String kCreateConfigurationFailed = "Báo cáo không tồn tại"; 
    static const String kUpdateReportFailed = $"Cập nhật thay đổi với báo cáo ID số {request.ReportId} không thành công.";

    //Route
    //VanLNT
    static const String kDepartureStationNotFound = $"Không thể tìm trạm bắt đầu với ID {request.RouteCreationDto.DepartureId}."; 
    static const String kDestinationStationNotFound =  $"Không thể tìm trạm kết thúc với ID {request.RouteCreationDto.DestinationId}."; 
    static const String DepartureDestinationNotMatchArea = "Trạm bắt đầu và trạm kết thúc không thuộc cùng khu vực."; 
    static const String kDepartureDestinationExisted =  $"Tuyến đi với điểm bắt đầu ID {request.RouteCreationDto.DepartureId} và điểm kết thúc ID {request.RouteCreationDto.DestinationId} đã tồn tại trước đó."; 
    static const String kCreateRouteFailed = "Tạo tuyến đi không thành công.";  
    static const String kRouteNotExisted = "Tuyến đi không tồn tại"; 
    static const String kCreateRouteFailed = $"Xóa tuyến đi với ID {request.RouteId} không thành công."; 
    static const String kRouteDeleted = $"Tuyến đi với ID {request.RouteId} đã bị xóa." + 
                                        "Xin hãy kích hoạt chuyến đi để tiếp tục sử dụng."; 
    static const String kUpdateRouteFailed = $"Cập nhật thông tin tuyến đi với ID {request.RouteId} không thành công."; 

    //SoS
    //VanLNT
    static const String kSoSMax = "Tài khoản này đã đạt giới hạn số lượng số điện thoại khẩn cấp: 3"; 
    static const String kCreateSoSNumFailed = "Tạo số điện thoại khẩn cấp không thành công"; 
    static const String kSoSNotExist = "Số điện thoại khẩn cấp không tồn tại"; 
    static const String kSoSNotBelongsTouser = "Số điện thoại này không phải số điện thoại khẩn cấp của người dùng"; 
    static const String kDeleteSoSFailed = $"Xóa số điện thoại khẩn cấp ID {request.SosId} không thành công."; 
    static const String kUpdateSoSFailed = $"Cập nhật số điện thoại khẩn cấp {request.SosId} không thành công."; 

    //Station 
    //VanLNT
    static const String kRadiusNotFound = "Tạo trạm không thành công vì thông tin cấu hình về phạm vi hoạt động không tồn tại."; 
    static const String kCentralStationNotFound = "Không thể tạo trạm vì không tìm thấy trạm trung tâm."; 
    static const String kDistanceLargerThanRadius = "Tạo trạm mới không thành công vì " +
                            $"khoảng cách giữa trạm và trạm trung tâm lớn hơn {activeRadius}."; 
    static const String kCreateStationFailed = "Tạo trạm mới không thành công."; 
    static const String kRadiusConfigError = "Lỗi thông số phạm vi hoạt động."; 
    static const String kStationNotExist = "Trạm không tồn tại."; 
    static const String kDeleteCentralPoint = "Không thể xóa trạm trung tâm của khu vực."; 
    static const String kDeleteStationFailed = $"Xóa trạm có ID {request.StationId} không thành công."; 
    static const String kDeleteStation = $"Trạm có ID {request.StationId} đã bị xóa. " +
                                                    "Vui lòng kích hoạt lại trạm nếu tiếp tục sử dụng."; 
    static const String kUpdateStationFailed = $"Cập nhật trạm có ID {request.StationId} không thành công."; 
    static const String kDapartureIDEqualsZerro = "ID của trạm bắt đầu lớn hơn 0."; 
    static const String kDestinationIDEqualsZerro = "ID của trạm kết thúc 0."; 
    static const String kDepartDestiNotProvided = "ID của trạm bắt đầu và trạm kết thúc cần được cung cấp."; 
    static const String kDepartIdDestiIDAtOneTime = "Only DepartureId or DestinationId can be provided at a time." +
                                "Do not send both parameters"; 

    //Trip Transaction
    //VanLNT
    static const String kCreateConfigurationFailed = $"Chuyến đi có ID {trip.TripId} không có người chở."; 
    static const String kCreateConfigurationFailed = $"Người chở với ID {trip.BikerId} không tồn tại."; 
    static const String kCreateConfigurationFailed = $"Người chở với ID {trip.BikerId} không có ví."; 
    static const String kCreateConfigurationFailed = "Tạo giao dịch chuyến mới không thành công."; 
    static const String kCreateConfigurationFailed = $"Người dùng với ID {trip.KeerId} không tồn tại."; 
    static const String kCreateConfigurationFailed =  $"Giao dịch chuyến với ID {request.TripTransactionId} không tồn tại."; 

    //Trips
    //VanLNT
    static const String kBikerNotFoundNoti = "Không tìm thấy người chở cho thông báo của chuyến đi"; 
    static const String kTripNotFoundCancel = "Không tìm thấy ID hủy chuyến"; 
    static const String kTripNotExist = "Chuyến không tồn tại"; 
    static const String kTripFinished = "Chuyến đã kết thúc"; 
    static const String kTripCanceled = "Chuyến đã bị hủy";
    static const String kTripStarted = "Chuyến đã bắt đầu";  
    static const String kCancelTripAutoFailed = "Tự động hủy chuyến không thành công"; 
    static const String kTripUserNotExist = $"Người dùng với ID {request.UserId} không tồn tại"; 
    static const String kDateInvalid = "Giá trị ngày không hợp lệ"; 
    static const String kTimeInvalid = "Giá trị tháng không tồn tại"; 
    static const String kDateLaterThanCurrentTime =  $"Giá trị ngày phải sau thời gian hiện tại {currentTime}."; 
    static const String kValidTime = "Thời gian khả dụng phải sau 5AM và trước 9PM"; 
    static const String kBikeKeerSamePerson = "Người chở và người đi ké không thể là cùng một người"; 
    static const String kBikerNotExist = "Người chở không tồn tại"; 
    static const String kBikerNotHaveVerifiedBike = "Người chở chưa có xe nào được xác minh"; 
    static const String kBikeNotExist = "Xe không tồn tại"; 
    static const String kCanceTriplRule = "Người hủy chuyến phải là người đi ké hoặc người chở trong chuyến"; 
    static const String kCancelTripFailed = $"Hủy chuyến có ID {request.TripId} không thành công."; 
    static const String kCreateScheduleTripFailedBooktimeOneHour = "Tạo chuyến được lên lịch không thành công vì thời gian đặt chuyến " +
                                                    $"{request.TripCreationDto.BookTime} cách thời gian khởi hành {limitOneHourTime}."; 
    static const String kCreateTripFailedBooktimeFifteenMinutes = "Tạo chuyến ké now không thành công vì thời gian đặt chuyến" +
                                                    $"{request.TripCreationDto.BookTime} cách thời gian khởi hành lớn hơn {limitFifteenMinutesTime}."; 
    static const String kCreateTripFailedBooktimeEarlierThanCurrenttime = "Tạo chuyến ké now không thành công vì thời gian đặt chuyến " +
                                                    $"{request.TripCreationDto.BookTime} lớn hơn thời gian hiện tại {currentTime}."; 
    static const String kRouteDepartAndDestiNotExist = $"Tuyến với trạm khởi hành ID {request.TripCreationDto.DepartureId} và trạm kết thúc ID " +
                            $"DestinationId {request.TripCreationDto.DestinationId} không tồn tại."; 
    static const String kBookTimeExisted = "Tạo chuyến không thành công vì chuyến với thời gian này" +
                                                    $"{request.TripCreationDto.BookTime} đã tồn tại " +
                                                    $"trong lịch trình của người đi ké hoặc người chở {request.TripCreationDto.KeerId}."; 
    static const String kReachMaxTrip =  $"Tạo chuyến lên lịch không thành công vì đã đạt số lượng chuyến tối đa có thể đặt ({Constant.MaxTripCount})."; 
    static const String kCreateTripFailed = "Tạo chuyến mới không thành công."; 
    static const String kNoBikeAvailable = "Hiện tại không có tài xế nào sẵn sàng."; 
    static const String kUserSentRequestNotExist = "Người gửi yêu cầu không tồn tại."; 
    static const String kTripByIDNotExist = $"Chuyến với ID {request.TripId} không tồn tại."; 
    static const String kUnauthorizeContent = $"Người dùng không có quyền truy cập tính năng này"; 
    static const String kBikeNotHaveBike = "Người chở không có xe"; 
    static const String kFinishtripWithoutBiker = "Chuyến cần có người chở trước khi kết thúc"; 
    static const String kBikerFinishTripRule = "Chỉ có người chở của chuyến này có quyền gửi yêu cầu két thúc chuyến đi"; 
    static const String kUpdateTripByIDFailed = $"Cập nhật chuyến với ID {request.TripId} không thành công. " +
                                                    (ex.InnerException?.Message ?? ex.Message); 
    static const String kCreateScheduleTripFailedBooktimeEmpty = "Tạo chuyến đặt lịch không thành công vì thời gian đặt trống"; 
    static const String kDuplicateBookTime = "Tạo chuyến đặt lịch không thành công vì thời gian đặt trùng"; 
    static const String kTripBooktimeExisted = "Tạo chuyến không thành công vì chuyến thời gian đặt " +
                                                    $"{request.TripScheduleCreationDto.BookTime!.First()} đã tồn tại."; 
    static const String kReachMaxScheduleTrip = $"Tạo chuyến không thành công vì đã đạt tối đa số chuyến có thể đặt ({Constant.MaxTripCount})."; 
    static const String kCreateMultipleTripScheduleFailed = "Tạo nhiều chuyến đặt lịch không thành công"; 
    static const String kStartTripBikerRule = "Chuyến cần có người chở trước khi bắt đầu"; 
    static const String kBikerNotBelongsToTrip = $"Người chở với ID {request.UserId} không ở trong chuyến."; 
    static const String kStartTripFailedNoOneArrived = "Không thể bắt đầu chuyến vì chưa ai đến điểm chờ"; 
    static const String kStartTripFaileOnePersonNotArrived = $"Không thể bắt đầu chuyến vì {personRole} chưa đến điểm chờ."; 
    static const String kTriWaitingBikerRule = "Chuyến phải có người chở trước khi chuyến sang trạng thái chờ"; 
    static const String kUserRequestEndpointOnce =   $"Người dùng với ID {request.UserId} không có quyền truy cập tính năng này"; 

    //User 
    //VanLNT
    static const String kUserVerifiedEmailAlready =   $"Người dùng với ID {request.UserId} đã xác thực email.";
    static const String kUserVerifiedPhoneAlready =   $"Người dùng với ID {request.UserId} đã xác thực số điện thoại."; 
    static const String kVerifiedUserFailed =   $"Xác thực người dùng với ID {request.UserId} không thành công."; 
    static const String kUserDeleted =   $"Người dùng với ID {request.UserId} đã bị xóa. " +
                                                    "Hãy kích hoạt lại người dùng này nếu tiếp tục cần sử dụng."; 
    static const String kChangeRole2AdminLeft =   "Không thể thay đổi vai trò người dùng này vì chỉ còn lại 2 admin"; 
    static const String kDelete2AdminLeft =   "Không thể xóa người dùng vì chỉ còn lại 2 admin"; 
    static const String kEditUserFalseFireBase =   "Lỗi chỉnh sửa vai trò người dùng ở FireBase. " +
                                                    $"{e.InnerException?.Message ?? e.Message}"; 
    static const String kUpdateUserRoleFailed =   $"Cập nhật vai trò của người dùng có ID {request.UserId} thành {user.RoleId} không thành công."; 
    static const String kUserNotBiker =   $"Người dùng với ID {request.UserId} không phải là người chở."; 
    static const String kNoBikeAvailable =   "Không có người chở nào khả dụng tại thời điểm này."; 
    static const String kNoUserOnFireBase =   "Không thể xóa ID của người dùng này ở Firebase " +
                                                    "vì không có người dùng nào ở database"; 
    static const String kDeleteAllUserErrorFireBase =   "Lỗi xóa tất cả người dùng ở Firebase. " +
                                                    $"{e.InnerException?.Message ?? e.Message}"; 
    static const String kUserByIDNotExist =   $"Người dùng với ID {request.UserId} không tồn tại."; 
    static const String kUpdateUserLoginDeviceFailed =   $"Cập nhật thiết bị login cuối cùng của người dùng không thành công {request.UserId}."; 
    static const String kCreateNewAddressFailed =   "Tạo địa chỉ mới của người dùng không thành công."; 
    static const String kAddressNotExist =   "Địa chỉ này không tồn tại."; 
    static const String kDeleteDefaultAddress =   "Không thể xóa địa chỉ cố định."; 
    static const String kDeleteUserAddressFailed =    $"Xóa địa chỉ của người dùng với ID của địa chỉ {request.UserAddressId} không thành công."; 
    static const String kUserAddressNotExist =   "Địa chỉ của người dùng không tồn tại."; 
    static const String kUpdateUserAddressFailed =   $"Cập nhật địa chỉ của người dùng với ID địa chỉ {request.UserAddressId} không thành công."; 
    static const String kCreateUserFailed =   "Tạo người dùng mới không thành công."; 
    static const String kDeleteUserFailed =   $"Xóa người dùng với ID {request.UserId} không thành công."; 
    static const String kUpdateUserTripNowAvailabilityFailed =   $"Cập nhật khả dụng trip now của người dùng ID {request.UserId} không thành công."; 
    static const String kUserWithIfNotExist =   $"User with userId {auth.User.LocalId} doesn't exist."; 
    static const String kUpdateEmailVerificationFailed =   "Failed to update email verification."; 
    static const String kUserHasNotVerifiedEmail =   $"User with userId {auth.User.LocalId} hasn't verified email."; 
    static const String kUserNotAdmin =   $"User with userId {auth.User.LocalId} is not an admin."; 
    static const String kSaveLoginInfoFailed =   "Failed to save login information"; 
    static const String kUpdateUserProfileFailed =   $"Failed to update user's profile by userId {request.UserId}."; 
    static const String kBikerHasNotVerifiedBike =   "User's bike has not been verified."; 
    static const String kUserDoesNotHaveBike =   "User does not have bike."; 
    static const String kUserRequestEndpointOnce =   ""; 
    static const String kUserRequestEndpointOnce =   ""; 
    static const String kUserRequestEndpointOnce =   ""; 
    static const String kUserRequestEndpointOnce =   ""; 

}
