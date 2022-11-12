using LLS.Common.Enums;
using LLS.Common.Models.LLO.Pages.Blocks.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.Common.Models.LLO.Pages.Blocks
{
    public class Block
    {
        public Int64 index { get; set; }
        public BlockType Type { get; set; }
        public MainType MainType { get; set; }
        public Content.Content Content { get; set; }
        public Configs Config { get; set; }
        public Score Score { get; set; }
        public Score Answers { get; set; }
        public float StudentScore { get; set; }

    }
}
