﻿// <auto-generated />
using System;
using Masya.TelegramBot.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Masya.TelegramBot.DataAccess.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.8")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Masya.TelegramBot.DataAccess.Models.Agency", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("DateOfUnblock")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasMaxLength(1024)
                        .HasColumnType("nvarchar(1024)");

                    b.Property<string>("ImportUrl")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool?>("IsRegWithoutAdmin")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("RegistrationKey")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.HasKey("Id");

                    b.ToTable("Agencies");
                });

            modelBuilder.Entity("Masya.TelegramBot.DataAccess.Models.BotSettings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("BotToken")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("bit");

                    b.Property<bool>("IsImporting")
                        .HasColumnType("bit");

                    b.Property<string>("WebhookHost")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.ToTable("BotSettings");
                });

            modelBuilder.Entity("Masya.TelegramBot.DataAccess.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("SuperType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("Masya.TelegramBot.DataAccess.Models.Command", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("DisplayInMenu")
                        .HasColumnType("bit");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<int?>("ParentId")
                        .HasColumnType("int");

                    b.Property<int>("Permission")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("Commands");
                });

            modelBuilder.Entity("Masya.TelegramBot.DataAccess.Models.Directory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.ToTable("Directories");
                });

            modelBuilder.Entity("Masya.TelegramBot.DataAccess.Models.DirectoryItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CategoryId")
                        .HasColumnType("int");

                    b.Property<int>("DirectoryId")
                        .HasColumnType("int");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("DirectoryId");

                    b.ToTable("DirectoryItems");
                });

            modelBuilder.Entity("Masya.TelegramBot.DataAccess.Models.Favorites", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("PropertyObjectId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<long?>("UserId1")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("PropertyObjectId");

                    b.HasIndex("UserId1");

                    b.ToTable("Favorites");
                });

            modelBuilder.Entity("Masya.TelegramBot.DataAccess.Models.RealtyObject", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AgentId")
                        .HasColumnType("int");

                    b.Property<long?>("AgentId1")
                        .HasColumnType("bigint");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("DistrictId")
                        .HasColumnType("int");

                    b.Property<DateTime>("EditedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("Floor")
                        .HasColumnType("int");

                    b.Property<int?>("InternalId")
                        .HasColumnType("int");

                    b.Property<float?>("KitchenSpace")
                        .HasColumnType("real");

                    b.Property<float?>("LivingSpace")
                        .HasColumnType("real");

                    b.Property<float?>("LotArea")
                        .HasColumnType("real");

                    b.Property<DateTime?>("MailingDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("StateId")
                        .HasColumnType("int");

                    b.Property<int?>("StreetId")
                        .HasColumnType("int");

                    b.Property<float?>("TotalArea")
                        .HasColumnType("real");

                    b.Property<int?>("TotalFloors")
                        .HasColumnType("int");

                    b.Property<int?>("TypeId")
                        .HasColumnType("int");

                    b.Property<int?>("WallMaterialId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AgentId1");

                    b.HasIndex("CategoryId");

                    b.HasIndex("DistrictId");

                    b.HasIndex("StateId");

                    b.HasIndex("StreetId");

                    b.HasIndex("TypeId");

                    b.HasIndex("WallMaterialId");

                    b.ToTable("RealtyObjects");
                });

            modelBuilder.Entity("Masya.TelegramBot.DataAccess.Models.Reference", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ReferenceId")
                        .HasColumnType("int");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("References");
                });

            modelBuilder.Entity("Masya.TelegramBot.DataAccess.Models.Report", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("PropertyObjectId")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<long?>("UserId1")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("PropertyObjectId");

                    b.HasIndex("UserId1");

                    b.ToTable("Reports");
                });

            modelBuilder.Entity("Masya.TelegramBot.DataAccess.Models.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AgencyId")
                        .HasColumnType("int");

                    b.Property<string>("BlockReason")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("IsBlocked")
                        .HasColumnType("bit");

                    b.Property<bool?>("IsBlockedByBot")
                        .HasColumnType("bit");

                    b.Property<bool>("IsIgnored")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastCalledAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Note")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("Permission")
                        .HasColumnType("int");

                    b.Property<long>("TelegramAccountId")
                        .HasColumnType("bigint");

                    b.Property<byte[]>("TelegramAvatar")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("TelegramFirstName")
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("TelegramLastName")
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("TelegramLogin")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("TelegramPhoneNumber")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("Id");

                    b.HasIndex("AgencyId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Masya.TelegramBot.DataAccess.Models.Command", b =>
                {
                    b.HasOne("Masya.TelegramBot.DataAccess.Models.Command", "ParentCommand")
                        .WithMany("Aliases")
                        .HasForeignKey("ParentId");

                    b.Navigation("ParentCommand");
                });

            modelBuilder.Entity("Masya.TelegramBot.DataAccess.Models.DirectoryItem", b =>
                {
                    b.HasOne("Masya.TelegramBot.DataAccess.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId");

                    b.HasOne("Masya.TelegramBot.DataAccess.Models.Directory", "Directory")
                        .WithMany()
                        .HasForeignKey("DirectoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Directory");
                });

            modelBuilder.Entity("Masya.TelegramBot.DataAccess.Models.Favorites", b =>
                {
                    b.HasOne("Masya.TelegramBot.DataAccess.Models.RealtyObject", "PropertyObject")
                        .WithMany()
                        .HasForeignKey("PropertyObjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Masya.TelegramBot.DataAccess.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId1");

                    b.Navigation("PropertyObject");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Masya.TelegramBot.DataAccess.Models.RealtyObject", b =>
                {
                    b.HasOne("Masya.TelegramBot.DataAccess.Models.User", "Agent")
                        .WithMany("PropertyObjects")
                        .HasForeignKey("AgentId1");

                    b.HasOne("Masya.TelegramBot.DataAccess.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Masya.TelegramBot.DataAccess.Models.DirectoryItem", "District")
                        .WithMany()
                        .HasForeignKey("DistrictId");

                    b.HasOne("Masya.TelegramBot.DataAccess.Models.DirectoryItem", "State")
                        .WithMany()
                        .HasForeignKey("StateId");

                    b.HasOne("Masya.TelegramBot.DataAccess.Models.DirectoryItem", "Street")
                        .WithMany()
                        .HasForeignKey("StreetId");

                    b.HasOne("Masya.TelegramBot.DataAccess.Models.DirectoryItem", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId");

                    b.HasOne("Masya.TelegramBot.DataAccess.Models.DirectoryItem", "WallMaterial")
                        .WithMany()
                        .HasForeignKey("WallMaterialId");

                    b.Navigation("Agent");

                    b.Navigation("Category");

                    b.Navigation("District");

                    b.Navigation("State");

                    b.Navigation("Street");

                    b.Navigation("Type");

                    b.Navigation("WallMaterial");
                });

            modelBuilder.Entity("Masya.TelegramBot.DataAccess.Models.Report", b =>
                {
                    b.HasOne("Masya.TelegramBot.DataAccess.Models.RealtyObject", "PropertyObject")
                        .WithMany()
                        .HasForeignKey("PropertyObjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Masya.TelegramBot.DataAccess.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId1");

                    b.Navigation("PropertyObject");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Masya.TelegramBot.DataAccess.Models.User", b =>
                {
                    b.HasOne("Masya.TelegramBot.DataAccess.Models.Agency", "Agency")
                        .WithMany("Users")
                        .HasForeignKey("AgencyId");

                    b.Navigation("Agency");
                });

            modelBuilder.Entity("Masya.TelegramBot.DataAccess.Models.Agency", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("Masya.TelegramBot.DataAccess.Models.Command", b =>
                {
                    b.Navigation("Aliases");
                });

            modelBuilder.Entity("Masya.TelegramBot.DataAccess.Models.User", b =>
                {
                    b.Navigation("PropertyObjects");
                });
#pragma warning restore 612, 618
        }
    }
}
