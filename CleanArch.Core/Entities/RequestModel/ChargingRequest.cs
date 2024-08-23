namespace CleanArch.Core.Entities.RequestModel
{
    public class ChargingRequest
    {
        public string Telco { get; set; }
        public string Code { get; set; }
        public string Serial { get; set; }
        public string Amount { get; set; }
        public string? RequestId { get; set; }
        public string? PartnerId { get; set; }
        public string? Sign { get; set; }
        public string Command { get; set; }
    }
}
