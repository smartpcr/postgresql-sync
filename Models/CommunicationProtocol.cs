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
    [DefaultValue(None)]
    public enum CommunicationProtocol
    {
        None = 0,
        [Description("Modbus")] Modbus = 1,
        [Description("BACnet")] BACnet = 2
    }
}