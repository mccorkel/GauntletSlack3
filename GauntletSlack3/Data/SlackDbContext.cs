using Microsoft.EntityFrameworkCore;
using GauntletSlack3.Shared.Models;

public class SlackDbContext : DbContext
{
    public SlackDbContext(DbContextOptions<SlackDbContext> options)
        : base(options)
    {
    }

    public DbSet<Channel> Channels { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<ChannelUser> ChannelUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure ChannelUser composite key
        modelBuilder.Entity<ChannelUser>()
            .HasKey(cu => new { cu.ChannelId, cu.UserId });

        // Configure relationships
        modelBuilder.Entity<Channel>()
            .HasMany(c => c.Messages)
            .WithOne(m => m.Channel)
            .HasForeignKey(m => m.ChannelId);

        modelBuilder.Entity<Channel>()
            .HasMany(c => c.ChannelUsers)
            .WithOne(cu => cu.Channel)
            .HasForeignKey(cu => cu.ChannelId);

        // Add default channels
        modelBuilder.Entity<Channel>().HasData(
            new Channel { Id = 1, Name = "general", Type = "public" },
            new Channel { Id = 2, Name = "random", Type = "public" }
        );
    }
}
