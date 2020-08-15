// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IngestMode.cs" company="Microsoft Corporation">
//   Copyright (c) 2020 Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Common.Config
{
    public enum IngestMode
    {
        AppendOnly,
        InsertNew,
        Refresh
    }
}