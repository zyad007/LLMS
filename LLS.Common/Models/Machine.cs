using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.Common.Models
{
    public class Machine
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string IP { get; set; }

        public List<Resource_Machine> resource_machines { get; set; }
    }
}
