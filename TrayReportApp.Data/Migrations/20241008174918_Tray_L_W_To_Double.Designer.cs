﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TrayReportApp.Data;

#nullable disable

namespace TrayReportApp.Data.Migrations
{
    [DbContext(typeof(TrayReportAppDbContext))]
    [Migration("20241008174918_Tray_L_W_To_Double")]
    partial class Tray_L_W_To_Double
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TrayReportApp.Models.Cable", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("FromLocation")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Tag")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ToLocation")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Type")
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

                    b.Property<string>("CrossSection")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Diameter")
                        .HasColumnType("integer");

                    b.Property<int>("Purpose")
                        .HasColumnType("integer");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Weight")
                        .HasColumnType("integer");

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

                    b.Property<int?>("TrayId")
                        .HasColumnType("integer");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Weight")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("TrayId");

                    b.ToTable("Supports");
                });

            modelBuilder.Entity("TrayReportApp.Models.Tray", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("CableId")
                        .HasColumnType("integer");

                    b.Property<double>("FreePercentage")
                        .HasColumnType("double precision");

                    b.Property<double>("FreeSpace")
                        .HasColumnType("double precision");

                    b.Property<int>("Height")
                        .HasColumnType("integer");

                    b.Property<double>("Length")
                        .HasColumnType("double precision");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Purpose")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("SupportsCount")
                        .HasColumnType("integer");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("Weight")
                        .HasColumnType("double precision");

                    b.Property<int>("Width")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CableId");

                    b.ToTable("Trays");
                });

            modelBuilder.Entity("TrayReportApp.Models.Cable", b =>
                {
                    b.HasOne("TrayReportApp.Models.CableType", "CableType")
                        .WithMany("Cables")
                        .HasForeignKey("Type")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CableType");
                });

            modelBuilder.Entity("TrayReportApp.Models.Support", b =>
                {
                    b.HasOne("TrayReportApp.Models.Tray", null)
                        .WithMany("Supports")
                        .HasForeignKey("TrayId");
                });

            modelBuilder.Entity("TrayReportApp.Models.Tray", b =>
                {
                    b.HasOne("TrayReportApp.Models.Cable", null)
                        .WithMany("Routing")
                        .HasForeignKey("CableId");
                });

            modelBuilder.Entity("TrayReportApp.Models.Cable", b =>
                {
                    b.Navigation("Routing");
                });

            modelBuilder.Entity("TrayReportApp.Models.CableType", b =>
                {
                    b.Navigation("Cables");
                });

            modelBuilder.Entity("TrayReportApp.Models.Tray", b =>
                {
                    b.Navigation("Supports");
                });
#pragma warning restore 612, 618
        }
    }
}
