using System.Text.Json.Serialization;

namespace Application.MomoTransactions.DTOs
{
    public class MomoTransactionCheckResultDto
    {
        [JsonPropertyName("partnerCode")] public string? PartnerCode { get; set; }
        [JsonPropertyName("requestType")] public string? RequestType { get; set; }
        [JsonPropertyName("accessKey")] public string? AccessKey { get; set; }
        [JsonPropertyName("signature")] public string? Signature { get; set; }
        [JsonPropertyName("requestId")] public string? RequestId { get; set; }
        [JsonPropertyName("orderId")] public string? OrderId { get; set; }
        [JsonPropertyName("extraData")] public string? ExtraData { get; set; }
        [JsonPropertyName("amount")] public string? Amount { get; set; }
        [JsonPropertyName("transId")] public string? TransId { get; set; }
        [JsonPropertyName("payType")] public string? PayType { get; set; }
        [JsonPropertyName("errorCode")] public int? ErrorCode { get; set; }
        [JsonPropertyName("message")] public string? Message { get; set; }
        [JsonPropertyName("localMessage")] public string? LocalMessage { get; set; }
    }
}