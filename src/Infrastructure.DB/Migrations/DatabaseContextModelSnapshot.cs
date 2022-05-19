﻿// <auto-generated />
using System;
using Infrastructure.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.DB.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Domain.Model.Media", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Media", (string)null);
                });

            modelBuilder.Entity("Domain.Model.ScrapeTarget", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("MediaId")
                        .HasColumnType("uuid");

                    b.Property<string>("RelativeUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("WebsiteId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("MediaId");

                    b.HasIndex("WebsiteId");

                    b.ToTable("ScrapeTarget", (string)null);
                });

            modelBuilder.Entity("Domain.Model.Subscriber", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("ExternalIdentifier")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ExternalIdentifier")
                        .IsUnique();

                    b.ToTable("Subscriber", (string)null);
                });

            modelBuilder.Entity("Domain.Model.Subscription", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("MediaId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("SubscriberId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("MediaId");

                    b.HasIndex("SubscriberId", "MediaId")
                        .IsUnique();

                    b.ToTable("Subscription", (string)null);
                });

            modelBuilder.Entity("Domain.Model.Website", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Website", (string)null);
                });

            modelBuilder.Entity("Domain.Model.Media", b =>
                {
                    b.OwnsOne("Domain.Model.Media.NewestRelease#Domain.Model.Release", "NewestRelease", b1 =>
                        {
                            b1.Property<Guid>("MediaId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Link")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("MediaId");

                            b1.ToTable("Media", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("MediaId");

                            b1.OwnsOne("Domain.Model.Media.NewestRelease#Domain.Model.Release.ReleaseNumber#Domain.Model.ReleaseNumber", "ReleaseNumber", b2 =>
                                {
                                    b2.Property<Guid>("ReleaseMediaId")
                                        .HasColumnType("uuid");

                                    b2.Property<int>("Major")
                                        .HasColumnType("integer");

                                    b2.Property<int>("Minor")
                                        .HasColumnType("integer");

                                    b2.HasKey("ReleaseMediaId");

                                    b2.ToTable("Media", (string)null);

                                    b2.WithOwner()
                                        .HasForeignKey("ReleaseMediaId");
                                });

                            b1.Navigation("ReleaseNumber")
                                .IsRequired();
                        });

                    b.Navigation("NewestRelease");
                });

            modelBuilder.Entity("Domain.Model.ScrapeTarget", b =>
                {
                    b.HasOne("Domain.Model.Media", null)
                        .WithMany("ScrapeTargets")
                        .HasForeignKey("MediaId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Domain.Model.Website", null)
                        .WithMany()
                        .HasForeignKey("WebsiteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Model.Subscription", b =>
                {
                    b.HasOne("Domain.Model.Media", null)
                        .WithMany()
                        .HasForeignKey("MediaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Model.Subscriber", null)
                        .WithMany("Subscriptions")
                        .HasForeignKey("SubscriberId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Domain.Model.Media", b =>
                {
                    b.Navigation("ScrapeTargets");
                });

            modelBuilder.Entity("Domain.Model.Subscriber", b =>
                {
                    b.Navigation("Subscriptions");
                });
#pragma warning restore 612, 618
        }
    }
}
