using CampusEats.Features.Menu;
using CampusEats.Features.Order;
using CampusEats.Features.Inventory;
using Microsoft.EntityFrameworkCore;

namespace CampusEats.Persistence;

public class CampusEatsContext(DbContextOptions<CampusEatsContext> options) : DbContext(options)
{
    public DbSet<Menu> Menu { get; set; }
    public DbSet<MenuItem> MenuItem { get; set; }
    public DbSet<Order> Order { get; set; }
    public DbSet<InventoryDay> InventoryDay { get; set; }
    public DbSet<InventoryDayItem> InventoryDayItems { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // menu entity
        modelBuilder.Entity<Menu>(entity =>
        {
            entity.ToTable("Menus");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            
            entity.Property(e => e.Category)
                .HasConversion<string>()
                .IsRequired();
            
            entity.Property(e => e.Restrictions)
                .HasConversion<string>()
                .IsRequired();
            
            // stocheaza lista de guid ca json
            entity.Property(e => e.ItemId)
                .HasColumnType("jsonb")
                .IsRequired();
        });

        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.ToTable("MenuItems");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500);
            
            entity.Property(e => e.Allergens)
                .HasColumnType("jsonb");
        });
        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Orders");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.ClientId)
                .IsRequired();

            entity.Property(e => e.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            
            entity.Property(e => e.MenuIDs)
                .HasColumnType("jsonb")
                .IsRequired();

            entity.Property(e => e.ItemIDs)
                .HasColumnType("jsonb")
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");
            
            entity.Property(e => e.Status)
                .HasConversion<string>()
                .IsRequired();
        });
        modelBuilder.Entity<InventoryDay>(e =>
        {
            e.ToTable("InventoryDays");
            e.HasKey(x => x.Date);
            e.Property(x => x.GeneratedAtUtc)
                .HasColumnType("timestamp with time zone");
        });
        modelBuilder.Entity<InventoryDayItem>(e =>
        {
            e.ToTable("InventoryDayItems");
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.Date, x.ItemId }).IsUnique(); 
            e.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
            e.HasOne<InventoryDay>()
                .WithMany(d => d.Items)
                .HasForeignKey(x => x.Date)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}