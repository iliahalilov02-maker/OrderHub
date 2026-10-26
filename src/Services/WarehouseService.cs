using System;
using System.Collections.Generic;
using System.Linq;

public class WarehouseService
{
    private Dictionary<string, int> stock = new Dictionary<string, int>();

    public void Receive(string productName, int quantity)
    {
        if (stock.TryGetValue(productName, out int existing))
        {
            stock[productName] = existing + quantity;
        }
        else
        {
            stock[productName] = quantity;
        }
        Console.WriteLine($"Принято {quantity} шт. товара \"{productName}\". Итого на складе: {stock[productName]}");
    }

    public bool Sell(string productName, int quantity)
    {
        // ЗАДАЧА 1: если товара нет вообще — сообщите об этом и верните false
        if (!stock.TryGetValue(productName, out int existing))
        {
            Console.WriteLine($"Товар \"{productName}\" отсутствует на складе.");
            return false;
        }

        // ЗАДАЧА 2: если товара недостаточно — выведите предупреждение и верните false
        if (existing < quantity)
        {
            Console.WriteLine($"Недостаточно товара \"{productName}\": запрошено {quantity}, в наличии {existing}.");
            return false;
        }

        // ЗАДАЧА 3: уменьшите остаток на quantity
        int remaining = existing - quantity;
        stock[productName] = remaining;

        // ЗАДАЧА 4: если остаток стал равен нулю — удалите запись через Remove
        if (remaining == 0)
        {
            stock.Remove(productName);
            Console.WriteLine($"Товар \"{productName}\" снят с учёта (остаток 0).");
        }
        else
        {
            Console.WriteLine($"Продано {quantity} шт. товара \"{productName}\". Остаток: {remaining}");
        }

        // ЗАДАЧА 5: верните true
        return true;
    }

    public bool TryGetStock(string productName, out int quantity)
    {
        // ЗАДАЧА: реализуйте безопасный поиск остатка через TryGetValue
        // ЗАДАЧА: при отсутствии товара присвойте quantity = 0
        if (stock.TryGetValue(productName, out quantity))
        {
            return true;
        }

        quantity = 0;
        return false;
    }

    public void PrintStock()
    {
        // ЗАДАЧА: переберите склад через foreach по KeyValuePair<string, int>
        // ЗАДАЧА: отсортируйте вывод по алфавиту (OrderBy)
        Console.WriteLine("Остатки на складе");

        if (stock.Count == 0)
        {
            Console.WriteLine("(склад пуст)");
            return;
        }

        foreach (KeyValuePair<string, int> item in stock.OrderBy(kv => kv.Key))
        {
            Console.WriteLine($"{item.Key}: {item.Value}");
        }
    }
}