using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArch.Core.Entities
{
    [Table("gift_codes")]
    public class GiftCode
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("type")]
        [Required]
        public byte Type { get; set; } // 0: For personal, 1: For everyone

        [Column("code")]
        [Required]
        [MaxLength(255)]
        public string Code { get; set; }

        [Column("gold")]
        [Required]
        public int Gold { get; set; } = 0;

        [Column("gem")]
        [Required]
        public int Gem { get; set; } = 0;

        [Column("ruby")]
        [Required]
        public int Ruby { get; set; } = 0;

        [Column("items")]
        public string Items { get; set; } // JSON string

        [Column("status")]
        [Required]
        public byte Status { get; set; } // 0: Inactive, 1: Active, 2: Expired

        [Column("active")]
        [Required]
        public int Active { get; set; } = 0; // 0: Không yêu cầu kích hoạt, 1: yêu cầu kích hoạt

        [Column("expires_at")]
        public DateTime? ExpiresAt { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}
