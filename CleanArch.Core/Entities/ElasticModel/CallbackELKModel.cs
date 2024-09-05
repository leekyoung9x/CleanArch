namespace CleanArch.Core.Entities.ElasticModel
{
    public class CallbackELKModel : BaseElasticModel
    {
        public int status { get; set; }
        public string? message { get; set; }
        public string? request_id { get; set; }
        public int? declared_value { get; set; }
        public int? value { get; set; }
        public int? amount { get; set; }
        public string? code { get; set; }
        public string? serial { get; set; }
        public string? telco { get; set; }
        public string? callback_sign { get; set; }
    }
}