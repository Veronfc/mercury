using backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class DatabaseContext : IdentityDbContext<User>
{
  public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.Entity<ConversationMember>().HasKey(cm => new { cm.ConversationId, cm.UserId });
  }

  public DbSet<Conversation> Conversations => Set<Conversation>();
  public DbSet<ConversationMember> ConversationMembers => Set<ConversationMember>();
  public DbSet<Message> Messages => Set<Message>();
}