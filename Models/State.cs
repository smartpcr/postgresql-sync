﻿// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Microsoft Corporation">
// //   Copyright (c) 2017 Microsoft Corporation.  All rights reserved.
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------

namespace Models
{
    using System.ComponentModel;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    [DefaultValue(Missing)]
    public enum State
    {
        [Description("Missing")] Missing,
        [Description("NotApplicable")] NotApplicable,
        [Description("NormallyClosed")] NormallyClosed,
        [Description("NormallyOpen")] NormallyOpen,
        [Description("Source1")] Source1,
        [Description("Source2")] Source2,
        [Description("Spare")] Spare,
        [Description("StandBy")] StandBy
    }
}