using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtobufAny.Database.Models;

namespace ProtobufAny.Database.Configurations
{
    public class ArtistMap : IEntityTypeConfiguration<Artist>
    {
        public void Configure(EntityTypeBuilder<Artist> builder)
        {
            builder.ToTable("Artist");
            builder.HasKey(x => x.ArtistId);
            builder.Property(x => x.ArtistId).HasColumnName("ArtistId");
            builder.Property(x => x.Name).HasColumnName("Name");
        }
    }
}
