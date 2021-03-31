using EF.Sharding.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Sharding.Configurations
{
    public class ArtistMap : IEntityTypeConfiguration<Artist>
    {
        public void Configure(EntityTypeBuilder<Artist> builder)
        {
            builder.ToTable("Artist");
            builder.HasKey(x => x.ArtistId);
            builder.Property(x => x.ArtistId).HasColumnName("ArtistId");
            builder.Property(x => x.Name).HasColumnName("Name");
            builder.Property(x => x.TenantId).HasColumnName("TenantId");
        }
    }
}
