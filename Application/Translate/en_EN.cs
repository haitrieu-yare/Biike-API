class en_EN{
    //Common Error 
    //VanLNT
    static const String kException = "Something went wrong, please try again!";
    static const String kCancelRequest = "Request was cancelled";
    static const String kPageEqualZero = "Page must be larger than 0";
    static const String kLimitEqualZero = "Limit must be larger than 0";

    //Address 
    //VanLNT
    static const String kFailToCreate = "Failed to create new address";
    static const String kAddressNotFound = "Address does not exist";
    static const String kFailedToDeleteAddress = "Failed to delete this address";
    static const String kFailedToUpdateAddress = "Failed to update this address";
    static const String kFailedToUpdateAddress = "Failed to update this address";
    
    //Advertisement
    //VanLNT
    static const String kAdsNotFound = "Advertisement doesn't  exist";
    static const String kFailedToUpdateClickCount = "Failed to update click count of this advertisement";
    static const String kEndDateSoonerThanStartDate = "EndDate must be set later than StartDate";
    static const String kFailedToCreateAds = "Failed to create new advertisement";
    static const String kFailedToDeleteAds = "Failed to delete advertisement";
    static const String kFailedToUpdateAds = "Failed to update this advertisement";
    static const String kAdsImageCreateFailed = "Failed to create new advertisement image";
    static const String kNoImageId = "There are no imageId in request";
    static const String kNotMatchAdsImageId = "ID of this Advertisement Image does not belong to ID of this advertisement"; 

    //Bike Availability 
    //VanLNT
    static const String kFailedToCreateBikeAvailability = "Failed to create new bike availability"; 
    static const String kBikeAvailabilityNotExist = "Bike availability doesn't exist"; 
    static const String kBikeAvailabilityDoesNotBelongsToUser = "This bike availability doesn't belong to user with this User"; 
    static const String kInvalidFromTime5AM = "FromTime must be later than 5AM"; 
    static const String kInvalidFromTime9PM = "FromTime must be earlier than 9PM"; 
    static const String kInvalidFromToTime = "FromTime must be earlier than ToTime"; 
    static const String kFailedtToUpdateBikeAvailability = "Failed to update bike availability"; 
    static const String kFailedToCreateBikeAvailability = "Failed to create new bike availability"; 

    //Bike
    //VanLNT
    static const String kBikeUserNotExist = "User does not exist"; 
    static const String kUserHasABike = "User already has a bike"; 
    static const String kFailedToCreateBike = "Failed to create new bike"; 
    static const String kUserDeleteBikeNotExist = "User to delete bike does not exist"; 
    static const String kFailedToDeleteBike = "Failed to delete bike"; 
    static const String kErrorCreateUserFireBase = "Error create user on Firebase"; 
    static const String kBikeNotFound = "Could not found bike"; 
    static const String kReplaceBikeFailed = "Failed to replace old bike with new bike"; 
    static const String kBikeVerifiedAlready = "Bike has already been verified"; 
    static const String kBikeNotVerified = "Failed verification need a reason about why it is not verified"; 
    static const String kBikeVerifiedFailed = "Failed to verify bike"; 

    //Configurations
    //VanLNT
    static const String kCreateConfigurationFailed = "Failed to create new configuration"; 
    static const String kConfigurationNotExisted = "Configuration doesn't exist"; 
    static const String kConfigurationName = "ConfigurationName field is required"; 
    static const String kConfigurationValue = "ConfigurationValue field is required"; 
    static const String kConfigurationUpdateFailed = "Failed to update configuration"; 

    //Core
    //VanLNT 

    //Feedback
    //VanLNT
    static const String kTripNotExist = "Trip doesn't exist"; 
    static const String kFeedbackInTripID = "User send feedback must be in this trip"; 
    static const String kTripCancelled = "Trip has already been cancelled"; 
    static const String kFeedbackWhileInTrip = "Can't create feedback because trip hasn't finished yet"; 
    static const String kFeedbackExisted = "Trip's feedback is already existed"; 
    static const String kKeerNotExisted = "Keer doesn't existed"; 
    static const String kTipRules = "Tip point should be larger than 1 and smaller than total point of keer"; 
    static const String kKeerNotHaveWallet = "Keer with UserId doesn't have wallet"; 
    static const String kCreateNewFeedbackFailed = "Failed to create new trip feedback"; 
    static const String kCreateFeedbackFailed = "Failed to create new feedback"; 
    static const String kFeedbackNotExisted = "Feedback doesn't exist"; 
    static const String kUserNotInTrip = "User send request must be in the trip"; 
    static const String kTripHasNotFinished = "Trip hasn't finished"; 

    //Intimacies
    //VanLNT
    static const String kIntimaciesExisted = "Intimacy has already existed"; 
    static const String kUser2NotExisted = "User two doesn't exist"; 
    static const String kCreateIntimacyFailed = "Failed to create new intimacy"; 
    static const String kUpdateIntimacyFailed = "Failed to update intimacy"; 
    
    //Momo Transaction 
    //VanLNT
    static const String kCheckMomoFailed = "Failed to check momo transaction on Momo server"; 
    static const String kAmountNotMatch = "Amount value doesn't match with Momo server"; 
    static const String kTransactionIDNotMatch = "TransactionId value doesn\'t match with Momo server"; 
    static const String kTransactionIDExisted = "Momo transaction with this TransactionId or OrderId is already exist"; 
    static const String kFindConversionRateFailed = "Failed to find conversion rate"; 
    static const String kConverVNDToPointFailed = "Failed to convert money to point"; 
    static const String kUserNotExisted = "User doesn't exist"; 
    static const String kUserNotHaveWallet = "User doesn't have wallet"; 
    static const String kCreateMomoTransactionFailed = "Failed to create new momo transaction"; 

    //Notification
    //VanLNT
    static const String kSentNotiSuccess = "Successfully sent notification to selected users"; 

    //Point History 
    //VanLNT
    static const String kCreatePointHistoryFailed = "Failed to create new point history"; 
    
    //Redemptions
    //VanLNT
    static const String kVoucherNotExisted = "Voucher doesn't exist"; 
    static const String kVoucherExprired = "Voucher has expired"; 
    static const String kVoucherNotForChangedYet = "Voucher is not open for exchange yet"; 
    static const String kNoCodeForVoucher = "There is no available voucher code for this voucher"; 
    static const String kUserNotHaveEnoughPoint = "There is no available voucher code for this voucher"; 
    static const String kCreateRedemptionFailed = "Failed to create new redemption"; 
    static const String kRedemptionNotExisted = "Redemption doesn't exist"; 
    static const String kUerNotHaveAnyRedemption = "User doesn't have any redemption"; 
    
    //Report
    //VanLNT
    static const String kCreateReportFailed = "Failed to create new report"; 
    static const String kUserOneNotExist =   $"User one with UserId {request.ReportCreationDto.UserTwoId} doesn't exist."; 
    static const String kUserTwoNotExist = $"User two with UserId {request.ReportCreationDto.UserTwoId} doesn't exist."; 
    static const String kCreateConfigurationFailed = "Report doesn't exist"; 
    static const String kUpdateReportFailed = $"Failed to update status of report with ReportId {request.ReportId}."; 
    
    //Route
    //VanLNT
    static const String kDepartureStationNotFound = $"Could not find departure station with StationId {request.RouteCreationDto.DepartureId}."; 
    static const String kDestinationStationNotFound =  $"Could not find destination station with StationId {request.RouteCreationDto.DestinationId}."; 
    static const String DepartureDestinationNotMatchArea = "Departure station and destination station does not belong to the same area."; 
    static const String kDepartureDestinationExisted =  $"Route with departureId {request.RouteCreationDto.DepartureId} and destinationId {request.RouteCreationDto.DestinationId} is already existed."; 
    static const String kCreateRouteFailed = "Failed to create new route."; 
    static const String kCreateRouteFailed = "Failed to create new route."; 
    static const String kRouteNotExisted = "Route doesn't exist"; 
    static const String kCreateRouteFailed = $"Failed to delete route by routeId {request.RouteId}."; 
    static const String kRouteDeleted = $"Route with RouteId {request.RouteId} has been deleted." + 
                                        "Please reactivate it if you want to edit it."; 
    static const String kUpdateRouteFailed = $"Failed to update route by routeId {request.RouteId}."; 

    //SoS
    //VanLNT
    static const String kSoSMax = "This user has reach the maximum number of sos (3) can be created"; 
    static const String kCreateSoSNumFailed = "Failed to create new sos."; 
    static const String kSoSNotExist = "Sos doesn't exist."; 
    static const String kSoSNotBelongsTouser = "This sos doesn't belong to this user."; 
    static const String kDeleteSoSFailed = $"Failed to delete sos by sosId {request.SosId}."; 
    static const String kUpdateSoSFailed = $"Failed to update sos by sosId {request.SosId}."; 

    //Station 
    //VanLNT
    static const String kRadiusNotFound = "Failed to create new station because active radius is not found."; 
    static const String kCentralStationNotFound = "Failed to create new station because central station is not found."; 
    static const String kDistanceLargerThanRadius = "Failed to create new station because " +
                            $"the distance between new station and central station is larger than {activeRadius}."; 
    static const String kCreateStationFailed = "Failed to create new station."; 
    static const String kRadiusConfigError = "Active radius configuration error."; 
    static const String kStationNotExist = "Station doesn't exist."; 
    static const String kDeleteCentralPoint = "Can not delete this station because it is central point of the area."; 
    static const String kDeleteStationFailed = $"Failed to delete station by stationId {request.StationId}."; 
    static const String kDeleteStation = $"Station with StationId {request.StationId} has been deleted. " +
                                                    "Please reactivate it if you want to edit it."; 
    static const String kUpdateStationFailed = $"Failed to update station by stationId {request.StationId}."; 
    static const String kDapartureIDEqualsZerro = "DepartureId must be larger than 0."; 
    static const String kDestinationIDEqualsZerro = "DestinationId must be larger than 0."; 
    static const String kDepartDestiNotProvided = "DepartureId or DestinationId must be provided."; 
    static const String kDepartIdDestiIDAtOneTime = "Only DepartureId or DestinationId can be provided at a time." +
                                "Do not send both parameters"; 

    //Trip Transaction
    //VanLNT
    static const String kCreateConfigurationFailed = $"Trip with TripId {trip.TripId} doesn't have Biker."; 
    static const String kCreateConfigurationFailed = $"Biker with UserId {trip.BikerId} doesn't exist."; 
    static const String kCreateConfigurationFailed = $"Biker with UserId {trip.BikerId} doesn't have wallet."; 
    static const String kCreateConfigurationFailed = "Failed to create new trip transaction."; 
    static const String kCreateConfigurationFailed = $"User with UserId {trip.KeerId} doesn't exist."; 
    static const String kCreateConfigurationFailed =  $"Trip transaction with TripTransactionId {request.TripTransactionId} doesn't exist."; 

    //Trips
    //VanLNT
    static const String kBikerNotFoundNoti = "Could not find BikerId for trip notification"; 
    static const String kTripNotFoundCancel = "Could not find tripId for trip cancellation"; 
    static const String kTripNotExist = "Trip doesn't exist"; 
    static const String kTripFinished = "Trip has already finished"; 
    static const String kTripCanceled = "Trip has already cancelled";
    static const String kTripStarted = "Trip has already started.";  
    static const String kCancelTripAutoFailed = "Failed to automatically cancel trip"; 
    static const String kTripUserNotExist = $"User with UserId {request.UserId} doesn't exist."; 
    static const String kDateInvalid = "Date parameter format is invalid."; 
    static const String kTimeInvalid = "Time parameter format is invalid"; 
    static const String kDateLaterThanCurrentTime =  $"Date parameter value must be later than current time {currentTime}."; 
    static const String kValidTime = "Time parameter value must be later than 5AM and before 21PM."; 
    static const String kBikeKeerSamePerson = "Biker and Keer can't be the same person"; 
    static const String kBikerNotExist = "Biker doesn't exist"; 
    static const String kBikerNotHaveVerifiedBike = "Biker doesn't have verified bike yet"; 
    static const String kBikeNotExist = "Bike doesn't exist."; 
    static const String kCanceTriplRule = "Cancellation request must come from Keer or Biker of the trip."; 
    static const String kCancelTripFailed = $"Failed to cancel trip with TripId {request.TripId}."; 
    static const String kCreateScheduleTripFailedBooktimeOneHour = "Failed to create new trip schedule because bookTime " +
                                                    $"{request.TripCreationDto.BookTime} is less than {limitOneHourTime}."; 
    static const String kCreateTripFailedBooktimeFifteenMinutes = "Failed to create new trip now because bookTime " +
                                                    $"{request.TripCreationDto.BookTime} is larger than {limitFifteenMinutesTime}."; 
    static const String kCreateTripFailedBooktimeEarlierThanCurrenttime = "Failed to create new trip now because bookTime " +
                                                    $"{request.TripCreationDto.BookTime} is earlier than current time {currentTime}."; 
    static const String kRouteDepartAndDestiNotExist = $"Route with DepartureId {request.TripCreationDto.DepartureId} and " +
                            $"DestinationId {request.TripCreationDto.DestinationId} doesn't exist."; 
    static const String kBookTimeExisted = "Failed to create new trip because trip with bookTime " +
                                                    $"{request.TripCreationDto.BookTime} is already existed " +
                                                    $"for Keer with KeerId {request.TripCreationDto.KeerId}."; 
    static const String kReachMaxTrip =  $"Failed to create new trip schedule because exceeding max number of trips ({Constant.MaxTripCount})."; 
    static const String kCreateTripFailed = "Failed to create new trip."; 
    static const String kNoBikeAvailable = "There are no bikers available now."; 
    static const String kUserSentRequestNotExist = "User who sent request doesn't exist."; 
    static const String kTripByIDNotExist = $"Trip with {request.TripId} doesn't exist."; 
    static const String kUnauthorizeContent = $"User with UserId {request.UserRequestId} " +
                                                                 $"request an unauthorized content of trip with TripId {request.TripId}";
    static const String kBikeNotHaveBike = $"Biker does not have bike."; 
    static const String kFinishtripWithoutBiker = "Trip must has Biker before finishing"; 
    static const String kBikerFinishTripRule = "Only Biker of this trip can send request to finish this trip."; 
    static const String kUpdateTripFailed = $"Failed to update trip with TripId {request.TripId}. " +
                                                    (ex.InnerException?.Message ?? ex.Message); 
    static const String kCreateScheduleTripFailedBooktimeEmpty = "Failed to create new trip schedule because list bookTime is empty"; 
    static const String kRouteNotExist = $"Route with DepartureId {request.TripScheduleCreationDto.DepartureId} and " +
                            $"DestinationId {request.TripScheduleCreationDto.DestinationId} doesn't exist."; 
    static const String kDuplicateBookTime = "Failed to create new trip schedule because list bookTime has a duplicate bookTime."; 
    static const String kTripBooktimeExisted = "Failed to create new trip schedule because trip with bookTime " +
                                                    $"{request.TripScheduleCreationDto.BookTime!.First()} is already existed."; 
    static const String kReachMaxScheduleTrip = $"Failed to create new trip schedule because exceeding max number of trips ({Constant.MaxTripCount})."; 
    static const String kCreateMultipleTripScheduleFailed = "Failed to create multiple trips by schedule."; 
    static const String kStartTripBikerRule = "Trip must has Biker before starting."; 
    static const String kBikerNotBelongsToTrip = $"Biker with UserId {request.UserId} doesn't belong to this trip."; 
    static const String kStartTripFailedNoOneArrived = "Can not start trip because no one is arrived at waiting point yet"; 
    static const String kStartTripFaileOnePersonNotArrived = $"Can not start trip because {personRole} has not arrived at waiting point yet."; 

    static const String kUpdateTripFailed = $"Failed to update trip with TripId {request.TripId}."; 
    static const String kTriWaitingBikerRule = "Trip must has Biker before waiting."; 
    static const String kUserRequestEndpointOnce =   $"User with UserId {request.UserId} can only request this endpoint once."; 

    //User 
    //VanLNT
    static const String kUserVerifiedEmailAlready =   $"User with UserId {request.UserId} has already verified email.";
    static const String kUserVerifiedPhoneAlready =   $"User with UserId {request.UserId} has already verified phone."; 
    static const String kVerifiedUserFailed =   $"Failed to verify user with UserId {request.UserId}."; 
    static const String kUserDeleted =   $"User with userId {request.UserId} has been deleted. " +
                                                    "Please reactivate it to edit it this user."; 
    static const String kChangeRole2AdminLeft =   "Can not change role because there are only 2 admin left."; 
    static const String kDelete2AdminLeft =   "Can not delete this user because there are only 2 admin left."; 
    static const String kEditUserFalseFireBase =   "Error edit user's role on Firebase. " +
                                                    $"{e.InnerException?.Message ?? e.Message}"; 
    static const String kUpdateUserRoleFailed =   $"Failed to update user's role by userId {request.UserId} to {user.RoleId}."; 
    static const String kUserNotBiker =   $"User with UserId {request.UserId} is not a biker."; 
    static const String kNoBikeAvailable =   "There are no biker available."; 
    static const String kNoUserOnFireBase =   "Can't get user's uid to delete on Firebase " +
                                                    "because there are no user in database"; 
    static const String kDeleteAllUserErrorFireBase =   "Error delete all users on Firebase. " +
                                                    $"{e.InnerException?.Message ?? e.Message}"; 
    static const String kUserByIDNotExist =   $"User with UserId {request.UserId} doesn't exist."; 
    static const String kUpdateUserLoginDeviceFailed =   $"Failed to update user's login device by userId {request.UserId}."; 
    static const String kCreateNewAddressFailed =   "Failed to create new user address."; 
    static const String kAddressNotExist =   "Address doesn't exist."; 
    static const String kDeleteDefaultAddress =   "Can not delete default address."; 
    static const String kDeleteUserAddressFailed =    $"Failed to delete user address with UserAddressId {request.UserAddressId}."; 
    static const String kUserAddressNotExist =   "User address doesn't exist."; 
    static const String kUpdateUserAddressFailed =   $"Failed to update user address with UserAddressId {request.UserAddressId}."; 
    static const String kDuplicateEmailOrPhone =   "User with the same email or phone number has already existed."; 
    static const String kCreateUserFailed =   "Failed to create new user."; 
    static const String kDeleteUserFailed =   $"Failed to delete user with userId {request.UserId}."; 
    static const String kUpdateUserTripNowAvailabilityFailed =   $"Failed to update user's trip now availability by userId {request.UserId}."; 
    static const String kUserWithIfNotExist =   $"User with userId {auth.User.LocalId} doesn't exist."; 
    static const String kUpdateEmailVerificationFailed =   "Failed to update email verification."; 
    static const String kUserHasNotVerifiedEmail =   $"User with userId {auth.User.LocalId} hasn't verified email."; 
    static const String kUserNotAdmin =   $"User with userId {auth.User.LocalId} is not an admin."; 
    static const String kSaveLoginInfoFailed =   "Failed to save login information"; 
    static const String kUpdateUserProfileFailed =   $"Failed to update user's profile by userId {request.UserId}."; 
    static const String kBikerHasNotVerifiedBike =   "User's bike has not been verified."; 
    static const String kUserDoesNotHaveBike =   "User does not have bike."; 
    static const String kCreateConfigurationFailed = ""; 
    static const String kCreateConfigurationFailed = ""; 
    static const String kCreateConfigurationFailed = ""; 
    static const String kCreateConfigurationFailed = ""; 
    static const String kCreateConfigurationFailed = ""; 
    static const String kCreateConfigurationFailed = ""; 
    static const String kCreateConfigurationFailed = ""; 

}