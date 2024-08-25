using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArch.Core.Entities
{
    [Table("transaction_card")]
    public class transaction_card
    {
        [Key]
        [Column("id")]
        public int id { get; set; }

        [Column("player_id")]
        public int player_id { get; set; }

        [Column("request_id")]
        public string request_id { get; set; }

        [Column("status")]
        public int status { get; set; }

        [Column("amount")]
        public int amount { get; set; }

        [Column("amount_real")]
        public int amount_real { get; set; }

        [Column("seri")]
        public string seri { get; set; }

        [Column("code")]
        public string code { get; set; }

        [Column("card_type")]
        public string card_type { get; set; }

        [Column("time")]
        public DateTime time { get; set; }
    }
}