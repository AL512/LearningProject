using Microsoft.EntityFrameworkCore;

namespace SOLID.Support;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }  = null;
}

