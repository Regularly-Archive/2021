using EF.Sharding.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Sharding
{
    public class ShardingContext : DbContext
    {
        public DbSet<Artist> Artist { get; set; }
        public DbSet<Album> Album { get; set; }

        private readonly IShardingPolicyProvider _shardingPolicyProvider;
        public string ShardingSuffix { get; private set; }

        public ShardingContext(DbContextOptions<ShardingContext> options, IShardingPolicyProvider shardingPolicyProvider) : base(options)
        {
            _shardingPolicyProvider = shardingPolicyProvider;
            ShardingSuffix = _shardingPolicyProvider.GetShardingSuffix();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Album
            modelBuilder.Entity<Album>().ToTable($"Album_{ShardingSuffix}");
            modelBuilder.Entity<Album>().HasKey(x => x.AlbumId);
            modelBuilder.Entity<Album>().Property(x => x.AlbumId).HasColumnName("AlbumId");
            modelBuilder.Entity<Album>().Property(x => x.Title).HasColumnName("Title");
            modelBuilder.Entity<Album>().Property(x => x.ArtistId).HasColumnName("ArtistId");
            modelBuilder.Entity<Album>().Property(x => x.TenantId).HasColumnName("TenantId");

            // Artist
            modelBuilder.Entity<Artist>().ToTable($"Artist_{ShardingSuffix}");
            modelBuilder.Entity<Artist>().HasKey(x => x.ArtistId);
            modelBuilder.Entity<Artist>().Property(x => x.ArtistId).HasColumnName("ArtistId");
            modelBuilder.Entity<Artist>().Property(x => x.Name).HasColumnName("Name");
            modelBuilder.Entity<Artist>().Property(x => x.TenantId).HasColumnName("TenantId");
        }
    }
}
