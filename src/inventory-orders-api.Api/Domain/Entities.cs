namespace InventoryOrders.Api.Domain;

public class Product
{
    public Guid Id { get; set; }
    public required string Sku { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
    public StockItem? Stock { get; set; }
}

public class StockItem
{
    public Guid ProductId { get; set; }           // PK and FK (1:1 with Product)
    public int QuantityOnHand { get; set; }
    public int QuantityReserved { get; set; }
    public byte[] RowVersion { get; set; } = [];   // optimistic concurrency token
    public int Available => QuantityOnHand - QuantityReserved;
}

public enum OrderStatus { Pending, Confirmed, Cancelled }

public class Order
{
    public Guid Id { get; set; }
    public required string CustomerId { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public List<OrderLine> Lines { get; set; } = [];
}

public class OrderLine
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }         // price snapshot at order time
}