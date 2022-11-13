using LLS.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.Common.Models.LLO.Pages.Blocks.Config
{
    public class Configs
    {
        public Grading? Grading { get; set; }
        public int? AnsweringTime { get; set; }
        public int? NumberOfTrials { get; set; }
        public bool? ShowCorrectAnswer { get; set; }
        public bool? ShowCountdown { get; set; }
        public bool? TimeState { get; set; }

    }
}
