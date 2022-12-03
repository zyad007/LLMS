﻿using LLS.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.Common.Dto
{
    public class Role
    {
        public string Name { get; set; }

        public List<Permission> Permissions { get; set; }
    }
}
