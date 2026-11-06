using System;
using System.Collections;
using System.Collections.Generic;

namespace OrderHub.Collections;

public class CircularBuffer<T> : IEnumerable<T>
{
    // «јƒј„ј 1: внутренний массив заданной Ємкости
    private readonly T[] buffer;

    // «јƒј„ј 2: счЄтчики
    private int next = 0;          // куда писать следующий элемент
    private int size = 0;          // сколько элементов сейчас хранитс€
    private readonly int capacity; // Ємкость (не мен€етс€)

    // «јƒј„ј 3: конструктор
    public CircularBuffer(int capacity)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity), "®мкость должна быть положительной.");

        this.capacity = capacity;
        buffer = new T[capacity];
    }

    public int Count => size;
    public bool IsFull => size == capacity;

    public void Add(T item)
    {
        // «јƒј„ј 1: запишите элемент в buffer[next]
        buffer[next] = item;

        // «јƒј„ј 2: сдвиньте next по кольцу
        next = (next + 1) % capacity;

        // «јƒј„ј 3: если size < capacity Ч увеличьте size
        if (size < capacity)
            size++;

        // «јƒј„ј 4: если буфер полон Ч size не трогаем:
        // старый элемент уже перезаписан в buffer[next]
    }

    // «јƒј„ј: обход от самого старого к самому новому
    public IEnumerator<T> GetEnumerator()
    {
        if (size == 0) yield break;

        // »ндекс самого старого элемента
        int index = (next - size + capacity) % capacity;

        for (int i = 0; i < size; i++)
        {
            yield return buffer[index];
            index = (index + 1) % capacity;
        }
    }

    // «јƒј„ј: обратный обход Ч от самого нового к самому старому
    public IEnumerable<T> Reverse()
    {
        if (size == 0) yield break;

        // —амый новый Ч это (next - 1 + capacity) % capacity
        int index = (next - 1 + capacity) % capacity;

        for (int i = 0; i < size; i++)
        {
            yield return buffer[index];
            index = (index - 1 + capacity) % capacity;
        }
    }

    // «јƒј„ј: €вна€ реализаци€ неуниверсального GetEnumerator
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}