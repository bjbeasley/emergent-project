using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AgentPathfinder
{
    private DataGrid<Vector2Int> previousCell;
    private DataGrid<float> distance;

    private Stack<Vector2Int> nodesToExpand = new Stack<Vector2Int>();

    private static readonly Vector2Int nullVec = new Vector2Int(-1, -1);

    private readonly float diagonalPenalty = 1000;//0.5f;//1000;// Mathf.Sqrt(2) - 1;

    private Agent agent;

    private float maxDistance;

    private Vector2Int[] adjacencies = new Vector2Int[8]
    {
        new Vector2Int(+1,+0),
        new Vector2Int(+1,+1),
        new Vector2Int(+0,+1),
        new Vector2Int(-1,+1),
        new Vector2Int(-1,+0),
        new Vector2Int(-1,-1),
        new Vector2Int(+0,-1),
        new Vector2Int(+1,-1),
    };

    public AgentPathfinder (int width, int height, Agent agent)
    {
        this.agent = agent;
        previousCell = new DataGrid<Vector2Int>(width, height);
        distance = new DataGrid<float>(width, height);
    }

    public void Update ()
    {
        CalculateWalkableTiles(agent.GridPos, agent.AvailableMovement, agent);
    }   


    public IEnumerable<Vector2Int> GetReachableCells ()
    {
        foreach(var cell in previousCell.GetIndices())
        {
            if(previousCell.Get(cell) != nullVec && CombatMap.Instance.SpaceAvailable(cell))
            {
                yield return cell;
            }
        }
    }

    public void CalculateWalkableTiles (Vector2Int start, float maxDistance, Agent agent)
    {
        this.maxDistance = maxDistance;
        previousCell.SetAll(nullVec);
        distance.SetAll(maxDistance + 1);
        nodesToExpand.Clear();

        distance.Set(start, 0);
        nodesToExpand.Push(start);

        while(nodesToExpand.Count != 0)
        {
            Vector2Int pos = nodesToExpand.Pop();

            int minX = Mathf.Max(pos.x - 1, 0);
            int maxX = Mathf.Min(pos.x + 1, distance.Width - 1);

            int minY = Mathf.Max(pos.y - 1, 0);
            int maxY = Mathf.Min(pos.y + 1, distance.Height - 1);

            float dist = distance.Get(pos);

            for(int x = minX; x <= maxX; x++)
            {
                for(int y = minY; y <= maxY; y++)
                {
                    float newDist = dist + 1 + (x == pos.x || y == pos.y ? 0 : diagonalPenalty);

                    if(distance.Get(x, y) > newDist
                        && CombatMap.Instance.SpacePassable(new Vector2Int(x, y), agent))
                    {
                        previousCell.Set(x, y, pos);

                        distance.Set(x, y, newDist);

                        nodesToExpand.Push(new Vector2Int(x, y));
                    }
                }
            }
        }
    }

    public bool CellReachable (Vector2Int pos)
    {
        return distance.Get(pos) <= maxDistance && CombatMap.Instance.SpaceAvailable(pos);
    }

    public float GetDistance (Vector2Int pos)
    {
        if(!CellReachable(pos))
        {
            throw new IndexOutOfRangeException("Position not reachable: " + pos.ToString());
        }

        return distance.Get(pos);
    }

    public IEnumerable<Vector2Int> GetPath (Vector2Int target)
    {
        if(!CellReachable(target)) return null;

        List<Vector2Int> points = new List<Vector2Int>();

        Vector2Int current = target;

        while(current != nullVec)
        {
            points.Add(current);
            current = previousCell.Get(current);
        }

        points.Reverse();

        return points;
    }

}
