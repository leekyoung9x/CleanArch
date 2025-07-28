using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArch.Core.Entities
{
    [Table("milestone_rewards")]
    public class MilestoneReward
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("milestone_name")]
        [Required]
        [MaxLength(255)]
        public string MilestoneName { get; set; }

        [Column("required_score")]
        [Required]
        public long RequiredScore { get; set; }

        [Column("reward_package_id")]
        [Required]
        public int RewardPackageId { get; set; }

        // Navigation properties
        [ForeignKey("RewardPackageId")]
        public virtual RewardPackage RewardPackage { get; set; }
        
        public virtual ICollection<UserMilestoneClaim> UserMilestoneClaims { get; set; }

        public MilestoneReward()
        {
            UserMilestoneClaims = new HashSet<UserMilestoneClaim>();
        }
    }
}
