using System;
using System.Collections.Generic;
using System.Linq;
using OrderHub.Domain;
using OrderHub.Services;
using OrderHub.Collections;

class Program
{
    static void Main()
    {
        
        // ЧАСТЬ 1. Склад (WarehouseService)
        var warehouse = new WarehouseService();

        warehouse.Receive("Клавиатура", 10);
        warehouse.Receive("Клавиатура", 5); // количество должно увеличиться, а не завести вторую запись
        warehouse.Receive("Монитор", 3);

        warehouse.Sell("Монитор", 1);
        warehouse.Sell("Монитор", 99);   // предупреждение: товара недостаточно
        warehouse.Sell("Проектор", 1);   // сообщение: товара нет на складе

        warehouse.PrintStock();

        // ЧАСТЬ 2. Заказы (OrderService)
        var service = new OrderService();
        service.CreateOrder("Иванов", "Клавиатура", 5490m);
        service.CreateOrder("Петрова", "Монитор", 18990m);
        service.CreateOrder("Сидоров", "Мышь", 1290m);

        service.ProcessNext();
        service.ProcessNext();
        service.PrintAllOrders();
        service.PrintPendingQueue();
        service.PrintRecentActions();


        service.InitHistory(
            service.CreateOrder("Иванов", "Клавиатура", 5490m),
            service.CreateOrder("Петрова", "Монитор", 18990m)
        );

        // ЧАСТЬ 3. Каталог (CatalogService)
        var catalog = new CatalogService();
        catalog.AddProduct("Клавиатура", 2500m, "Периферия");
        catalog.AddProduct("Мышь", 1200m, "Периферия");
        catalog.AddProduct("Монитор", 18000m, "Дисплеи");
        catalog.AddProduct("Наушники", 4500m, "Аудио");

        catalog.PrintAll();
        catalog.PrintSortedByNaturalOrder();
        catalog.PrintSortedByName();
        catalog.PrintSortedByNameLength();

        // ЧАСТЬ 4. Отчёты (ReportService) — этапы 5–6
        var reports = new ReportService();

        var products = new List<Product>
        {
            new("Клавиатура", 2500m, "Периферия"),
            new("Мышь", 1200m, "Периферия"),
            new("Монитор", 18000m, "Дисплеи"),
            new("Наушники", 4500m, "Аудио"),
        };

        const decimal threshold = 5000m;

        var list = reports.ExpensiveProducts(products, threshold);
        foreach (string line in list)
            Console.WriteLine(line);

        Console.WriteLine(reports.CountExpensive(products, threshold));
        Console.WriteLine(reports.HasVeryExpensive(products, 50000m));
        Console.WriteLine(reports.AllInStock(products));
        Console.WriteLine(reports.Cheapest(products));

        var summary = reports.SummarizeByCategory(products);
        foreach (var row in summary)
            Console.WriteLine($"{row.Category}: {row.Count} шт., средняя {row.AveragePrice:0.00} ₽");

        Console.WriteLine();

        var topCategory = reports.MostExpensiveCategory(products);
        Console.WriteLine(topCategory is null
            ? "Каталог пуст — нет категорий."
            : $"Самая дорогая категория: {topCategory.Category} (средняя {topCategory.AveragePrice:0.00} ₽)");

        var big = reports.CategoriesWithAtLeast(products, 2);
        Console.WriteLine($"Категории с ≥2 товарами: {string.Join(", ", big)}");

        // ЧАСТЬ 5. Join / GroupBy / Take — этап 7
        
        var customers = new List<Customer>
        {
            new(1, "Иван",  "Москва"),
            new(2, "Мария", "Санкт-Петербург"),
            new(3, "Пётр",  "Казань"),
            new(4, "Ольга", "Москва"),  
        };

        var orders = new List<Order>
        {
            new() { CustomerId = 1, Price = 12000m, CreatedAt = new DateTime(2026, 1, 10) },
            new() { CustomerId = 1, Price =  8000m, CreatedAt = new DateTime(2026, 3, 15) },
            new() { CustomerId = 2, Price = 25000m, CreatedAt = new DateTime(2026, 2, 20) },
            new() { CustomerId = 2, Price =  5000m, CreatedAt = new DateTime(2025, 12, 1) },
            new() { CustomerId = 3, Price = 30000m, CreatedAt = new DateTime(2026, 5, 5) },
            new() { CustomerId = 3, Price = 30000m, CreatedAt = new DateTime(2026, 6, 6) },
            new() { CustomerId = 3, Price = 30000m, CreatedAt = new DateTime(2026, 7, 7) },
        };

        Console.WriteLine("Топ-2 клиента по сумме заказов");
        foreach (var line in reports.TopCustomers(customers, orders, 2))
            Console.WriteLine(line);

        Console.WriteLine($"\nВыручка за 2026: {reports.RevenueForYear(orders, 2026):0.00} ₽");
        Console.WriteLine($"Выручка за 2025: {reports.RevenueForYear(orders, 2025):0.00} ₽");

        var topCustomer = reports.TopCustomerByOrderCount(customers, orders);
        Console.WriteLine($"\nКлиент с максимумом заказов: {topCustomer ?? "(нет данных)"}");

        Console.WriteLine("\nЗаказы дороже среднего");
        decimal avg = orders.Average(o => o.Price);
        Console.WriteLine($"(средняя: {avg:0.00} ₽)");
        foreach (var o in reports.AboveAverageOrders(orders))
            Console.WriteLine($"  {o.Price:0.00} ₽ ({o.CreatedAt:yyyy-MM-dd})");

        // ЧАСТЬ 6. Итераторы — этап 8

        Console.WriteLine(" ДЕМОНСТРАЦИЯ 1: бесконечный Orders() ");

        IEnumerable<Order> stream = OrderStream.Orders();
        Console.WriteLine("После вызова метода — ни одной строки \"Готовлю...\" ещё нет");

        foreach (Order o in stream.Take(5))
        {
            Console.WriteLine($"  Получен: {o.CustomerName}, {o.Price:0.00} ₽");
        }

        Console.WriteLine();
        Console.WriteLine(" ДЕМОНСТРАЦИЯ 2: конечный Expensive(10, 10000m) ");

        foreach (Order o in OrderStream.Expensive(10, 10000m))
        {
            Console.WriteLine($"  Дорогой: {o.CustomerName}, {o.Price:0.00} ₽");
        }

        Console.WriteLine();
        Console.WriteLine(" ДЕМОНСТРАЦИЯ 3: почему Orders().ToList() зависнет ");
        Console.WriteLine("Orders() — бесконечный итератор: while(true) + yield return.");
        Console.WriteLine("ToList() пытается материализовать ВСЮ последовательность —");
        Console.WriteLine("он будет вызывать MoveNext() бесконечно, память кончится (OOM).");
        Console.WriteLine("Спасение — ограничить потребление: Take(N) ДО ToList().");
        Console.WriteLine("Ограниченная версия:");

        var safe = OrderStream.Orders().Take(3).ToList();
        foreach (var o in safe)
            Console.WriteLine($"  {o.CustomerName}, {o.Price:0.00} ₽");
        // ЧАСТЬ 7. Кольцевой буфер — этап 9

        Console.WriteLine();
        Console.WriteLine("=== CircularBuffer<string>(3) ===");

        var buffer = new CircularBuffer<string>(3);

        buffer.Add("1");
        buffer.Add("2");
        buffer.Add("3");
        buffer.Add("4");   // перезапишет "1"
        buffer.Add("5");   // перезапишет "2"

        Console.Write("Прямой обход (3 4 5): ");
        foreach (var item in buffer)
            Console.Write($"{item} ");
        Console.WriteLine();

        Console.Write("Обратный обход (5 4 3): ");
        foreach (var item in buffer.Reverse())
            Console.Write($"{item} ");
        Console.WriteLine();

        Console.WriteLine($"Count = {buffer.Count}, IsFull = {buffer.IsFull}");
        Console.WriteLine("LINQ: " + string.Join(", ", buffer.Select(x => $"[{x}]")));
    }
}