using System.Text.Json.Serialization;

namespace CleanArch.Core.Entities.ResponseModel
{
    public class Event
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("siteKey")]
        public string SiteKey { get; set; }

        [JsonPropertyName("userAgent")]
        public string UserAgent { get; set; }

        [JsonPropertyName("userIpAddress")]
        public string UserIpAddress { get; set; }

        [JsonPropertyName("expectedAction")]
        public string ExpectedAction { get; set; }

        [JsonPropertyName("hashedAccountId")]
        public string HashedAccountId { get; set; }

        [JsonPropertyName("express")]
        public bool Express { get; set; }

        [JsonPropertyName("requestedUri")]
        public string RequestedUri { get; set; }

        [JsonPropertyName("wafTokenAssessment")]
        public bool WafTokenAssessment { get; set; }

        [JsonPropertyName("ja3")]
        public string Ja3 { get; set; }

        [JsonPropertyName("headers")]
        public List<string> Headers { get; set; }

        [JsonPropertyName("firewallPolicyEvaluation")]
        public bool FirewallPolicyEvaluation { get; set; }

        [JsonPropertyName("fraudPrevention")]
        public string FraudPrevention { get; set; }
    }

    public class RiskAnalysis
    {
        [JsonPropertyName("score")]
        public float Score { get; set; }

        [JsonPropertyName("reasons")]
        public List<string> Reasons { get; set; }

        [JsonPropertyName("extendedVerdictReasons")]
        public List<string> ExtendedVerdictReasons { get; set; }
    }

    public class TokenProperties
    {
        [JsonPropertyName("valid")]
        public bool Valid { get; set; }

        [JsonPropertyName("invalidReason")]
        public string InvalidReason { get; set; }

        [JsonPropertyName("hostname")]
        public string Hostname { get; set; }

        [JsonPropertyName("androidPackageName")]
        public string AndroidPackageName { get; set; }

        [JsonPropertyName("iosBundleId")]
        public string IosBundleId { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("createTime")]
        public DateTime CreateTime { get; set; }
    }

    public class AccountDefenderAssessment
    {
        [JsonPropertyName("labels")]
        public List<string> Labels { get; set; }
    }

    public class ResponseCaptcha
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("event")]
        public Event Event { get; set; }

        [JsonPropertyName("riskAnalysis")]
        public RiskAnalysis RiskAnalysis { get; set; }

        [JsonPropertyName("tokenProperties")]
        public TokenProperties TokenProperties { get; set; }

        [JsonPropertyName("accountDefenderAssessment")]
        public AccountDefenderAssessment AccountDefenderAssessment { get; set; }
    }
}