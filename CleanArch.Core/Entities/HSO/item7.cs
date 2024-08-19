using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    [Table("item7")]
    public class item7
    {
        [Key]
        [Column("id")]
        public int id { get; set; }
        [Column("imgid")]
        public int imgid { get; set; }
        [Column("price")]
        public long price { get; set; }
        [Column("name")]
        public string name { get; set; }
        [Column("content")]
        public string content { get; set; }
        [Column("type")]
        public int type { get; set; }
        [Column("pricetype")]
        public int pricetype { get; set; }
        [Column("sell")]
        public int sell { get; set; }
        [Column("value")]
        public int value { get; set; }
        [Column("trade")]
        public int trade { get; set; }
        [Column("setcolorname")]
        public int setcolorname { get; set; }
    }
}
