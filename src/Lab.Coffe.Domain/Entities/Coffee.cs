namespace Lab.Coffe.Domain.Entities;

public class Coffee : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public bool IsActive { get; private set; }

    protected Coffee() 
    {
        Name = string.Empty;
        Description = string.Empty;
    } // Para ORM

    public Coffee(string name, string description, decimal price, int stock)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Price = price;
        Stock = stock;
        IsActive = true;
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));
        
        Name = name;
        UpdateTimestamp();
    }

    public void UpdatePrice(decimal price)
    {
        if (price < 0)
            throw new ArgumentException("Price cannot be negative", nameof(price));
        
        Price = price;
        UpdateTimestamp();
    }

    public void UpdateStock(int stock)
    {
        if (stock < 0)
            throw new ArgumentException("Stock cannot be negative", nameof(stock));
        
        Stock = stock;
        UpdateTimestamp();
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdateTimestamp();
    }

    public void Activate()
    {
        IsActive = true;
        UpdateTimestamp();
    }
}
