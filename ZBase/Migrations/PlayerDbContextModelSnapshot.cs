﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ZBase.Persistence;

namespace ZBase.Migrations
{
    [DbContext(typeof(PlayerDbContext))]
    partial class PlayerDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.0-preview.2.20159.4");

            modelBuilder.Entity("ZBase.Persistence.IpBanModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("BannedBy")
                        .HasColumnType("TEXT");

                    b.Property<string>("Ip")
                        .HasColumnType("TEXT");

                    b.Property<string>("Reason")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("IpBans");
                });

            modelBuilder.Entity("ZBase.Persistence.PlayerModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("BanMessage")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Banned")
                        .HasColumnType("INTEGER");

                    b.Property<string>("BannedBy")
                        .HasColumnType("TEXT");

                    b.Property<double>("BannedUntil")
                        .HasColumnType("REAL");

                    b.Property<int>("BoundBlock")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("GlobalChat")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Ip")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<short>("Rank")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Stopped")
                        .HasColumnType("INTEGER");

                    b.Property<double>("TimeMuted")
                        .HasColumnType("REAL");

                    b.Property<bool>("Vanished")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Players");
                });
#pragma warning restore 612, 618
        }
    }
}