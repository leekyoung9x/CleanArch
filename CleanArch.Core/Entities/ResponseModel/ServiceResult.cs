namespace CleanArch.Core.Entities.ResponseModel
{
    public class ServiceResult
    {
        public bool Status { get; set; }
        public string StatusMessage { get; set; } = string.Empty;
        public object? Data { get; set; }
    }
}