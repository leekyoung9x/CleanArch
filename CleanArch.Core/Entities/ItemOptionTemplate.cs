using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArch.Core.Entities
{
    [Table("item_option_template")]
    public class ItemOptionTemplate
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("NAME")]
        [MaxLength(255)]
        public string Name { get; set; }

        [Column("TYPE")]
        [Required]
        public int Type { get; set; }
    }
}
