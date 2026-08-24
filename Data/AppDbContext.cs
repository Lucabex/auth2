using Microsoft.EntityFrameworkCore;
using auth2.Models;
namespace auth2.Data;

public class AppdbContext : DbContext
{
    public AppdbContext(DbContextOptions<AppdbContext> options) : base(options)
    {
        
    }
   public DbSet<User>Users{get;set;}
}