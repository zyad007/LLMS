using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LLS.Common.Models
{
    public class Resource_Exp
    {
        public Guid Id { get; set; }

        public Guid Exp_CourseId { get; set; }
        [ForeignKey(nameof(Exp_CourseId))]
        public Exp_Course Exp_Course { get; set; }

        public Guid ResourceId { get; set; }
        [ForeignKey(nameof(ResourceId))]
        public Resource Resource { get; set; }
    }
}