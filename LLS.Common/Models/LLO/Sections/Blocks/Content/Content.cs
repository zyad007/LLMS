using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.Common.Models.LLO.Pages.Blocks.Content
{
    public class Content
    {
        public string title { get; set; }
        public string text { get; set; }
        public string instructions { get; set; }
        public List<Choices.ChoiceContent> choices { get; set; }
        public List<Statements.StatementContent> statements { get; set; }
        public string fileUrl { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }
}
