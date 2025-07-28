using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArch.Core.Entities
{
    [Table("user_milestone_claims")]
    public class UserMilestoneClaim
    {
        [Column("user_id")]
        public long UserId { get; set; }

        [Column("milestone_id")]
        public int MilestoneId { get; set; }

        [Column("claimed_at")]
        [Required]
        public DateTime ClaimedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("MilestoneId")]
        public virtual MilestoneReward MilestoneReward { get; set; }
    }
}
