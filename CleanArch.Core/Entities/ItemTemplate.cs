using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArch.Core.Entities
{
    [Table("item_template")]
    public class ItemTemplate
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("TYPE")]
        [Required]
        public int Type { get; set; }

        [Column("gender")]
        [Required]
        public short Gender { get; set; }

        [Column("NAME")]
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Column("description")]
        [Required]
        [MaxLength(255)]
        public string Description { get; set; }

        [Column("icon_id")]
        [Required]
        public int IconId { get; set; }

        [Column("part")]
        [Required]
        public int Part { get; set; }

        [Column("is_up_to_up")]
        [Required]
        public bool IsUpToUp { get; set; }

        [Column("power_require")]
        [Required]
        public int PowerRequire { get; set; }

        // Navigation properties
        public virtual ICollection<ItemOptionTemplate> ItemOptionTemplates { get; set; }

        public ItemTemplate()
        {
            ItemOptionTemplates = new HashSet<ItemOptionTemplate>();
        }
    }
}
