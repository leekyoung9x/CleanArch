namespace CleanArch.Core.Entities.ResponseModel
{
    public class TransactionCardResponse
    {
        public string request_id { get; set; }
        public int status { get; set; }

        public int amount { get; set; }

        public int amount_real { get; set; }

        public string seri { get; set; }

        public string code { get; set; }

        public string card_type { get; set; }

        public DateTime time { get; set; }
    }
}
