using System;
using System.Collections.Generic;

public class PriorityQueue<T> where T : IComparable<T>
{
    private List<T> items;

    public PriorityQueue()
    {
        items = new List<T>();
    }

    public int Count => items.Count;

    public void Enqueue(T item)
    {
        items.Add(item);
        int currentIndex = items.Count - 1;

        // Bubble up
        while (currentIndex > 0)
        {
            int parentIndex = (currentIndex - 1) / 2;

            if (items[currentIndex].CompareTo(items[parentIndex]) >= 0)
                break;

            Swap(currentIndex, parentIndex);
            currentIndex = parentIndex;
        }
    }

    public T Dequeue()
    {
        if (items.Count == 0)
            throw new InvalidOperationException("The priority queue is empty.");

        T result = items[0];

        // Move the last element to the root and bubble down
        items[0] = items[items.Count - 1];
        items.RemoveAt(items.Count - 1);

        int currentIndex = 0;

        while (true)
        {
            int leftChildIndex = 2 * currentIndex + 1;
            int rightChildIndex = 2 * currentIndex + 2;
            int smallestIndex = currentIndex;

            if (leftChildIndex < items.Count && items[leftChildIndex].CompareTo(items[smallestIndex]) < 0)
                smallestIndex = leftChildIndex;

            if (rightChildIndex < items.Count && items[rightChildIndex].CompareTo(items[smallestIndex]) < 0)
                smallestIndex = rightChildIndex;

            if (smallestIndex == currentIndex)
                break;

            Swap(currentIndex, smallestIndex);
            currentIndex = smallestIndex;
        }

        return result;
    }

    public bool Contains(T item)
    {
        return items.Contains(item);
    }

    private void Swap(int index1, int index2)
    {
        T temp = items[index1];
        items[index1] = items[index2];
        items[index2] = temp;
    }
}
