namespace CleanArch.Core.Entities.RequestModel
{
    public class BankRequest : BaseRequestModel
    {
        public int amount { get; set; }
        public string? otp { get; set; }
    }
}