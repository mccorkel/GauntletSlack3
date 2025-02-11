﻿// <auto-generated />
using System;
using GauntletSlack3.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GauntletSlack3.Api.Migrations
{
    [DbContext(typeof(SlackDbContext))]
    [Migration("20250110003356_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("GauntletSlack3.Shared.Models.Channel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("OwnerId")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.HasIndex("OwnerId");

                    b.ToTable("Channels");

                    b.HasData(
                        new
                        {
                            Id = -1,
                            CreatedAt = new DateTime(2025, 1, 10, 0, 33, 56, 391, DateTimeKind.Utc).AddTicks(6223),
                            Name = "general",
                            OwnerId = -1,
                            Type = "public"
                        });
                });

            modelBuilder.Entity("GauntletSlack3.Shared.Models.ChannelMembership", b =>
                {
                    b.Property<int>("ChannelId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<bool>("IsMuted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("JoinedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("ChannelId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("ChannelMemberships");

                    b.HasData(
                        new
                        {
                            ChannelId = -1,
                            UserId = -1,
                            IsMuted = false,
                            JoinedAt = new DateTime(2025, 1, 10, 0, 33, 56, 391, DateTimeKind.Utc).AddTicks(6223)
                        });
                });

            modelBuilder.Entity("GauntletSlack3.Shared.Models.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ChannelId")
                        .HasColumnType("int");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ParentMessageId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ParentMessageId");

                    b.HasIndex("UserId");

                    b.HasIndex("ChannelId", "CreatedAt");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("GauntletSlack3.Shared.Models.MessageReaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Emoji")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MessageId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MessageId");

                    b.HasIndex("UserId");

                    b.ToTable("MessageReactions");
                });

            modelBuilder.Entity("GauntletSlack3.Shared.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = -1,
                            Email = "admin@example.com",
                            IsAdmin = true,
                            Name = "Admin"
                        });
                });

            modelBuilder.Entity("GauntletSlack3.Shared.Models.Channel", b =>
                {
                    b.HasOne("GauntletSlack3.Shared.Models.User", "Owner")
                        .WithMany("OwnedChannels")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("GauntletSlack3.Shared.Models.ChannelMembership", b =>
                {
                    b.HasOne("GauntletSlack3.Shared.Models.Channel", "Channel")
                        .WithMany("Memberships")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GauntletSlack3.Shared.Models.User", "User")
                        .WithMany("Memberships")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Channel");

                    b.Navigation("User");
                });

            modelBuilder.Entity("GauntletSlack3.Shared.Models.Message", b =>
                {
                    b.HasOne("GauntletSlack3.Shared.Models.Channel", "Channel")
                        .WithMany("Messages")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GauntletSlack3.Shared.Models.Message", "ParentMessage")
                        .WithMany("Replies")
                        .HasForeignKey("ParentMessageId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("GauntletSlack3.Shared.Models.User", "User")
                        .WithMany("Messages")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Channel");

                    b.Navigation("ParentMessage");

                    b.Navigation("User");
                });

            modelBuilder.Entity("GauntletSlack3.Shared.Models.MessageReaction", b =>
                {
                    b.HasOne("GauntletSlack3.Shared.Models.Message", "Message")
                        .WithMany("Reactions")
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GauntletSlack3.Shared.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Message");

                    b.Navigation("User");
                });

            modelBuilder.Entity("GauntletSlack3.Shared.Models.Channel", b =>
                {
                    b.Navigation("Memberships");

                    b.Navigation("Messages");
                });

            modelBuilder.Entity("GauntletSlack3.Shared.Models.Message", b =>
                {
                    b.Navigation("Reactions");

                    b.Navigation("Replies");
                });

            modelBuilder.Entity("GauntletSlack3.Shared.Models.User", b =>
                {
                    b.Navigation("Memberships");

                    b.Navigation("Messages");

                    b.Navigation("OwnedChannels");
                });
#pragma warning restore 612, 618
        }
    }
}
