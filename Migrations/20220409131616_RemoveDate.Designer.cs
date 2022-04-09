﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using yrhacks2022_api.Database;

#nullable disable

namespace yrhacks2022_api.Migrations
{
    [DbContext(typeof(CacheContext))]
    [Migration("20220409131616_RemoveDate")]
    partial class RemoveDate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.3");

            modelBuilder.Entity("yrhacks2022_api.Models.CacheData", b =>
                {
                    b.Property<string>("Url")
                        .HasColumnType("TEXT");

                    b.HasKey("Url");

                    b.ToTable("FileCache");
                });

            modelBuilder.Entity("yrhacks2022_api.Models.CacheData", b =>
                {
                    b.OwnsOne("yrhacks2022_api.Models.ScanResult", "Result", b1 =>
                        {
                            b1.Property<string>("CacheDataUrl")
                                .HasColumnType("TEXT");

                            b1.Property<string>("Color")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<bool>("Delete")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("Description")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<string>("Title")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.HasKey("CacheDataUrl");

                            b1.ToTable("FileCache");

                            b1.WithOwner()
                                .HasForeignKey("CacheDataUrl");
                        });

                    b.Navigation("Result")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
