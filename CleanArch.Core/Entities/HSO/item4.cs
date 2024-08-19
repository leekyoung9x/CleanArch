using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    [Table("item4")]
    public class item4
    {
        [Key]
        [Column("id")]
        public int id { get; set; }
        [Column("icon")]
        public int icon { get; set; }
        [Column("price")]
        public long price { get; set; }
        [Column("name")]
        public string name { get; set; }
        [Column("content")]
        public string content { get; set; }
        [Column("typepotion")]
        public int typepotion { get; set; }
        [Column("moneytype")]
        public int moneytype { get; set; }
        [Column("sell")]
        public int sell { get; set; }
        [Column("value")]
        public int value { get; set; }
        [Column("canTrade")]
        public int canTrade { get; set; }
    }
}
