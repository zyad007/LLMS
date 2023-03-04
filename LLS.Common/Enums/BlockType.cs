using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LLS.Common.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum BlockType
    {
        TEXT,
        CODE,
        OPEN_QUESTION_BLOCK,
        MULTI_SELECT_MCQ_BLOCK,
        SINGLE_SELECT_MCQ_BLOCK,
        TRUE_OR_FALSE_QUESTION_BLOCK,
        MATCH,
        FILL_BLANKS,
        REORDER,
        DRAG_TEXT,
        IMAGE,
        MEDIA,
        DOCUMENT,
        EQUATION,
        FRAME
    }
}
