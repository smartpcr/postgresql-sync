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
    using Common.Config;
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
            table = context.Set<T>();
        }
        
        public async Task<int> Ingest(IEnumerable<T> source, IngestMode ingestMode, CancellationToken cancel)
        {
            var count = source.Count();
            logger.LogInformation($"adding {typeof(T).Name}...");
            var stopwatch = Stopwatch.StartNew();
            
            var uploader = new NpgsqlBulkUploader(context);
            await uploader.InsertAsync(source);
            logger.LogInformation($"it took {stopwatch.Elapsed} to injest {count} records");
            return count;
        }

        private IEnumerable<string> GetIds(CancellationToken cancel)
        {
            var entityType = context.Model.FindEntityType(typeof(T));
            var pkProp = entityType?.GetProperties().FirstOrDefault(p => p.IsPrimaryKey());
            if (pkProp == null) return null;
            var colName = pkProp.GetColumnName();
            var query = $"select {colName} from {entityType.GetSchema()}.{entityType.GetTableName()}";
            var entities = table.FromSqlRaw(query).ToList();
            var ids = entities.Select(e => pkProp.PropertyInfo.GetValue(e)?.ToString()).Where(s => s != null).ToList();
            logger.LogInformation($"total of {ids.Count} ids found");
            return ids;
        }
    }
}