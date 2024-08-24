using System.Text.Json.Serialization;

namespace CleanArch.Core.Entities.ResponseModel
{
    public class Transaction
    {
        [JsonPropertyName("transactionID")]
        public int TransactionID { get; set; }

        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("transactionDate")]
        public string TransactionDate { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }

    public class TransactionResponse
    {
        [JsonPropertyName("status")]
        public bool Status { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("transactions")]
        public List<Transaction> Transactions { get; set; }
    }
}