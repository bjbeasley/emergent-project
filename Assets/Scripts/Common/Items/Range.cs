using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Range
{
    public enum RangeMode
    {
        None = 0,
        Self = 1,
        Line = 2,
    }

    public RangeMode Mode { get; }

    public int Orthogonal { get; }
    public int Diagonal { get; }

    private static readonly Vector2Int[] adjacencies = new Vector2Int[8]
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

    public static Range Self { get; } = new Range(RangeMode.Self);
    public static Range Melee { get; } = new Range(RangeMode.Line, 1, 1);

    public Range (RangeMode mode, int orthogonal = 0, int diagonal = 0)
    {
        Mode = mode;
        Orthogonal = orthogonal;
        Diagonal = diagonal;
    }

    public bool CanTarget (Agent from, Agent target, Vector2Int? alternativeFromPosition = null)
    {
        if(Mode == RangeMode.None)
        {
            return false;
        }

        if(Mode == RangeMode.Self)
        {
            return from == target;
        }

        Vector2Int start = GetStartPos(from, alternativeFromPosition);

        if(Mode == RangeMode.Line)
        {
            Vector2Int delta = target.GridPos - start;

            bool isDiagonal = (Mathf.Abs(delta.x) == Mathf.Abs(delta.y));
            bool isOrthogonal = delta.x == 0 || delta.y == 0;

            int dist = Mathf.Max(Mathf.Abs(delta.x), Mathf.Abs(delta.y));

            if((!isDiagonal && !isOrthogonal) 
                || ((isDiagonal ? Diagonal : Orthogonal) < dist))
            {
                return false;
            }

            Vector2Int dir = GetDirection(delta);

            return CastLine(start, dir, dist + 1, from) == target;
        }

        return false;
    }

    public IEnumerable<Agent> GetTargetAgents (Agent from, Vector2Int? alternativeFromPosition = null)
    {
        if(Mode == RangeMode.None)
        {
            yield break;
        }
        else if(Mode == RangeMode.Self)
        {
            yield return from;
        }
        else if (Mode == RangeMode.Line)
        {
            foreach(var target in GetLineTargetAgents(from, alternativeFromPosition))
            {
                yield return target;
            }
        }
    }

    private IEnumerable<Agent> GetLineTargetAgents (Agent from, Vector2Int? alternativeFromPosition = null)
    {
        Vector2Int start = GetStartPos(from, alternativeFromPosition);

        for(int i = 0; i < adjacencies.Length; i++)
        {
            Vector2Int dir = adjacencies[i];
            int range = i % 2 == 0 ? Orthogonal : Diagonal;

            for(int j = 1; j <= range; j++)
            {
                Vector2Int pos = start + dir * j;

                if(CombatMap.Instance.Grid.InGrid(pos))
                {
                    var cell = CombatMap.Instance.Grid.Get(pos);

                    if(cell.IsCover(from))
                    {
                        if(cell.Agent != null && cell.Agent.Friendly != from.Friendly)
                        {
                            yield return cell.Agent;
                        }
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }

    public IEnumerable<Vector2Int> GetTargetPositions (Agent from, Vector2Int? alternativeFromPosition = null)
    {
        if(Mode == RangeMode.None || Mode == RangeMode.Self)
        {
            yield break;
        }
        else if(Mode == RangeMode.Line)
        {
            foreach(var target in GetLineTargetPositions(from, alternativeFromPosition))
            {
                yield return target;
            }
        }
    }

    public IEnumerable<Vector2Int> GetLineTargetPositions (Agent from, Vector2Int? alternativeFromPosition = null)
    {
        Vector2Int start = GetStartPos(from, alternativeFromPosition);

        for(int i = 0; i < adjacencies.Length; i++)
        {
            Vector2Int dir = adjacencies[i];
            int range = i % 2 == 0 ? Orthogonal : Diagonal;

            for(int j = 1; j <= range; j++)
            {
                Vector2Int pos = start + dir * j;

                if(CombatMap.Instance.Grid.InGrid(pos))
                {
                    var cell = CombatMap.Instance.Grid.Get(pos);

                    if(cell.IsCover(from))
                    {                        
                        break;
                    }
                    else
                    {
                        yield return pos;
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }

    private Vector2Int GetStartPos (Agent from, Vector2Int? alternativeFromPosition = null)
    {
        return alternativeFromPosition == null ? from.GridPos : alternativeFromPosition.Value;
    }

    private Agent CastLine (Vector2Int start, Vector2Int dir, int distance, Agent ignoreAgent = null)
    {
        for(int i = 1; i < distance; i++)
        {
            var cell = CombatMap.Instance.Grid.Get(start + dir * i);
            if(cell.IsCover(ignoreAgent))
            {
                return cell.Agent;
            }
        }
        return null;
    }

    private Vector2Int GetDirection (Vector2Int vec)
    {
        return new Vector2Int(Sign(vec.x), Sign(vec.y));
    }

    private int Sign (int x)
    {
        return x == 0 ? 0 : (x > 0 ? 1 : -1);
    }

    public string GetDescription ()
    {
        switch(Mode)
        {
            case RangeMode.None:
                return "None";
            case RangeMode.Self:
                return "Self";
            case RangeMode.Line:
                return Orthogonal + " <i>(orthogonal)</i> / " + Diagonal + " <i>(diagonal)</i>";
            default:
                return "ErrorUnknown";
        }
    }

    public static Range Ranged (int orthogonal, int diagonal)
    {
        return new Range(RangeMode.Line, orthogonal, diagonal);
    }

}

