// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackendData.cs" company="Microsoft Corporation">
//   Copyright (c) 2020 Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Common.Repositories
{
    using System;
    using Common.DocDb;
    using Models;

    public class BackendData
    {
        [MappedModel(typeof(DataCenter))] public EntityStoreSettings DataCenter { get; set; }

        [MappedModel(typeof(PowerDevice))] public EntityStoreSettings Device { get; set; }

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class MappedModelAttribute : Attribute
    {
        public MappedModelAttribute(Type modelType)
        {
            ModelType = modelType;
        }

        public Type ModelType { get; set; }
    }
}