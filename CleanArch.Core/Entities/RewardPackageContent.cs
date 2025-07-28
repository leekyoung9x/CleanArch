using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArch.Core.Entities
{
    [Table("reward_package_contents")]
    public class RewardPackageContent
    {
        [Column("package_id")]
        public int PackageId { get; set; }

        [Column("item_id")]
        public int ItemId { get; set; }

        [Column("quantity")]
        [Required]
        public int Quantity { get; set; }

        [Column("options")]
        [MaxLength(5000)]
        public string Options { get; set; }

        // Navigation properties
        [ForeignKey("PackageId")]
        public virtual RewardPackage RewardPackage { get; set; }
    }
}
