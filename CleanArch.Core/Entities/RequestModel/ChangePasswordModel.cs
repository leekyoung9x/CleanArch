namespace CleanArch.Core.Entities.RequestModel
{
    public class ChangePasswordModel : BaseRequestModel
    {
        public string? password { get; set; }
        public string? passwordnew { get; set; }
    }
}