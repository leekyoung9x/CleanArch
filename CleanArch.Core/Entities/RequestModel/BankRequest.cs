using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArch.Core.Entities.RequestModel
{
    public class BankRequest : BaseRequestModel
    {
        public int amount { get; set; }
        public string? otp { get; set; }
    }
}
