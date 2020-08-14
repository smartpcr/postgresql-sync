// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Microsoft Corporation">
// //   Copyright (c) 2017 Microsoft Corporation.  All rights reserved.
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------

namespace Models
{
    using System;
    using System.ComponentModel;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    [Flags]
    public enum Tag
    {
        None = 0,
        [Description("Azure Flex")] Flex = 1,
        [Description("PUE")] PUE = 2,
        [Description("IDF")] IDF = 4
    }
}