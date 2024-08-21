using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArch.Core.Entities.RequestModel
{
    public class ChangePasswordModel
    {
        public string password { get; set; }
        public string passwordnew { get; set; }
        public string token { get; set; }
    }
}
