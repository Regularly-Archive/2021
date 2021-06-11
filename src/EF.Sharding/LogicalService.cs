using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using EF.Sharding.MulitiTenancy;

namespace EF.Sharding
{
    public class LogicalService
    {
        private readonly ChinookContext _chinookContext;
        private readonly ShardingContext _shardingContext;
        private readonly MulitiTenancyContext _mulitiTenancyContext;
        private readonly IOptionsSnapshot<DatabaseOptions> _options;
        private readonly ITenantInfoProvider _tenantInfoProvider;
        public LogicalService(
            ChinookContext chinookContext, 
            ShardingContext shardingContext,
            MulitiTenancyContext mulitiTenancyContext,
            IOptionsSnapshot<DatabaseOptions> options,
            ITenantInfoProvider tenantInfoProvider
        )
        {
            _chinookContext = chinookContext;
            _shardingContext = shardingContext;
            _mulitiTenancyContext = mulitiTenancyContext;
            _tenantInfoProvider = tenantInfoProvider;
            _options = options;
        }

        public void MapShardingTables()
        {

        }

        public void ShardingWithMulitiDatabases()
        {
            // 这里随机连接到某一个数据库
            // 实际应该按照某种方式获得数据库库名后缀
            var shardings = _options.Value.MultiTenants;
            var sharding = shardings[new Random().Next(0, shardings.Count)];
            _chinookContext.Database.GetDbConnection().ConnectionString = sharding.ConnectionString;
            Console.WriteLine("--------分库场景--------");

            Console.WriteLine(_chinookContext.Database.GetDbConnection().ConnectionString);
            Console.WriteLine(_chinookContext.Album.ToQueryString());
            Console.WriteLine(_chinookContext.Artist.ToQueryString());
        }

        public void ShardingWithMulitiTables()
        {
              // 这里应该连接到Album_2021表
            // 实际应该按照某种方式获得表名后缀
            Console.WriteLine("--------分表场景--------");
            Console.WriteLine(_shardingContext.Database.GetDbConnection().ConnectionString);
            Console.WriteLine(_shardingContext.Album.ToQueryString());
            Console.WriteLine(_shardingContext.Artist.ToQueryString());
            Console.WriteLine("------------------------");
        }

        public void SharingWithMulitiTenancy()
        {
            // 这里应该查询01租户内的Album
            var tenantId = _tenantInfoProvider.GetTenantId();
            Console.WriteLine("--------多租户 + 单数据库 --------");
            Console.WriteLine($"TenantId:{tenantId}");
            Console.WriteLine(_mulitiTenancyContext.Database.GetDbConnection().ConnectionString);
            Console.WriteLine(_mulitiTenancyContext.Album.ToQueryString());
            Console.WriteLine(_mulitiTenancyContext.Artist.ToQueryString());
        }

        public void WithMulitiTenancy()
        {
            var tenantId = _tenantInfoProvider.GetTenantId();
            var database = _options.Value.MultiTenants.FirstOrDefault(x => x.TenantId == tenantId);
            if (database == null)
                throw new Exception($"Invalid tenantId \"{tenantId}\"");
            _chinookContext.Database.GetDbConnection().ConnectionString = database.ConnectionString;
            Console.WriteLine("--------多租户 + 多数据库 --------");
            Console.WriteLine($"TenantId:{tenantId}");
            Console.WriteLine(_chinookContext.Database.GetDbConnection().ConnectionString);
            Console.WriteLine(_chinookContext.Album.ToQueryString());
            Console.WriteLine(_chinookContext.Artist.ToQueryString());
        }
    }
}
