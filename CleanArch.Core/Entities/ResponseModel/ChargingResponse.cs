using System.Text.Json.Serialization;

namespace CleanArch.Core.Entities.ResponseModel
{
    public class ChargingResponse
    {
        [JsonPropertyName("trans_id")]
        public int? TransId { get; set; }

        [JsonPropertyName("request_id")]
        public string RequestId { get; set; }

        [JsonPropertyName("amount")]
        public int? Amount { get; set; }

        [JsonPropertyName("value")]
        public int? Value { get; set; }

        [JsonPropertyName("declared_value")]
        public string DeclaredValue { get; set; }

        [JsonPropertyName("telco")]
        public string Telco { get; set; }

        [JsonPropertyName("serial")]
        public string Serial { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("status")]
        public int? Status { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}