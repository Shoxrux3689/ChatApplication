using ChatData.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatData.Context;

public class ChatDbContext : DbContext
{
	public DbSet<Chat> Chats { get; set; }
	public DbSet<Message> Messages { get; set; }
	public DbSet<UserIds> UserIds { get; set; }

	public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
	{

	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<UserIds>().HasKey(u => new {u.ChatId, u.UserId});
		
		modelBuilder.Entity<Message>()
			.HasOne(m => m.Chat)
			.WithMany(c => c.Messages)
			.HasForeignKey(m => m.ChatId);
	}
}
