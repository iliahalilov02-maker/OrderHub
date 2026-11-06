using System;
using System.Collections.Generic;
using OrderHub.Domain;

namespace OrderHub.Services;

public class OrderStream
{
    // ЗАДАЧА 1-2: бесконечный генератор с трассировкой
    public static IEnumerable<Order> Orders()
    {
        int n = 0;
        string[] clients = { "Иван", "Мария", "Пётр" };

        while (true)
        {
            n++;
            Console.WriteLine($"Готовлю заказ №{n}");

            yield return new Order
            {
                CustomerName = clients[(n - 1) % clients.Length],
                Price = 1000m * n,
                CreatedAt = new DateTime(2026, 1, 1).AddDays(n - 1)
            };
        }
    }

    // ЗАДАЧА 3: конечный итератор с фильтром
    public static IEnumerable<Order> Expensive(int limit, decimal threshold)
    {
        int n = 0;
        string[] clients = { "Иван", "Мария", "Пётр" };

        for (int i = 0; i < limit; i++)
        {
            n++;
            decimal price = 1000m * n;

            // continue пропускает итерацию БЕЗ yield return
            if (price <= threshold) continue;

            yield return new Order
            {
                CustomerName = clients[(n - 1) % clients.Length],
                Price = price,
                CreatedAt = new DateTime(2026, 1, 1).AddDays(n - 1)
            };
        }
    }
}