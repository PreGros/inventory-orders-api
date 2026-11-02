using InventoryOrders.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace InventoryOrders.Api.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<StockItem> StockItems => Set<StockItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Product>(e =>
        {
            e.HasIndex(p => p.Sku).IsUnique();
            e.Property(p => p.Sku).HasMaxLength(50);
            e.Property(p => p.Name).HasMaxLength(200);
            e.Property(p => p.Price).HasPrecision(18, 2);
            e.HasOne(p => p.Stock).WithOne().HasForeignKey<StockItem>(s => s.ProductId);
        });

        b.Entity<StockItem>(e =>
        {
            e.HasKey(s => s.ProductId);
            e.Property(s => s.RowVersion).IsRowVersion();
            e.Ignore(s => s.Available);
            e.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Stock_OnHand", "[QuantityOnHand] >= 0");
                t.HasCheckConstraint("CK_Stock_Reserved", "[QuantityReserved] >= 0 AND [QuantityReserved] <= [QuantityOnHand]");
            });
        });

        b.Entity<Order>(e =>
        {
            e.Property(o => o.Status).HasConversion<string>().HasMaxLength(20);
            e.Property(o => o.CustomerId).HasMaxLength(100);
            e.HasMany(o => o.Lines).WithOne().HasForeignKey(l => l.OrderId);
        });

        b.Entity<OrderLine>(e =>
        {
            e.Property(l => l.UnitPrice).HasPrecision(18, 2);
            e.HasOne<Product>().WithMany().HasForeignKey(l => l.ProductId);
        });
    }
}