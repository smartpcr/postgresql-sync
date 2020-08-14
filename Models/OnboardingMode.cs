// // --------------------------------------------------------------------------------------------------------------------
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
    [DefaultValue(Hierarchy)]
    public enum OnboardingMode
    {
        [Description("Hierarchical Onboarding")]
        Hierarchy = 0,
        [Description("Zenon Onboarding")] Zenon = 1,
        [Description("Thermal Onboarding")] Thermal = 2,
        [Description("Mechanical Onboarding")] Mechanical = 3
    }
}