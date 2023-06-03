using IdentityApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityApi.Data.Context;

public class IdentityDbContext : DbContext
{
	public DbSet<User> Users { get; set; }

	public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
	{

	}
}
