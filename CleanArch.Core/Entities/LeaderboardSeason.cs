using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArch.Core.Entities
{
    [Table("leaderboard_seasons")]
    public class LeaderboardSeason
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("season_name")]
        [Required]
        [MaxLength(255)]
        public string SeasonName { get; set; }

        [Column("start_time")]
        [Required]
        public DateTime StartTime { get; set; }

        [Column("end_time")]
        [Required]
        public DateTime EndTime { get; set; }

        [Column("status")]
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "upcoming"; // upcoming, active, ended

        // Navigation properties
        public virtual ICollection<LeaderboardReward> LeaderboardRewards { get; set; }
        public virtual ICollection<UserLeaderboardClaim> UserLeaderboardClaims { get; set; }

        public LeaderboardSeason()
        {
            LeaderboardRewards = new HashSet<LeaderboardReward>();
            UserLeaderboardClaims = new HashSet<UserLeaderboardClaim>();
        }
    }
}
