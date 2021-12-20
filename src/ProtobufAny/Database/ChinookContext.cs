using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProtobufAny.Database.Models;
using ProtobufAny.Database.Configurations;

namespace ProtobufAny.Database
{
    public class ChinookContext : DbContext
    {
        public DbSet<Artist> Artist { get; set; }
        public DbSet<Album> Album { get; set; }

        public ChinookContext(DbContextOptions<ChinookContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new ArtistMap());
            modelBuilder.ApplyConfiguration(new AlbumMap());
        }
    }

}