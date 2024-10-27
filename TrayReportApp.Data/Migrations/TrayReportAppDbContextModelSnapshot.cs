﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TrayReportApp.Data;

#nullable disable

namespace TrayReportApp.Data.Migrations
{
    [DbContext(typeof(TrayReportAppDbContext))]
    partial class TrayReportAppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CableTray", b =>
                {
                    b.Property<int>("CablesId")
                        .HasColumnType("integer");

                    b.Property<int>("RoutingId")
                        .HasColumnType("integer");

                    b.HasKey("CablesId", "RoutingId");

                    b.HasIndex("RoutingId");

                    b.ToTable("CableTray");
                });

            modelBuilder.Entity("TrayReportApp.Models.Cable", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("FromLocation")
                        .HasColumnType("text");

                    b.Property<string>("Tag")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ToLocation")
                        .HasColumnType("text");

                    b.Property<int?>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Type");

                    b.ToTable("Cables");
                });

            modelBuilder.Entity("TrayReportApp.Models.CableType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<double>("Diameter")
                        .HasColumnType("double precision");

                    b.Property<int>("Purpose")
                        .HasColumnType("integer");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("Weight")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.ToTable("CableTypes");
                });

            modelBuilder.Entity("TrayReportApp.Models.Support", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Distance")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Weight")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Supports");
                });

            modelBuilder.Entity("TrayReportApp.Models.Tray", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<double?>("FreePercentage")
                        .HasColumnType("double precision");

                    b.Property<double?>("FreeSpace")
                        .HasColumnType("double precision");

                    b.Property<int?>("Height")
                        .HasColumnType("integer");

                    b.Property<double?>("Length")
                        .HasColumnType("double precision");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Purpose")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("SupportId")
                        .HasColumnType("integer");

                    b.Property<int?>("SupportsCount")
                        .HasColumnType("integer");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double?>("Weight")
                        .HasColumnType("double precision");

                    b.Property<int?>("Width")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("SupportId");

                    b.ToTable("Trays");
                });

            modelBuilder.Entity("TrayReportApp.Models.TrayCable", b =>
                {
                    b.Property<int?>("CableId")
                        .HasColumnType("integer");

                    b.Property<int?>("TrayId")
                        .HasColumnType("integer");

                    b.HasKey("CableId", "TrayId");

                    b.HasIndex("TrayId");

                    b.ToTable("TraysCables");
                });

            modelBuilder.Entity("CableTray", b =>
                {
                    b.HasOne("TrayReportApp.Models.Cable", null)
                        .WithMany()
                        .HasForeignKey("CablesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TrayReportApp.Models.Tray", null)
                        .WithMany()
                        .HasForeignKey("RoutingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TrayReportApp.Models.Cable", b =>
                {
                    b.HasOne("TrayReportApp.Models.CableType", "CableType")
                        .WithMany("Cables")
                        .HasForeignKey("Type");

                    b.Navigation("CableType");
                });

            modelBuilder.Entity("TrayReportApp.Models.Tray", b =>
                {
                    b.HasOne("TrayReportApp.Models.Support", "Supports")
                        .WithMany()
                        .HasForeignKey("SupportId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Supports");
                });

            modelBuilder.Entity("TrayReportApp.Models.TrayCable", b =>
                {
                    b.HasOne("TrayReportApp.Models.Cable", "Cable")
                        .WithMany()
                        .HasForeignKey("CableId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TrayReportApp.Models.Tray", "Tray")
                        .WithMany()
                        .HasForeignKey("TrayId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Cable");

                    b.Navigation("Tray");
                });

            modelBuilder.Entity("TrayReportApp.Models.CableType", b =>
                {
                    b.Navigation("Cables");
                });
#pragma warning restore 612, 618
        }
    }
}
