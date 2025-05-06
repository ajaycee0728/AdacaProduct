using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AdacaProduct.Model
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options)
            : base(options) { }
        public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDBContext>
        {
            public AppDBContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<AppDBContext>();
                optionsBuilder.UseSqlServer("Server=localhost;Database=ProductDb;Trusted_Connection=True;TrustServerCertificate=True;");

                return new AppDBContext(optionsBuilder.Options);
            }
        }
        public DbSet<Product> Products { get; set; } 
    }
}
