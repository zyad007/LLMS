using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LLS.Common.Models
{
    public class Resource_Exp
    {
        public Guid Id { get; set; }

        public Guid ExperimentId { get; set; }
        [ForeignKey(nameof(ExperimentId))]
        public Experiment Experiment { get; set; }

        public Guid ResourceId { get; set; }
        [ForeignKey(nameof(ResourceId))]
        public Resource Resource { get; set; }
    }
}