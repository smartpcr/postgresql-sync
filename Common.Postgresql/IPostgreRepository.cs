// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepository.cs" company="Microsoft Corporation">
//   Copyright (c) 2020 Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Common.Postgresql
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IPostgreRepository<T> where T : class, new()
    {
        Task<int> Ingest(IEnumerable<T> source, CancellationToken cancel);
    }
}