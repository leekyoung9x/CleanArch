using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArch.Core.Entities.RequestModel
{
    public class send_item
    {
        public int player_id { get; set; }
        public int item_id { get; set; }
        public int quantity { get; set; }
        public int option_quantity { get; set; }
        public List<item_shop_option> options { get; set; }
    }
}
