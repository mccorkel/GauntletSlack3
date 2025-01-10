using Microsoft.EntityFrameworkCore;
using GauntletSlack3.Shared.Models;

namespace GauntletSlack3.Api.Data;

public class SlackDbContext : DbContext
{
    public SlackDbContext(DbContextOptions<SlackDbContext> options)
        : base(options)
    {
    }

    public DbSet<Channel> Channels { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<ChannelMembership> ChannelMemberships { get; set; }
    public DbSet<MessageReaction> MessageReactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Email).IsRequired();
            entity.Property(e => e.IsAdmin).IsRequired();

            // Index for email lookups
            entity.HasIndex(e => e.Email).IsUnique();

            entity.HasMany(u => u.Memberships)
                .WithOne(cm => cm.User)
                .HasForeignKey(cm => cm.UserId);
        });

        // Configure Channel
        modelBuilder.Entity<Channel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.OwnerId).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            // Channel owner relationship
            entity.HasOne(e => e.Owner)
                .WithMany(e => e.OwnedChannels)
                .HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Index for channel name lookups
            entity.HasIndex(e => e.Name);

            // Update this to use Memberships instead of ChannelUsers
            entity.HasMany(e => e.Memberships)
                .WithOne(e => e.Channel)
                .HasForeignKey(e => e.ChannelId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Message
        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            // Parent message relationship
            entity.HasOne(e => e.ParentMessage)
                .WithMany(e => e.Replies)
                .HasForeignKey(e => e.ParentMessageId)
                .OnDelete(DeleteBehavior.Restrict);

            // Message sender relationship
            entity.HasOne(e => e.User)
                .WithMany(e => e.Messages)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Message channel relationship
            entity.HasOne(e => e.Channel)
                .WithMany(e => e.Messages)
                .HasForeignKey(e => e.ChannelId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index for channel message lookups
            entity.HasIndex(e => new { e.ChannelId, e.CreatedAt });
            entity.HasIndex(e => e.ParentMessageId);
        });

        // Configure MessageReaction
        modelBuilder.Entity<MessageReaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Emoji).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasOne(e => e.Message)
                .WithMany(e => e.Reactions)
                .HasForeignKey(e => e.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.MessageId);
            entity.HasIndex(e => e.UserId);
        });

        // Configure ChannelMembership
        modelBuilder.Entity<ChannelMembership>(entity =>
        {
            entity.HasKey(e => new { e.ChannelId, e.UserId });
            entity.Property(e => e.JoinedAt).IsRequired();

            // Channel membership relationships
            entity.HasOne(e => e.Channel)
                .WithMany(e => e.Memberships)
                .HasForeignKey(e => e.ChannelId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany(e => e.Memberships)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index for user membership lookups
            entity.HasIndex(e => e.UserId);
        });

        // Seed initial data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        var adminId = -1;
        var now = DateTime.UtcNow;

        // Seed admin user
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = adminId,
                Name = "Admin",
                Email = "admin@example.com",
                IsAdmin = true
            }
        );

        // Seed general channel
        modelBuilder.Entity<Channel>().HasData(
            new Channel
            {
                Id = -1,
                Name = "general",
                Type = "public",
                OwnerId = adminId,
                CreatedAt = now
            }
        );

        // Seed admin membership in general channel
        modelBuilder.Entity<ChannelMembership>().HasData(
            new ChannelMembership
            {
                ChannelId = -1,
                UserId = adminId,
                JoinedAt = now,
                IsMuted = false
            }
        );
    }
} 