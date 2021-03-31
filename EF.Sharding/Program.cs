using EF.Sharding.MulitiTenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EF.Sharding
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("AppSetting.json");
            var config = builder.Build();

            var services = new ServiceCollection();

            // DatabaseOptions
            services.Configure<DatabaseOptions>(config.GetSection("Database"));

            // DbContext
            services.AddDbContext<ChinookContext>(options => options.UseSqlite(config.GetValue<string>("Database:Default")));
            services.AddDbContext<ShardingContext>(options => {
                options.UseSqlite(config.GetValue<string>("Database:Default"));
                options.ReplaceService<IModelCacheKeyFactory, DynamicModelCacheKeyFactory>();
            });
            services.AddDbContext<MulitiTenancyContext>(options => options.UseSqlite(config.GetValue<string>("Database:Default")));

            // Sharding & MulitiTenancy
            services.AddTransient<IShardingPolicyProvider, ShardingByYearPolicy>();
            //services.AddTransient<ITenantInfoProvider, TenantInfoProvider>();
            services.AddTransient<ITenantInfoProvider, MockTenantInfoProvider>();

            // Logicals
            services.AddTransient<LogicalService>();
            var serviceProvider = services.BuildServiceProvider();
            var logicalService = serviceProvider.GetRequiredService<LogicalService>();
            logicalService.ShardingWithMulitiDatabases();
            logicalService.ShardingWithMulitiTables();
            logicalService.SharingWithMulitiTenancy();
            logicalService.WithMulitiTenancy();
        }
    }
}
