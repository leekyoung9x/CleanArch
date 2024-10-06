using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArch.Core.Entities
{
    [Table("post")]
    public class Post
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_user")]
        public int IdUser { get; set; }

        [Column("tieude")]
        public string Tieude { get; set; }

        [Column("noidung")]
        public string Noidung { get; set; }

        [Column("ngaytao")]
        public DateTime Ngaytao { get; set; }

        [Column("id_chuyenmuc")]
        public int? IdChuyenmuc { get; set; }

        [Column("luotxem")]
        public int Luotxem { get; set; }

        [Column("ghim")]
        public int Ghim { get; set; }
    }
}