// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataCenter.cs" company="Microsoft Corporation">
//   Copyright (c) 2020 Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class DataCenter : BaseEntity
    {
        [Key, Required, MaxLength(50)]
        public string DcName { get; set; }
        [Index] public string DcLongName { get; set; }
        [Index] public long DcCode { get; set; }
        public string Region { get; set; }
        public string CampusName { get; set; }
        [Index] public string DcGeneration { get; set; }
        [Index] public string FormFactorName { get; set; }
        public string Owner { get; set; }
        public string ClassName { get; set; }
        public string PhaseName { get; set; }
        public string CoolingType { get; set; }
        [MaxLength(1000)]
        public string HVACType { get; set; }
        public long MSAssetId { get; set; }
    }
}