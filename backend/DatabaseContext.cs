using backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class DatabaseContext : IdentityDbContext<User>
{
  public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    builder.Entity<ConversationMember>().HasKey(cm => new { cm.ConversationId, cm.UserId });

    builder.Entity<UserFriend>().HasKey(uf => new { uf.RequesterId, uf.ReceiverId });

    builder.Entity<UserFriend>().HasOne(uf => uf.Requester).WithMany(u => u.RequestsSent).HasForeignKey(uf => uf.RequesterId).OnDelete(DeleteBehavior.Restrict);

    builder.Entity<UserFriend>().HasOne(uf => uf.Receiver).WithMany(u => u.RequestsReceieved).HasForeignKey(uf => uf.ReceiverId).OnDelete(DeleteBehavior.Restrict);
  }

  public DbSet<Conversation> Conversations => Set<Conversation>();
  public DbSet<ConversationMember> ConversationMembers => Set<ConversationMember>();
  public DbSet<Message> Messages => Set<Message>();
  public DbSet<UserFriend> UserFriends => Set<UserFriend>();
}