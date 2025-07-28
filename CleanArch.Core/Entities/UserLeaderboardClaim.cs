using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArch.Core.Entities
{
    [Table("user_leaderboard_claims")]
    public class UserLeaderboardClaim
    {
        [Column("user_id")]
        public long UserId { get; set; }

        [Column("season_id")]
        public int SeasonId { get; set; }

        [Column("final_rank")]
        [Required]
        public int FinalRank { get; set; }

        [Column("claimed_at")]
        [Required]
        public DateTime ClaimedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("SeasonId")]
        public virtual LeaderboardSeason LeaderboardSeason { get; set; }
    }
}
