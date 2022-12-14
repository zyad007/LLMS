using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LLS.Common.Models.LLO.Pages.Blocks
{
    public class Score
    {
        public string Answer { get; set; }
        public float score { get; set; }
        public List<ChoiceScore> choices { get; set; }
        public List<Statement> statements { get; set; }
    }

    public class ChoiceScore
    {
        public int ChoiceId { get; set; }
        public float score { get; set; }
    }

    public class Statement
    {
        public int statementId { get; set; }
        public float score { get; set; }
        public bool answer { get; set; }
    }
}