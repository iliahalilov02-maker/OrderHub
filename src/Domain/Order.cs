namespace OrderHub.Domain;
public class Order
{
    public int CustomerId { get; set; }
    public int Id { get; set; }
    public string CustomerName { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public Order(int id,string customerName,string productName, decimal price, DateTime createdAt)
    {
        Id = id;
        CustomerName = customerName;
        ProductName = productName;
        Price = price;
        CreatedAt = DateTime.Now;

    }
    public override string ToString()
    {
        return $"#{Id} {CustomerName} — {ProductName} {Price:N2} ₽ ({CreatedAt:dd.MM.yyyy})";
    }
}