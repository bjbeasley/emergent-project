using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataGrid<T> : IEnumerable<T>, IEnumerable
{
    public int Width { get; }
    public int Height { get; }

    protected T[,] data;

    public DataGrid (int width, int height)
    {
        this.Width = width;
        this.Height = height;

        data = new T[width, height];
    }

    public void Set(Vector2Int point, T value)
    {
        Set(point.x, point.y, value);
    }

    public void Set (int x, int y, T value)
    {
        if(!InGrid(x, y))
        {
            throw new IndexOutOfRangeException("Requested coords outside of grid: (" + x + ", " + y + ") not in [" + Width + " * " + Height + "]");
        }

        data[x, y] = value;
    }

    public T Get (Vector2Int point)
    {
        return Get(point.x, point.y);
    }

    public T Get (int x, int y)
    {
        if(!InGrid(x,y))
        {
            throw new IndexOutOfRangeException("Requested coords outside of grid: (" + x + ", " + y + ") not in [" + Width + " * " + Height + "]");
        }        
        return data[x, y];        
    }



    public IEnumerable<Vector2Int> GetIndices ()
    {
        for(int x = 0; x < Width; x++)
        {
            for(int y = 0; y < Height; y++)
            {
                yield return new Vector2Int(x, y);
            }
        }
    }

    public bool InGrid (Vector2Int point)
    {
        return InGrid(point.x, point.y);
    }

    public bool InGrid (int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    public IEnumerator<T> GetEnumerator ()
    {
        for(int x = 0; x < Width; x++)
        {
            for(int y = 0; y < Height; y++)
            {
                yield return data[x, y];
            }
        }
    }

    public void SetAll (T value)
    {
        for(int x = 0; x < Width; x++)
        {
            for(int y = 0; y < Height; y++)
            {
                data[x, y] = value;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
        return this.GetEnumerator();
    }

    public Vector2Int FindNearest (Vector2Int start, Func<T, bool> criteria)
    {
        DataGrid<bool> visited = new DataGrid<bool>(Width, Height);
        Queue<Vector2Int> nodesToExpand = new Queue<Vector2Int>();

        visited.SetAll(false);
        nodesToExpand.Clear();

        nodesToExpand.Enqueue(start);

        while(nodesToExpand.Count != 0)
        {
            Vector2Int pos = nodesToExpand.Dequeue();

            int minX = Mathf.Max(pos.x - 1, 0);
            int maxX = Mathf.Min(pos.x + 1, Width - 1);

            int minY = Mathf.Max(pos.y - 1, 0);
            int maxY = Mathf.Min(pos.y + 1, Height - 1);

            for(int x = minX; x <= maxX; x++)
            {
                for(int y = minY; y <= maxY; y++)
                {
                    if(!visited.Get(x,y))
                    {
                        var data = Get(x, y);
                        if(criteria.Invoke(data))
                        {
                            return new Vector2Int(x, y);
                        }

                        visited.Set(x, y, true);
                        nodesToExpand.Enqueue(new Vector2Int(x, y));
                    }
                }
            }
        }

        throw new Exception("No matching cell found");
    }

}
