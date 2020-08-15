// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseEntity.cs" company="Microsoft Corporation">
//   Copyright (c) 2020 Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Common.Config
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public abstract class BaseEntity
    {
        protected BaseEntity()
        {
            TS = DateTime.UtcNow; // this will always be overwritten by docdb
        }

        [JsonProperty("id"), StringLength(36)] public string Id { get; set; }

        [JsonProperty(PropertyName = "_ts")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime TS { get; set; }

        public abstract string GetKeyValue();
    }
}