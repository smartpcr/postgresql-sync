// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncKustoToPostgre.cs" company="Microsoft Corporation">
//   Copyright (c) 2020 Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tools.DataSync
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Config;
    using Common.Postgresql;
    using Common.Repositories;
    using Microsoft.Azure.Documents.SystemFunctions;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Models;
    using Npgsql;

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
            // data center 
            var existingDcNames = (await dcTargetRepo.GetKeys(cancel)).ToList();
            logger.LogInformation($"total of {existingDcNames.Count} dc already exist in target db");
            
            Func<IList<DataCenter>, CancellationToken, Task> onDataCentersReceived = async (list, ct) =>
            {
                var newDcRecords = list.Where(item => !existingDcNames.Contains(item.GetKeyValue())).ToList();
                await dcTargetRepo.Ingest(newDcRecords, IngestMode.Refresh, ct);
            };
            var totalDcSynced = await dcSourceRepo.QueryInBatches("", onDataCentersReceived, 1000, cancel);
            logger.LogInformation($"total of {totalDcSynced} data centers synchronized");

            var allDCs = await dcTargetRepo.GetAll(cancel);
            logger.LogInformation($"total of {allDCs.Count()} data centers found in target db");

            // devices
            var existingDeviceNames = (await deviceTargetRepo.GetKeys(cancel)).ToList();
            logger.LogInformation($"total of {existingDeviceNames.Count} devices already exist in target db");
            
            Func<IList<PowerDevice>, CancellationToken, Task> onDevicesReceived = async (list, ct) =>
            {
                var newRecords = list.Where(item => !existingDeviceNames.Contains(item.GetKeyValue())).ToList();
                await deviceTargetRepo.Ingest(newRecords, IngestMode.Refresh, ct);
            };
            
            var totalSynced = 0;
            foreach (var dc in allDCs)
            {
                await RetryBlock.RetryOnThrottling(3, TimeSpan.FromSeconds(1), async () =>
                {
                    var deviceQuery = $"c.dcName='{dc.DcName}'";
                    var devicesSynced = await deviceSourceRepo.QueryInBatches(deviceQuery, onDevicesReceived, 5000, cancel);
                    totalSynced += devicesSynced;
                    logger.LogInformation($"Synced {devicesSynced} devices for {dc.DcName}, total={totalSynced}");
                }, logger, ex => (ex as NpgsqlException)?.IsTransient == true);
            }
            
            logger.LogInformation("Done");
        }
    }
}