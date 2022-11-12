using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.Common.Models
{
    public class Resource_Machine
    {
        public Guid Id { get; set; }

        public Guid MachineId { get; set; }
        [ForeignKey(nameof(MachineId))]
        public Machine Machine { get; set; }

        public Guid ResourceId { get; set; }
        [ForeignKey(nameof(ResourceId))]
        public Resource Resource { get; set; }

    }
}
