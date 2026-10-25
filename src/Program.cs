using System;

class Program
{
    static void Main()
    {
        var warehouse = new WarehouseService();

        warehouse.Receive("Клавиатура", 10);
        warehouse.Receive("Клавиатура", 5); // количество должно увеличиться, а не завести вторую запись
        warehouse.Receive("Монитор", 3);

        warehouse.Sell("Монитор", 1);
        warehouse.Sell("Монитор", 99);  // предупреждение: товара
        warehouse.Sell("Проектор", 1);  // сообщение: товара нет на складе

        warehouse.PrintStock();

        var service = new OrderService();
        service.CreateOrder("Иванов", "Клавиатура", 5490m);
        service.CreateOrder("Петрова", "Монитор", 18990m);
        service.CreateOrder("Сидоров", "Мышь", 1290m);
        service.ProcessNext();
        service.ProcessNext();
        service.PrintAllOrders();
        service.PrintPendingQueue();
        service.PrintRecentActions();
        service.InitHistory
            (
                service.CreateOrder("Иванов", "Клавиатура", 5490m),
                service.CreateOrder("Петрова", "Монитор", 18990m)
            );
        using OrderHub.Services;

        var catalog = new CatalogService();
        catalog.AddProduct("Клавиатура", 2500m, "Периферия");
        catalog.AddProduct("Мышь", 1200m, "Периферия");
        catalog.AddProduct("Монитор", 18000m, "Дисплеи");
        catalog.AddProduct("Наушники", 4500m, "Аудио");

        catalog.PrintAll();
        catalog.PrintSortedByNaturalOrder();
        catalog.PrintSortedByName();
        catalog.PrintSortedByNameLength();
    }
}
