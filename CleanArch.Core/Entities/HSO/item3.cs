using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    [Table("item3")]
    public class item3
    {
        [Key]
        [Column("id")]
        public int id { get; set; }
        [Column("name")]
        public string name { get; set; }
        [Column("type")]
        public int type { get; set; }
        [Column("part")]
        public int part { get; set; }
        [Column("clazz")]
        public int clazz { get; set; }
        [Column("iconid")]
        public int iconid { get; set; }
        [Column("level")]
        public int level { get; set; }
        [Column("data")]
        public string data { get; set; }
        [Column("color")]
        public int color { get; set; }
    }
}
