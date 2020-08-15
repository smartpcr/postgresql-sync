// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PowerDevice.cs" company="Microsoft Corporation">
//   Copyright (c) 2020 Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class PowerDevice : BaseEntity
    {
        #region common props
        [Key, Required, MaxLength(255)]
        public string DeviceName { get; set; }
        [Required, MaxLength(50)]
        public string DcName { get; set; }
        public long DcCode { get; set; }
        public DeviceType DeviceType { get; set; }
        public State DeviceState { get; set; }
        public string Hierarchy { get; set; }
        public string PanelName { get; set; }
        public string ColoName { get; set; }
        #endregion

        #region relation props
        public string PrimaryParent { get; set; }
        public string SecondaryParent { get; set; }
        public string MaintenanceParent { get; set; }
        public string RedundantDeviceNames { get; set; }
        #endregion

        #region capacity props
        public double? Amperage { get; set; }
        public double? Voltage { get; set; }
        public double? RatedCapacity { get; set; }
        public double? DeRatedCapacity { get; set; }
        #endregion

        #region datatype props
        public string DataType { get; set; }
        public string ConfiguredObjectType { get; set; }
        public string DriverName { get; set; }
        public string ConnectionName { get; set; }
        public string IpAddress { get; set; }
        public string PortNumber { get; set; }
        public string NetAddress { get; set; }
        public bool IsMonitorable { get; set; }
        public string ProjectName { get; set; }

        #endregion

        #region power props
        public double? PowerFactor { get; set; }
        public double? DeRatingFactor { get; set; }
        public double? KwRating { get; set; }
        public double? KvaRating { get; set; }
        public int UnitId { get; set; }
        public double? ItCapacity { get; set; }
        public double? MaxitCapacity { get; set; }
        #endregion

        #region misc props
        public double Resistance { get; set; }
        public double? RippleCurrent { get; set; }
        public double Temperature { get; set; }
        public string FriendlyName { get; set; }
        public Tag Tags { get; set; }
        #endregion
    }
}