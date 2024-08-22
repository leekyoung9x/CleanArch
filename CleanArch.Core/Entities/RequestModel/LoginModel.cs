namespace CleanArch.Core.Entities.RequestModel
{
    public class LoginModel : BaseRequestModel
    {
        public string? username { get; set; }
        public string? password { get; set; }
    }
}