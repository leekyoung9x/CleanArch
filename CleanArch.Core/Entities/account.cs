using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArch.Core.Entities
{
    [Table("account")]
    public class account
    {
        [Key]
        [Column("id")]
        public int id { get; set; }

        [Column("username")]
        public string username { get; set; }

        [Column("password")]
        public string password { get; set; }

        [Column("create_time")]
        public object create_time { get; set; }

        [Column("update_time")]
        public object update_time { get; set; }

        [Column("ban")]
        public object ban { get; set; }

        [Column("point_post")]
        public int point_post { get; set; }

        [Column("last_post")]
        public int last_post { get; set; }

        [Column("role")]
        public int role { get; set; }

        [Column("is_admin")]
        public int is_admin { get; set; }

        [Column("last_time_login")]
        public object last_time_login { get; set; }

        [Column("last_time_logout")]
        public object last_time_logout { get; set; }

        [Column("ip_address")]
        public string ip_address { get; set; }

        [Column("active")]
        public int active { get; set; }

        [Column("reward")]
        public string reward { get; set; }

        [Column("thoi_vang")]
        public int thoi_vang { get; set; }

        [Column("server_login")]
        public int server_login { get; set; }

        [Column("new_reg")]
        public int new_reg { get; set; }

        [Column("ip")]
        public string ip { get; set; }

        [Column("phone")]
        public string phone { get; set; }

        [Column("last_server_change_time")]
        public object last_server_change_time { get; set; }

        [Column("ruby")]
        public int ruby { get; set; }

        [Column("count_card")]
        public int count_card { get; set; }

        [Column("type_bonus")]
        public int type_bonus { get; set; }

        [Column("ref")]
        public string ref_account { get; set; }

        [Column("diemgioithieu")]
        public int diemgioithieu { get; set; }

        [Column("vnd_old")]
        public int vnd_old { get; set; }

        [Column("tongnap_old")]
        public int tongnap_old { get; set; }

        [Column("gioithieu")]
        public int gioithieu { get; set; }

        [Column("tongnapcu")]
        public int tongnapcu { get; set; }

        [Column("account_old")]
        public int account_old { get; set; }

        [Column("pointNap")]
        public int pointNap { get; set; }

        [Column("vnd")]
        public int vnd { get; set; }

        [Column("tongnap")]
        public int tongnap { get; set; }
    }
}