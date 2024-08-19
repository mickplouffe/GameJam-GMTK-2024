using System.Collections.Generic;

public class PriorityQueue<T>
{
    private List<(float, T)> elements = new();

    public int Count => elements.Count;

    public void Enqueue(T item, float priority)
    {
        elements.Add((priority, item));
        int c = elements.Count - 1;
        while (c > 0 && elements[c].Item1 < elements[(c - 1) / 2].Item1)
        {
            (elements[c], elements[(c - 1) / 2]) = (elements[(c - 1) / 2], elements[c]);
            c = (c - 1) / 2;
        }
    }

    public T Dequeue()
    {
        int li = elements.Count - 1;
        var frontItem = elements[0];
        elements[0] = elements[li];
        elements.RemoveAt(li);

        --li;
        int i = 0;
        while (true)
        {
            int ci = i * 2 + 1;
            if (ci > li) break;
            int rc = ci + 1;
            if (rc <= li && elements[rc].Item1 < elements[ci].Item1)
                ci = rc;
            if (elements[i].Item1 <= elements[ci].Item1) break;
            (elements[i], elements[ci]) = (elements[ci], elements[i]);
            i = ci;
        }
        return frontItem.Item2;
    }

    public T Peek()
    {
        return elements[0].Item2;
    }
}