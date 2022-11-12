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
        TEXT ,
        CODE ,
        OPEN_QUESTION ,
        MULTI_SELECT_MCQ ,
        SINGLE_SELECT_MCQ ,
        ONSPOT ,
        SECTION_TITLE ,
        IFRAME ,
        N_TUPLE_SPLIT,
        TRUE_OR_FALSE_QUESTION ,
        VRL,
        SINGLE_SELECT_IMAGE_MCQ,
        MULTI_SELECT_IMAGE_MCQ,
        MEDIA
    }
}
