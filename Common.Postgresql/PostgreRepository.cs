// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Repository.cs" company="Microsoft Corporation">
//   Copyright (c) 2020 Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Common.Postgresql
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Npgsql.Bulk;

    public class PostgreRepository<T> : IPostgreRepository<T> where T : class, new()
    {
        private readonly ILogger<PostgreRepository<T>> logger;
        private readonly DbContext context;
        private readonly DbSet<T> table;
        
        public PostgreRepository(ILoggerFactory loggerFactory, DbContext dbContext)
        {
            logger = loggerFactory.CreateLogger<PostgreRepository<T>>();
            context = dbContext;
            // EntityFrameworkManager.ContextFactory = context => dbContext;
            table = context.Set<T>();
        }
        
        public async Task<int> Ingest(IEnumerable<T> source, CancellationToken cancel)
        {
            var count = source.Count();
            logger.LogInformation($"adding {typeof(T).Name}...");
            var stopwatch = Stopwatch.StartNew();
            // foreach (var record in source)
            // {
            //     await table.AddAsync(record, cancel);    
            // }
            // await context.BulkInsertAsync(source, cancel);
            var uploader = new NpgsqlBulkUploader(context);
            await uploader.InsertAsync(source);
            logger.LogInformation($"it took {stopwatch.Elapsed} to injest {count} records");
            return count;
        }
    }
}