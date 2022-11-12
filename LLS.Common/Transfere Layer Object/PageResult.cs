using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.Common.Transfere_Layer_Object
{
    public class PageResult : Result
    {
        public int Page { get; set; }
        public int ResultCount { get; set; }
        public int ResultPerPage { get; set; }
    }
}
