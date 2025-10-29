using CampusEats.Features.Menu;
using Microsoft.EntityFrameworkCore;

namespace CampusEats.Persistence;

public class CampusEatsContext(DbContextOptions<CampusEatsContext> options) : DbContext(options)
{
    public DbSet<Menu> Menu { get; set; }
    
    public DbSet<MenuItem> MenuItem { get; set; }
}