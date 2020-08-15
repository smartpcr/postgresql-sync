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
    using Common.Config;

    public interface IPostgreRepository<T> where T : BaseEntity, new()
    {
        Task<IEnumerable<T>> GetAll(CancellationToken cancel);
        Task<Dictionary<string, int>> CountBy(string propName, CancellationToken cancel);
        Task<IEnumerable<string>> GetKeys(CancellationToken cancel);
        Task<int> Ingest(IEnumerable<T> source, IngestMode ingestMode, CancellationToken cancel);
    }
}