using Microsoft.EntityFrameworkCore;

namespace Payment.API.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Payment> Payment { get; set; }
    }
}
