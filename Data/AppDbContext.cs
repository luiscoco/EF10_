using EF10_ExecuteUpdateDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace EF10_ExecuteUpdateDemo.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
    }
}