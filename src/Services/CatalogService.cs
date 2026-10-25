using System;
using System.Collections.Generic;
using OrderHub.Domain;

namespace OrderHub.Services;

public class CatalogService
{
    private readonly List<Product> products = new();

    public void AddProduct(string name, decimal price, string category)
    {
        products.Add(new Product(name, price, category));
    }

    public void PrintSortedByNaturalOrder()
    {
        Console.WriteLine("Сортировка: естественный порядок (IComparable — по цене)");
        products.Sort();
        PrintList(products);
    }

    public void PrintSortedByName()
    {
        Console.WriteLine("Сортировка: по алфавиту (IComparer — NameComparer)");
        products.Sort(new NameComparer());
        PrintList(products);
    }

    public void PrintSortedByNameLength()
    {
        Console.WriteLine("Сортировка: по длине названия (лямбда Comparison<Product>)");
        Console.WriteLine("(тип Product не менялся — порядок задан разово через лямбду)");
        products.Sort((a, b) => a.Name.Length.CompareTo(b.Name.Length));
        PrintList(products);
    }

    // ЗАДАЧА: метод PrintAll() — каталог в исходном порядке
    public void PrintAll()
    {
        Console.WriteLine("Каталог в исходном порядке");
        PrintList(products);
    }

    private static void PrintList(IEnumerable<Product> items)
    {
        int i = 1;
        foreach (var p in items)
        {
            Console.WriteLine($"{i++}. {p}");
        }
        Console.WriteLine();
    }
}