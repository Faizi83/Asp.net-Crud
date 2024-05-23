using crud.Models;
using Microsoft.EntityFrameworkCore;

namespace crud.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<prodinfo> Product { get; set; }
    }
}
