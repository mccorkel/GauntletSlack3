using Microsoft.EntityFrameworkCore;
using GauntletSlack3.Shared.Models;

namespace GauntletSlack3.Api.Data;

public class SlackDbContext : DbContext
{
    public SlackDbContext(DbContextOptions<SlackDbContext> options)
        : base(options)
    {
    }

    public DbSet<Channel> Channels => Set<Channel>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<User> Users => Set<User>();
    public DbSet<ChannelMembership> ChannelMemberships => Set<ChannelMembership>();
    public DbSet<MessageReaction> MessageReactions => Set<MessageReaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Add any additional model configuration here
    }
} 