using System.Collections.Generic;
using OrderHub.Domain;
namespace OrderHub.Services;
public class OrderService
{
    private List<Order> allOrders = new List<Order>();
    private Queue<Order> pendingQueue = new Queue<Order>();
    private Stack<string> undoHistory = new Stack<string>();
    private int _nextOrderId = 1;

    private LinkedList<Order> orderHistory = new LinkedList<Order>();
    private LinkedListNode<Order>? current = null;

    public bool TryFindOrder(string customerName, out Order? order)
    {
        LinkedListNode<Order>? node = orderHistory.Find(o => o.CustomerName == customerName);
        if (node != null)
        {
            order = node.Value;
            return true;
        }
        order = null;
        return false;
    }
    public void PrintHistory()
    {
        foreach(Order order in orderHistory)
        {
            console.WriteLine(order);
        }
        Console.WriteLine(current?.Value?.ToString() ?? "Текущий заказ не выбран");
    }
    
}
