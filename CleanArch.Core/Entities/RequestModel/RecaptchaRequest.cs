using System.Text.Json.Serialization;

namespace CleanArch.Core.Entities.RequestModel
{
    public class EventData
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("expectedAction")]
        public string ExpectedAction { get; set; }

        [JsonPropertyName("siteKey")]
        public string SiteKey { get; set; }
    }

    public class RecaptchaRequest
    {
        [JsonPropertyName("event")]
        public EventData Event { get; set; }
    }
}