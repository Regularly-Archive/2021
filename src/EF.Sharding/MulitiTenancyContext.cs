using EF.Sharding.Configurations;
using EF.Sharding.Models;
using EF.Sharding.MulitiTenancy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Sharding
{
    public class MulitiTenancyContext : DbContext
    {
        public DbSet<Artist> Artist { get; set; }
        public DbSet<Album> Album { get; set; }

        private readonly ITenantInfoProvider _tenantInfoProvider;
        public MulitiTenancyContext(DbContextOptions<MulitiTenancyContext> options, ITenantInfoProvider tenantInfoProvider) : base(options)
        {
            _tenantInfoProvider = tenantInfoProvider;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ArtistMap());
            modelBuilder.ApplyConfiguration(new AlbumMap());

            var tenantId = _tenantInfoProvider.GetTenantId();
            if (!string.IsNullOrEmpty(tenantId))
            {
                modelBuilder.Entity<Album>().HasQueryFilter(x => x.TenantId == tenantId);
                modelBuilder.Entity<Artist>().HasQueryFilter(x => x.TenantId == tenantId);
            }
        }
    }
}
