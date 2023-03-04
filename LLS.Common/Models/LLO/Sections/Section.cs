using LLS.Common.Dto;
using LLS.Common.Models.LLO.Pages.Blocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.Common.Models.LLO.Pages
{
    public class Section
    {
        public int index { get; set; }
        public string title { get; set; }
        public config config { get; set; }
        public VRL vrl { get; set; }
        public List<Block> Column1 { get; set; }
        public List<Block> Column2 { get; set; }
        public List<Block> Column3 { get; set; }
    }

    public class VRL
    {
        public bool active { get; set; }
        public string layout { get; set; }
        public bool showScreenShotButton { get; set; }
        //public List<ResourceDto> resources { get; set; }
        public string resources { get; set; }
    }

    public class config
    {
        public int numberOfColumns { get; set; }
        public string help { get; set; }
        public Int64? answeringTime { get; set; }
        public bool showCountdown { get; set; }
    }
}
