// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostgreRepoFactory.cs" company="Microsoft Corporation">
//   Copyright (c) 2020 Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Common.Postgresql
{
    using System;
    using System.Collections.Concurrent;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    public class PostgreRepoFactory
    {
        private readonly ConcurrentDictionary<string, object>
            _repositories = new ConcurrentDictionary<string, object>();

        private readonly ILogger<PostgreRepoFactory> logger;
        private readonly ILoggerFactory loggerFactory;
        private readonly IServiceProvider serviceProvider;

        public PostgreRepoFactory(
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
            this.serviceProvider = serviceProvider;
            logger = this.loggerFactory.CreateLogger<PostgreRepoFactory>();
        }
        
        public IPostgreRepository<T> CreateRepository<T>(DbContext context) where T : class, new()
        {
            if (_repositories.TryGetValue(typeof(T).Name, out var found) && found is IPostgreRepository<T> repo) return repo;

            logger.LogInformation($"Creating doc db repo for type: {typeof(T).Name}");
            IPostgreRepository<T> repository = new PostgreRepository<T>(loggerFactory, context);
            _repositories.AddOrUpdate(typeof(T).Name, repository, (k, v) => repository);
            return repository;
        }
    }
}