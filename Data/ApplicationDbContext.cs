using Fastigheterse.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fastigheterse.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Image> Images { get; set; }

    public DbSet<Property> Properties { get; set; }

    public DbSet<PropertyCat> PropertyCats { get; set; }
}

