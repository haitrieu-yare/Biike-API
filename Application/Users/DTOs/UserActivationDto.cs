using System.Text.Json.Serialization;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Application.Users.DTOs
{
    public class UserActivationDto
    {
        // TODO: Bỏ việc verify email qua api
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? IsEmailVerified { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? IsPhoneVerified { get; init; }

        public bool? IsVerified { get; set; }
    }
}