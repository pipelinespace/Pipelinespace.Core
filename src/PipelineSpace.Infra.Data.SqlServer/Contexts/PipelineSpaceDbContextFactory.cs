using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace PipelineSpace.Infra.Data.SqlServer.Contexts
{
    public class PipelineSpaceDbContextFactory : IDesignTimeDbContextFactory<PipelineSpaceDbContext>
    {
        public PipelineSpaceDbContext Create(string connectionString = null)
        {
            return CreateDbContext(new string[] { connectionString });
        }

        public PipelineSpaceDbContext CreateDbContext(string[] args)
        {
            Console.WriteLine($"PipelineSpaceDbContext - With arguments: {JsonConvert.SerializeObject(args)}");

            IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("dbsettings.json")
            .AddJsonFile($"dbsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true)
            .AddEnvironmentVariables()
            .Build();

            var builder = new DbContextOptionsBuilder<PipelineSpaceDbContext>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (String.IsNullOrWhiteSpace(connectionString) == true)
            {
                throw new InvalidOperationException("Could not find a connection string named 'DefaultConnection'.");
            }

            if (args.Length > 0)
            {
                connectionString = args[0];
            }

            builder.UseSqlServer(connectionString);
            builder.UseLazyLoadingProxies();

            return new PipelineSpaceDbContext(builder.Options);
        }
    }

    public class ConfigurationDbContextFactory : IDesignTimeDbContextFactory<ConfigurationDbContext>
    {
        public ConfigurationDbContext Create(string connectionString = null)
        {
            return CreateDbContext(new string[] { connectionString });
        }

        public ConfigurationDbContext CreateDbContext(string[] args)
        {
            Console.WriteLine($"ConfigurationDbContext - With arguments: {JsonConvert.SerializeObject(args)}");

            IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("dbsettings.json")
            .AddJsonFile($"dbsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true)
            .AddEnvironmentVariables()
            .Build();

            var builder = new DbContextOptionsBuilder<ConfigurationDbContext>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (String.IsNullOrWhiteSpace(connectionString) == true)
            {
                throw new InvalidOperationException("Could not find a connection string named 'DefaultConnection'.");
            }

            if (args.Length > 0)
            {
                connectionString = args[0];
            }

            builder.UseSqlServer(connectionString, x => x.MigrationsAssembly(typeof(PipelineSpaceDbContext).GetTypeInfo().Assembly.GetName().Name));

            return new ConfigurationDbContext(builder.Options, new IdentityServer4.EntityFramework.Options.ConfigurationStoreOptions());
        }
    }

    public class PersistedGrantDbContextFactory : IDesignTimeDbContextFactory<PersistedGrantDbContext>
    {
        public PersistedGrantDbContext Create(string connectionString = null)
        {
            return CreateDbContext(new string[] { connectionString });
        }

        public PersistedGrantDbContext CreateDbContext(string[] args)
        {
            Console.WriteLine($"PersistedGrantDbContext - With arguments: {JsonConvert.SerializeObject(args)}");

            IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("dbsettings.json")
            .AddJsonFile($"dbsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true)
            .AddEnvironmentVariables()
            .Build();

            var builder = new DbContextOptionsBuilder<PersistedGrantDbContext>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (String.IsNullOrWhiteSpace(connectionString) == true)
            {
                throw new InvalidOperationException("Could not find a connection string named 'DefaultConnection'.");
            }

            if (args.Length > 0)
            {
                connectionString = args[0];
            }

            builder.UseSqlServer(connectionString, x => x.MigrationsAssembly(typeof(PipelineSpaceDbContext).GetTypeInfo().Assembly.GetName().Name));

            return new PersistedGrantDbContext(builder.Options, new IdentityServer4.EntityFramework.Options.OperationalStoreOptions());
        }
    }
}
