﻿// <auto-generated />
using EF.Naming;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EF.Naming.Migrations
{
    [DbContext(typeof(ChinookContext))]
    [Migration("20210618011259_Init-Database")]
    partial class InitDatabase
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.7");

            modelBuilder.Entity("EF.Naming.Models.Album", b =>
                {
                    b.Property<int>("AlbumId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("album_id");

                    b.Property<int>("ArtistId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("artist_id");

                    b.Property<string>("TenantId")
                        .HasColumnType("TEXT")
                        .HasColumnName("tenant_id");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT")
                        .HasColumnName("title");

                    b.HasKey("AlbumId")
                        .HasName("pk_album");

                    b.ToTable("Album");
                });

            modelBuilder.Entity("EF.Naming.Models.Artist", b =>
                {
                    b.Property<int>("ArtistId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("artist_id");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.Property<string>("TenantId")
                        .HasColumnType("TEXT")
                        .HasColumnName("tenant_id");

                    b.HasKey("ArtistId")
                        .HasName("pk_artist");

                    b.ToTable("Artist");
                });
#pragma warning restore 612, 618
        }
    }
}
