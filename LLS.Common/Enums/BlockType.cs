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
        OPEN_QUESTION,
        MULTIPLE_CHOICE,
        SINGLE_CHOICE,
        TRUE_OR_FALSE,
        IMAGE,
        MEDIA,
        DOCUMENT,
        EQUATION
    }
}
