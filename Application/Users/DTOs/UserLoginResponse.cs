// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Application.Users.DTOs
{
    public class UserLoginResponse
    {
        public string? UserId { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? DisplayName { get; set; }
        public string? ProfilePicture { get; set; }
        public string? IdToken { get; set; }
        public string? RefreshToken { get; set; }
        public string? ExpiresIn { get; set; }
        public bool? IsPhoneVerified { get; set; }
    }
}