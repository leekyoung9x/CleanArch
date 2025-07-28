using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArch.Core.Entities
{
    [Table("reward_packages")]
    public class RewardPackage
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }

        // Navigation properties
        public virtual ICollection<RewardPackageContent> RewardPackageContents { get; set; }
        public virtual ICollection<MilestoneReward> MilestoneRewards { get; set; }
        public virtual ICollection<LeaderboardReward> LeaderboardRewards { get; set; }

        public RewardPackage()
        {
            RewardPackageContents = new HashSet<RewardPackageContent>();
            MilestoneRewards = new HashSet<MilestoneReward>();
            LeaderboardRewards = new HashSet<LeaderboardReward>();
        }
    }
}
