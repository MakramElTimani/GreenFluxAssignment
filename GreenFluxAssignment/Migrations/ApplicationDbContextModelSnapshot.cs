﻿// <auto-generated />
using System;
using GreenFluxAssignment.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GreenFluxAssignment.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.2");

            modelBuilder.Entity("GreenFluxAssignment.Data.ChargeStationDataModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.ToTable("ChargeStations");
                });

            modelBuilder.Entity("GreenFluxAssignment.Data.ConnectorDataModel", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("ChargeStationId")
                        .HasColumnType("TEXT");

                    b.Property<int>("MaxCurrentInAmps")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id", "ChargeStationId");

                    b.HasIndex("ChargeStationId");

                    b.ToTable("Connectors");
                });

            modelBuilder.Entity("GreenFluxAssignment.Data.GroupDataModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int>("CapacityInAmps")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("GreenFluxAssignment.Data.ChargeStationDataModel", b =>
                {
                    b.HasOne("GreenFluxAssignment.Data.GroupDataModel", "Group")
                        .WithMany("ChargeStations")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");
                });

            modelBuilder.Entity("GreenFluxAssignment.Data.ConnectorDataModel", b =>
                {
                    b.HasOne("GreenFluxAssignment.Data.ChargeStationDataModel", "ChargeStation")
                        .WithMany("Connectors")
                        .HasForeignKey("ChargeStationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ChargeStation");
                });

            modelBuilder.Entity("GreenFluxAssignment.Data.ChargeStationDataModel", b =>
                {
                    b.Navigation("Connectors");
                });

            modelBuilder.Entity("GreenFluxAssignment.Data.GroupDataModel", b =>
                {
                    b.Navigation("ChargeStations");
                });
#pragma warning restore 612, 618
        }
    }
}
