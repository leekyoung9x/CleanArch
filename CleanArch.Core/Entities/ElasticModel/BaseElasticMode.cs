namespace CleanArch.Core.Entities.ElasticModel
{
    public class BaseElasticModel
    {
         public string created_date
        {
            get
            {
                return DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
            }
        }
    }
}