namespace Tools.DataSync
{
    using System;
    using System.IO;
    using System.Net;
    using System.Runtime.Loader;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Auth;
    using Common.Config;
    using Common.DocDb;
    using Common.KeyVault;
    using Common.Kusto;
    using Common.Postgresql;
    using Common.Repositories;
    using Common.Telemetry;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Models;

    public class Program
    {
        private static async Task Main(string[] args)
        {
            ThreadPool.SetMinThreads(100, 100);
            ServicePointManager.DefaultConnectionLimit = 50;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var cts = new CancellationTokenSource();
            AssemblyLoadContext.Default.Unloading += (ctx) => cts.Cancel();
            Console.CancelKeyPress += (sender, cpe) => cts.Cancel();

            var builder = new HostBuilder()
                .ConfigureServices((hostingContext, services) =>
                {
                    ConfigureServices(services, args);
                })
                .UseConsoleLifetime();

            using (var host = builder.Build())
            {
                var app = host.Services.GetRequiredService<IExecutor>();
                await app.ExecuteAsync(cts.Token);
            }
        }

        private static void ConfigureServices(IServiceCollection services, string[] args)
        {
            SetupConfigOptions(services);
            RegisterDependencies(services, args);
        }

        private static void SetupConfigOptions(IServiceCollection services)
        {
            services
                .ConfigureSettings<AppInsightsSettings>()
                .ConfigureSettings<AadSettings>()
                .ConfigureSettings<VaultSettings>()
                .ConfigureSettings<DocDbSettings>()
                .ConfigureSettings<BackendData>()
                .ConfigureSettings<KustoSettings>()
                .ConfigureSettings<KustoDataSettings>()
                .ConfigureSettings<PostgreSettings>()
                .AddOptions();
        }

        private static void RegisterDependencies(IServiceCollection services, string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, false)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true, false)
                .AddJsonFile("local.settings.json", true, false)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
            services.AddSingleton<IConfiguration>(config);
            Console.WriteLine("registered configuration");

            services.AddAppInsights();
            services.AddKeyVault(config);
            services.AddKusto(config);
            services.AddSingleton<MetaDataContext>();
            
            services.AddSingleton<RepositoryFactory>();
            services.AddSingleton<KustoRepoFactory>();
            services.AddSingleton<PostgreRepoFactory>();
            
            services.AddSingleton<IExecutor, SyncKustoToPostgre>();
        }
    }
}