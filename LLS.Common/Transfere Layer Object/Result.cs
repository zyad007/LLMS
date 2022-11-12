﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.Common.Transfere_Layer_Object
{
    public class Result
    {
        public object Data { get; set; }
        public object Message { get; set; }
        public bool Status { get; set; }
        public DateTime ResponceTime { get; set; } = DateTime.Now;
    }
}
