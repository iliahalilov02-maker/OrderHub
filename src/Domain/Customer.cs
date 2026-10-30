using OrderHub.Domain;
namespace OrderHub.Services;
public class OrderService
{
    List<Order> allOrders = new List<Order>
    Queue<Order> pendingQueue = new Queue<Order>
    Stack<string> undoHistory = new Stack<string>
       
    public Order CreateOrder(string customerName, string, productName, decimal price)
    {
        int newId = allOrders.Count + 1;
        var order = new Order(newId, customerName, productName, price, DateTime.Now);
        allOrders.Add(order);
        pendingQueue.Enqueue(order);
        undoHistory.Push($"create {order.Id} for user {customerName}");
        return order;
    }
    public Order? ProcessNext()
    {
        if (pendingQueue.Count == 0) return null;
        Order order = pendingQueue.Dequeue();
        undoHistory.push($"sucsessfull {order.Id}");
        Console.WriteLine($"Обработан заказ {order}");
        return order;
    }
    public bool UndoLastAction()
    { 
        if(undoHistory.Count == 0) return false;
        string action = undoHistory.Pop();
        Console.WriteLine($"Отменено действие: {action}");
        return true;
    }
    public void PrintAllOrders()
    {
        Console.WriteLine($"all orders");
        foreach(Order order in allOrders)
        {
            Console.Writeline(order);
        }
    }
    public void PrintPendingQueue()
    {
        if (pendingQueue.Count > 0)
        {
            Order firstOrder = pendingQueue.Peek();

            Console.WriteLine($"first - {firstOrder}");
        }
        else
        {
            Console.WriteLine("Queue is empty");
        }
    }
    public void PrintRecentActions()
    {
        if (undoHistory.count == 0)
        {
            Console.WriteLine("history is empty")
            return;
        }
        foreach(string action in undoHistory)
        {
            Console.WriteLine(action);
        }
    }
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string City { get; set; } = "";

        // ЗАДАЧА: конструктор и ToString()
        public Customer(int id, string name, string city)
        {
            Id = id;
            Name = name;
            City = city;
        }

        public override string ToString() => $"{Name} (Id={Id}, {City})";
    }
}