// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeviceType.cs" company="Microsoft Corporation">
//   Copyright (c) 2020 Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DeviceType
    {
        Unknown,
        Breaker,
        Transformer,
        Switch,
        Generator,
        STS,
        UPS,
        AHU,
        ATS,
        Busbar,
        DistributionPanel,
        Panel,
        PowerMeter,
        RPP,
        PDU,
        Heat,
        Condenser,
        DOAS,
        Filter,
        Heater,
        Humidifier,
        LoadBank,
        Pump,
        SurgeProtectiveDevice,
        VFD,
        HRG,
        Feed,
        Zenon,
        End,
        Busway,
        TieBreaker,
        GenBreaker,
        BMS,
        BMS_JAR,
        BMS_STRING,
        FuelPolisher,
        FuelFill,
        ParallelPanel
    }
}