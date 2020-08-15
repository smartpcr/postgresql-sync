// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Repository.cs" company="Microsoft Corporation">
//   Copyright (c) 2020 Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Common.Postgresql
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Config;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.Extensions.Logging;
    using Npgsql.Bulk;

    public class PostgreRepository<T> : IPostgreRepository<T> where T : BaseEntity, new()
    {
        private readonly ILogger<PostgreRepository<T>> logger;
        private readonly DbContext context;
        private readonly DbSet<T> table;
        private readonly IProperty pk;
        private readonly string schemaName;
        
        public PostgreRepository(ILoggerFactory loggerFactory, DbContext dbContext)
        {
            logger = loggerFactory.CreateLogger<PostgreRepository<T>>();
            context = dbContext;
            table = context.Set<T>();
            pk = GetPkProp();
            schemaName = GetSchemaName();
        }

        public async Task<IEnumerable<T>> GetAll(CancellationToken cancel)
        {
            return await table.AsQueryable().ToListAsync(cancel);
        }

        public async Task<Dictionary<string, int>> CountBy(string propName, CancellationToken cancel)
        {
            var output = new Dictionary<string, int>();
            var entityType = context.Model.FindEntityType(typeof(T));
            if (entityType == null)
            {
                throw new InvalidOperationException($"unable to find entity by type {typeof(T).Name}");
            }
            
            var prop = entityType?.GetProperties().FirstOrDefault(p => p.PropertyInfo.Name == propName);
            if (prop == null)
            {
                throw new InvalidOperationException($"unable to find property {propName} for type {typeof(T).Name}");
            }
            
            var query =
                $"select {prop.GetColumnName()}, Count = Count() from {schemaName}.{entityType.GetTableName()} group by {prop.GetColumnName()}";
            var connection = context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancel);
            }
            
            var cmd = connection.CreateCommand();
            cmd.CommandText = query;
            var reader = await cmd.ExecuteReaderAsync(cancel);
            while (await reader.ReadAsync(cancel))
            {
                var field = reader.GetString(0);
                var count = reader.GetInt32(1);
                output.Add(field, count);
            }
            await reader.CloseAsync();
            
            logger.LogInformation($"total of {output.Count} distinct records retrieved by field {propName}");
            return output;
        }

        public async Task<IEnumerable<string>> GetKeys(CancellationToken cancel)
        {
            var entityType = context.Model.FindEntityType(typeof(T));
            if (pk == null) return null;
            var colName = pk.GetColumnName();
            var query = $"select {colName} from {schemaName}.{entityType.GetTableName()}";
            var keys = new List<string>();
            var connection = context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancel);
            }
            
            var cmd = connection.CreateCommand();
            cmd.CommandText = query;
            var reader = await cmd.ExecuteReaderAsync(cancel);
            while (await reader.ReadAsync(cancel))
            {
                var key = reader.GetString(0);
                keys.Add(key);
            }
            await reader.CloseAsync();
            
            return keys;
        }

        public async Task<int> Ingest(IEnumerable<T> source, IngestMode ingestMode, CancellationToken cancel)
        {
            var list = source.GroupBy(s => pk.PropertyInfo.GetValue(s).ToString()).Select(g => g.First()).ToList();
            var count = list.Count();
            logger.LogInformation($"adding {typeof(T).Name}...");
            var stopwatch = Stopwatch.StartNew();
            
            var uploader = new NpgsqlBulkUploader(context);
            await uploader.InsertAsync(list);
            logger.LogInformation($"it took {stopwatch.Elapsed} to injest {count} records");
            return count;
        }

        private IProperty GetPkProp()
        {
            var entityType = context.Model.FindEntityType(typeof(T));
            var pkProp = entityType?.GetProperties().FirstOrDefault(p => p.IsPrimaryKey());
            return pkProp;
        }

        private string GetSchemaName()
        {
            var entityType = context.Model.FindEntityType(typeof(T));
            var schemaAnnotation = entityType?.GetAnnotations()?.FirstOrDefault(a => a.Name == "Relational:Schema");
            return schemaAnnotation == null ? "public" : schemaAnnotation.Value.ToString();
        }
    }
}