﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PrismBot.SDK.Data;

#nullable disable

namespace PrismBot.Migrations
{
    [DbContext(typeof(BotDbContext))]
    [Migration("20230128163012_test11")]
    partial class test11
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.2");

            modelBuilder.Entity("PrismBot.SDK.Models.Group", b =>
                {
                    b.Property<string>("GroupName")
                        .HasColumnType("TEXT");

                    b.Property<string>("ParentGroupName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Permissions")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("GroupName");

                    b.HasIndex("ParentGroupName");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("PrismBot.SDK.Models.Player", b =>
                {
                    b.Property<long>("QQ")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("Coins")
                        .HasColumnType("INTEGER");

                    b.Property<string>("GroupName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Permissions")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("QQ");

                    b.HasIndex("GroupName");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("PrismBot.SDK.Models.Server", b =>
                {
                    b.Property<string>("Identity")
                        .HasColumnType("TEXT");

                    b.Property<string>("Host")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Port")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ServerName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Identity");

                    b.ToTable("Servers");
                });

            modelBuilder.Entity("PrismBot.SDK.Models.Group", b =>
                {
                    b.HasOne("PrismBot.SDK.Models.Group", "Parent")
                        .WithMany()
                        .HasForeignKey("ParentGroupName");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("PrismBot.SDK.Models.Player", b =>
                {
                    b.HasOne("PrismBot.SDK.Models.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupName");

                    b.Navigation("Group");
                });
#pragma warning restore 612, 618
        }
    }
}
