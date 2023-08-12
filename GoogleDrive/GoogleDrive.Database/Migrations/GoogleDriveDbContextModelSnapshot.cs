﻿// <auto-generated />
using System;
using GoogleDrive.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GoogleDrive.Database.Migrations
{
    [DbContext(typeof(GoogleDriveDbContext))]
    partial class GoogleDriveDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("GoogleDrive.Database.Entities.File", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Id", "Path")
                        .IsUnique();

                    b.ToTable("Files");
                });

            modelBuilder.Entity("GoogleDrive.Database.Entities.FileUser", b =>
                {
                    b.Property<int>("FileId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("FileId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("FileUsers");
                });

            modelBuilder.Entity("GoogleDrive.Database.Entities.FileVersionChunk", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("BlobStorageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ChunkHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ChunkNumber")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedTimestamp")
                        .HasColumnType("datetime2");

                    b.Property<int>("FileId")
                        .HasColumnType("int");

                    b.Property<int>("Version")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("FileId");

                    b.ToTable("FileVersionChunks");
                });

            modelBuilder.Entity("GoogleDrive.Database.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("GoogleDrive.Database.Entities.FileUser", b =>
                {
                    b.HasOne("GoogleDrive.Database.Entities.File", "File")
                        .WithMany("FileUsers")
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GoogleDrive.Database.Entities.User", "User")
                        .WithMany("FileUsers")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("File");

                    b.Navigation("User");
                });

            modelBuilder.Entity("GoogleDrive.Database.Entities.FileVersionChunk", b =>
                {
                    b.HasOne("GoogleDrive.Database.Entities.File", "File")
                        .WithMany("FileVersionChunks")
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("File");
                });

            modelBuilder.Entity("GoogleDrive.Database.Entities.File", b =>
                {
                    b.Navigation("FileUsers");

                    b.Navigation("FileVersionChunks");
                });

            modelBuilder.Entity("GoogleDrive.Database.Entities.User", b =>
                {
                    b.Navigation("FileUsers");
                });
#pragma warning restore 612, 618
        }
    }
}
