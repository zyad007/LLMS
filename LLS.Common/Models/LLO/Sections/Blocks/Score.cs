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
        public List<Row> rows { get; set; }
        public List<DragText> dragText { get; set; }
        public List<ReOrder> reOrder { get; set; }
        public List<FillBlanks> fillBlanks { get; set; }
    }

    public class FillBlanks
    {
        public int idnex { get; set; }
        public string text { get; set; }
    }

    public class ReOrder
    {
        public int index { get; set; }
        public int orderId { get; set; }
    }

    public class DragText
    {
        public int index { get; set; }
        public int textId { get; set; }
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

    public class Row
    {
        public int rowId { get; set; }
        public int answerId { get; set; }
    }
}