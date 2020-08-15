// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncKustoToPostgre.cs" company="Microsoft Corporation">
//   Copyright (c) 2020 Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tools.DataSync
{
    using System;
    using System.Collections.Generic;
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
        private readonly IDocDbRepository<DataCenter> dcSourceRepo;
        private readonly IPostgreRepository<PowerDevice> deviceTargetRepo;
        private readonly IPostgreRepository<DataCenter> dcTargetRepo;

        public SyncKustoToPostgre(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<SyncKustoToPostgre>();
            var docRepoFactory = serviceProvider.GetRequiredService<RepositoryFactory>();
            dcSourceRepo = docRepoFactory.CreateRepository<DataCenter>();
            deviceSourceRepo = docRepoFactory.CreateRepository<PowerDevice>();
            
            var postgreRepoFactory = serviceProvider.GetRequiredService<PostgreRepoFactory>();
            var dbContext = serviceProvider.GetRequiredService<MetaDataContext>();
            dcTargetRepo = postgreRepoFactory.CreateRepository<DataCenter>(dbContext);
            deviceTargetRepo = postgreRepoFactory.CreateRepository<PowerDevice>(dbContext);
        }
        
        public async Task ExecuteAsync(CancellationToken cancel)
        {
            // Func<IList<DataCenter>, CancellationToken, Task> onDataCentersReceived = async (list, ct) =>
            // {
            //     await dcTargetRepo.Ingest(list, IngestMode.Refresh, ct);
            // };
            // var totalDcSynced = await dcSourceRepo.QueryInBatches("", onDataCentersReceived, 1000, cancel);
            // logger.LogInformation($"total of {totalDcSynced} data centers synchronized");
            
            Func<IList<PowerDevice>, CancellationToken, Task> onDevicesReceived = async (list, ct) =>
            {
                await deviceTargetRepo.Ingest(list, IngestMode.Refresh, ct);
            };
            var totalDevicesSynced = await deviceSourceRepo.QueryInBatches("", onDevicesReceived, 1000, cancel);
            logger.LogInformation($"total of {totalDevicesSynced} devices synchronized");
        }
    }
}