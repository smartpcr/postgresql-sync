// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncKustoToPostgre.cs" company="Microsoft Corporation">
//   Copyright (c) 2020 Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tools.DataSync
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Config;
    using Common.Postgresql;
    using Common.Repositories;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Models;

    public class SyncKustoToPostgre : IExecutor
    {
        private readonly ILogger<SyncKustoToPostgre> logger;
        private readonly IDocDbRepository<PowerDevice> deviceSourceRepo;
        private readonly IPostgreRepository<PowerDevice> deviceTargetRepo;

        public SyncKustoToPostgre(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<SyncKustoToPostgre>();
            var docRepoFactory = serviceProvider.GetRequiredService<RepositoryFactory>();
            deviceSourceRepo = docRepoFactory.CreateRepository<PowerDevice>();
            var postgreRepoFactory = serviceProvider.GetRequiredService<PostgreRepoFactory>();
            var dbContext = serviceProvider.GetRequiredService<MetaDataContext>();
            
            deviceTargetRepo = postgreRepoFactory.CreateRepository<PowerDevice>(dbContext);
        }
        
        public async Task ExecuteAsync(CancellationToken cancel)
        {
            var devices = await deviceSourceRepo.Query("c.dcName='AMS05'");
            await deviceTargetRepo.Ingest(devices, cancel);
        }
    }
}