// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostgresqlBuilder.cs" company="Microsoft Corporation">
//   Copyright (c) 2020 Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Common.Postgresql
{
    using Common.Config;
    using Common.KeyVault;
    using Microsoft.Azure.KeyVault;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class PostgresqlBuilder
    {
        // public static void AddPostgreSql(this IServiceCollection services)
        // {
        //     var serviceProvider = services.BuildServiceProvider();
        //     var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        //     var postgreSettings = configuration.GetConfiguredSettings<PostgreSettings>();
        //     var vaultSettings = configuration.GetConfiguredSettings<VaultSettings>();
        //     var kvClient = serviceProvider.GetRequiredService<IKeyVaultClient>();
        //     var pwd = kvClient.GetSecretAsync(vaultSettings.VaultUrl, postgreSettings.PwdSecretName).Result;
        //     var connectionString = $"Host={postgreSettings.Host};Database={postgreSettings.Db};Username={postgreSettings.User};Password={pwd.Value}";
        //     services.AddDbContext<MetaDataContext>(options =>
        //     {
        //         options.UseNpgsql(connectionString);
        //     });
        // }
    }
}