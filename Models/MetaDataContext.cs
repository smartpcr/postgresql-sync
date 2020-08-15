// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetaDataContext.cs" company="Microsoft Corporation">
//   Copyright (c) 2020 Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Common.Config;
    using Common.KeyVault;
    using Common.Postgresql;
    using Microsoft.Azure.KeyVault;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;
    using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public sealed class MetaDataContext : DbContext
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILoggerFactory loggerFactory;
        private ILogger<MetaDataContext> logger;
        public DbSet<PowerDevice> PowerDevices { get; set; }
        public DbSet<DataCenter> DataCenters { get; set; }
        
        public MetaDataContext(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            this.serviceProvider = serviceProvider;
            this.loggerFactory = loggerFactory;
            logger = loggerFactory.CreateLogger<MetaDataContext>();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var postgreSettings = configuration.GetConfiguredSettings<PostgreSettings>();
            var vaultSettings = configuration.GetConfiguredSettings<VaultSettings>();
            var kvClient = serviceProvider.GetRequiredService<IKeyVaultClient>();
            var pwd = kvClient.GetSecretAsync(vaultSettings.VaultUrl, postgreSettings.PwdSecretName).Result;
            var connectionString = $"Host={postgreSettings.Host};Database={postgreSettings.Db};Username={postgreSettings.User};Password={pwd.Value};Ssl Mode=Require;";
            optionsBuilder.UseNpgsql(connectionString)
                .UseLoggerFactory(loggerFactory)
                .UseSnakeCaseNamingConvention();
            logger.LogInformation($"configured postgre sql: host={postgreSettings.Host}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var type in modelBuilder.Model.GetEntityTypes())
            {
                logger.LogInformation($"define table schema for {type.Name}");
                foreach (var prop in type.GetProperties())
                {
                    if (prop.ClrType == typeof(string))
                    {
                        // if (prop.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                        // {
                        //     prop.AsProperty()?.Builder.HasMaxLength(36);
                        // }

                        IAnnotation maxLen = prop.GetAnnotations()?.FirstOrDefault(a => a.Name.Equals("MaxLength"));
                        if (maxLen == null)
                        {
                            prop.AsProperty()?.Builder?.HasColumnType($"VARCHAR(255)");
                        }
                        else
                        {
                            var len = (int) maxLen.Value;
                            prop.AsProperty()?.Builder?.HasColumnType($"VARCHAR({len})");
                        }
                    }
                    else if (prop.ClrType.IsEnum)
                    {
                        var propType = typeof(EnumToStringConverter<>).MakeGenericType(prop.ClrType);
                        var converter = Activator.CreateInstance(propType, new ConverterMappingHints()) as ValueConverter;
                        prop.AsProperty()?.SetValueConverter(converter);
                        prop.AsProperty()?.Builder?.HasColumnType("VARCHAR(255)");
                    }
                }
            }
        }
    }
}