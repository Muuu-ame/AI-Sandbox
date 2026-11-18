using System.Collections.Generic;
using UnityEngine;

public class SpatialHash
{
    private readonly float cellSize;

    private readonly Dictionary<Vector3Int, List<PhysicsBody>> cells = new();

    private readonly Stack<List<PhysicsBody>> listPool = new();

    public SpatialHash(float cellSize)
    {
        this.cellSize = Mathf.Max(0.01f, cellSize);
    }

    private Vector3Int GetCell(Vector3 position)
    {
        return new Vector3Int(
            Mathf.FloorToInt(position.x / cellSize),
            Mathf.FloorToInt(position.y / cellSize),
            Mathf.FloorToInt(position.z / cellSize)
        );
    }

    private List<PhysicsBody> GetList()
    {
        return listPool.Count > 0
            ? listPool.Pop()
            : new List<PhysicsBody>();
    }

    public void Clear()
    {
        foreach (var cell in cells.Values)
        {
            cell.Clear();
            listPool.Push(cell);
        }

        cells.Clear();
    }

    public void Insert(PhysicsBody body, Bounds bounds)
    {
        if (body == null) return;

        Vector3Int min = GetCell(bounds.min);
        Vector3Int max = GetCell(bounds.max);

        for (int x = min.x; x <= max.x; x++)
        {
            for (int y = min.y; y <= max.y; y++)
            {
                for (int z = min.z; z <= max.z; z++)
                {
                    Vector3Int key = new Vector3Int(x, y, z);

                    if (!cells.TryGetValue(key, out var list))
                    {
                        list = GetList();
                        cells.Add(key, list);
                    }

                    list.Add(body);
                }
            }
        }
    }

    public void Insert(PhysicsBody body)
    {
        if (body == null) return;

        PhysicsShape shape = body.GetComponent<PhysicsShape>();

        if (shape == null) return;

        Insert(body, shape.WorldBounds);
    }

    public void GetNearby(
        Bounds bounds,
        List<PhysicsBody> results
    )
    {
        results.Clear();

        Vector3Int min = GetCell(bounds.min);
        Vector3Int max = GetCell(bounds.max);

        foreach (var pair in cells)
        {
            Vector3Int key = pair.Key;

            if (key.x < min.x || key.x > max.x ||
                key.y < min.y || key.y > max.y ||
                key.z < min.z || key.z > max.z)
                continue;

            foreach (PhysicsBody body in pair.Value)
            {
                if (body != null && !results.Contains(body))
                {
                    results.Add(body);
                }
            }
        }
    }
}
