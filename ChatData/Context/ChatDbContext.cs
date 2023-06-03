using ChatData.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatData.Context;

public class ChatDbContext : DbContext
{
	public DbSet<Chat> Chats { get; set; }
	public DbSet<Message> Messages { get; set; }

	public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
	{

	}
}
