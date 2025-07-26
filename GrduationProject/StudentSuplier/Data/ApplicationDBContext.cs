using Microsoft.EntityFrameworkCore;
using StudentSuplier.Models;

namespace StudentSuplier.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().ToTable("Items");
        }
        public DbSet<Library> Libraries { get; set; }
        public DbSet<Order> Orders { get; set; }

    }

}
