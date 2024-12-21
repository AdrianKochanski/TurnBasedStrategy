using System.Collections.Generic;
using System;

public class PriorityQueue<T> where T : IComparable<T>
{
    private T[] items;
    private HashSet<T> itemSet;
    private int size;
    private const int InitialCapacity = 16;

    public PriorityQueue()
    {
        items = new T[InitialCapacity];
        itemSet = new HashSet<T>();
        size = 0;
    }

    public int Count => size;

    public void Enqueue(T item)
    {
        if (!itemSet.Add(item)) return; 

        if (size == items.Length) Resize(items.Length * 2);

        items[size] = item;
        BubbleUp(size);
        size++;
    }

    public T Dequeue()
    {
        if (size == 0)
            throw new InvalidOperationException("The priority queue is empty.");

        T result = items[0];
        itemSet.Remove(result);

        size--;
        if (size > 0)
        {
            items[0] = items[size];
            BubbleDown(0);
        }
        items[size] = default;

        return result;
    }

    public bool Contains(T item)
    {
        return itemSet.Contains(item);
    }

    private void BubbleUp(int index)
    {
        while (index > 0)
        {
            int parentIndex = (index - 1) / 2;

            if (items[index].CompareTo(items[parentIndex]) >= 0)
                break;

            Swap(index, parentIndex);
            index = parentIndex;
        }
    }

    private void BubbleDown(int index)
    {
        while (true)
        {
            int leftChild = 2 * index + 1;
            int rightChild = 2 * index + 2;
            int smallest = index;

            if (leftChild < size && items[leftChild].CompareTo(items[smallest]) < 0)
                smallest = leftChild;

            if (rightChild < size && items[rightChild].CompareTo(items[smallest]) < 0)
                smallest = rightChild;

            if (smallest == index)
                break;

            Swap(index, smallest);
            index = smallest;
        }
    }

    private void Swap(int i, int j)
    {
        T temp = items[i];
        items[i] = items[j];
        items[j] = temp;
    }

    private void Resize(int newCapacity)
    {
        T[] newArray = new T[newCapacity];
        Array.Copy(items, newArray, size);
        items = newArray;
    }
}
