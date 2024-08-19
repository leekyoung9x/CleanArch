using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArch.Core.Entities.PagingData
{
    public class PagingData<T> where T : class
    {
        public IEnumerable<T> Data { get; set; }
        public int TotalRecord { get; set; }
    }
}
