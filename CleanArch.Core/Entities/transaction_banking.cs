using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArch.Core.Entities
{
    [Table("transaction_banking")]
    public class transaction_banking
    {
        [Key]
        [Column("id")]
        public long id { get; set; }

        [Column("player_id")]
        public long player_id { get; set; }

        [Column("amount")]
        public long amount { get; set; }

        [Column("description")]
        public string? description { get; set; }

        [Column("status")]
        public bool status { get; set; }

        [Column("is_recieve")]
        public bool is_recieve { get; set; }

        [Column("last_time_check")]
        public DateTime last_time_check { get; set; }

        [Column("created_date")]
        public DateTime created_date { get; set; }
    }
}