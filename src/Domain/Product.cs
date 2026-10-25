namespace OrderHub.Domain;

public class Product : IComparable<Product>
{
    public string Name { get; private set; } = "";
    public decimal Price { get; private set; }
    public string Category { get; private set; } = "";

    public Product(string name, decimal price, string category)
    {
        Name = name;
        Price = price;
        Category = category;
    }
    public int CompareTo(Product? other)
    {
        // «јƒј„ј: естественный пор€док Ч по цене
        if (other is null) return 1;
        return Price.CompareTo(other.Price);
    }
    public override string ToString() => $"{Name} Ч {Price:0.00} – ({Category})";
}

// «јƒј„ј: класс NameComparer : IComparer<Product>
public class NameComparer : IComparer<Product>
{
    public int Compare(Product? a, Product? b)
    {
        // «јƒј„ј: сортировка по алфавиту названий
        return string.Compare(a?.Name, b?.Name, StringComparison.Ordinal);
    }
}

// «јƒј„ј: класс PriceDescComparer : IComparer<Product> Ч по убыванию цены
public class PriceDescComparer : IComparer<Product>
{
    public int Compare(Product? a, Product? b)
    {
        if (a is null && b is null) return 0;
        if (a is null) return 1;
        if (b is null) return -1;
        return b.Price.CompareTo(a.Price);
    }
}