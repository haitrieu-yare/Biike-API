using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.MomoTransactions.DTOs
{
    public class MomoTransactionCheckDto
    {
        [Required] [JsonPropertyName("partnerCode")] public string? PartnerCode { get; set; }
        [Required] [JsonPropertyName("requestType")] public string? RequestType { get; set; }
        [Required] [JsonPropertyName("accessKey")] public string? AccessKey { get; set; }
        [Required] [JsonPropertyName("signature")] public string? Signature { get; set; }
        [Required] [JsonPropertyName("requestId")] public string? RequestId { get; set; }
        [Required] [JsonPropertyName("orderId")] public string? OrderId { get; set; }
    }
}