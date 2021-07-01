﻿// <auto-generated />
using System;
using Asc_Time_Tracker.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Asc_Time_Tracker.Migrations
{
    [DbContext(typeof(Asc_Time_TrackerContext))]
    partial class Asc_Time_TrackerContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.6")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Asc_Time_Tracker.Models.TimeLog", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DATE")
                        .HasColumnType("datetime2");

                    b.Property<int>("EMPID")
                        .HasColumnType("int");

                    b.Property<string>("JOBNUM")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NOTES")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("RD")
                        .HasColumnType("bit");

                    b.Property<double>("TIME")
                        .HasColumnType("float");

                    b.HasKey("ID");

                    b.ToTable("TimeLog");
                });
#pragma warning restore 612, 618
        }
    }
}