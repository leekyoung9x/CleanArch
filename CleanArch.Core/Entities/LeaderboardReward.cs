using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArch.Core.Entities
{
    [Table("leaderboard_rewards")]
    public class LeaderboardReward
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("season_id")]
        [Required]
        public int SeasonId { get; set; }

        [Column("rank_start")]
        [Required]
        public int RankStart { get; set; }

        [Column("rank_end")]
        [Required]
        public int RankEnd { get; set; }

        [Column("reward_package_id")]
        [Required]
        public int RewardPackageId { get; set; }

        // Navigation properties
        [ForeignKey("SeasonId")]
        public virtual LeaderboardSeason LeaderboardSeason { get; set; }

        [ForeignKey("RewardPackageId")]
        public virtual RewardPackage RewardPackage { get; set; }

        public virtual ICollection<UserLeaderboardClaim> UserLeaderboardClaims { get; set; }

        public LeaderboardReward()
        {
            UserLeaderboardClaims = new HashSet<UserLeaderboardClaim>();
        }
    }
}
