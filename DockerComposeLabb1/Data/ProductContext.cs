using DockerComposeLabb1.Models;
using Microsoft.EntityFrameworkCore;

namespace DockerComposeLabb1.Data
{
    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions options) :base(options) { }
        
        public DbSet<Product> Products { get; set; }
    }
}
