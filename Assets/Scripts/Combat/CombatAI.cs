using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatAI
{ 
    private DataGrid<AICell> cells;
    private AgentPathfinder pathfinder;

    // Start is called before the first frame update
    public CombatAI()
    {        
        cells = new DataGrid<AICell>(CombatMap.Instance.Grid.Width, CombatMap.Instance.Grid.Height);
        pathfinder = new AgentPathfinder(cells.Width, cells.Height, null);
        foreach(var index in cells.GetIndices())
        {
            cells.Set(index, new AICell());
        }
    }

    public (Vector2Int, ActionTarget) GetBestMove (Agent agent)
    {
        Clear();

        //Find all reachable cells
        List<Vector2Int> reachable = agent.Pathfinder.GetReachableCells().ToList();
        reachable.Add(agent.GridPos);

        //Find each of these that can be targetted 


        //Find best attack for each of these
        foreach(Vector2Int cell in reachable)
        {
            FindBestAction(cell, agent);
            CalculatePositionScore(cell, agent);
        }

        //Evaluate each cell using a combination of the defensiveness and offensiveness defined by a character trait/team trait

        float offensiveness = 1;

        var bestMove = reachable.OrderByDescending(c => cells.Get(c).GetScore(offensiveness, 1)).First();

        return (bestMove, cells.Get(bestMove).BestTarget);
    }

    private void FindBestAction (Vector2Int cell, Agent agent)
    {

        float highscore = 0;
        ActionTarget best = null;

        foreach(Action action in agent.Character.Actions)
        {
            var targets = action.Range.GetTargetAgents(agent, cell);

            foreach(Agent target in targets)
            {
                float efficacy = action.Effect.GetEfficacy(agent, target);

                if(efficacy > highscore)
                {
                    highscore = efficacy;
                    best = new ActionTarget(action, target);
                }
            }
        }

        var aiCell = cells.Get(cell);

        aiCell.BestTarget = best;
        aiCell.ActionScore = highscore;
    }

    private readonly Vector2Int[] adjacencies = new Vector2Int[] { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down };

    private void CalculatePositionScore (Vector2Int cell, Agent agent)
    {
        float score = 0;

        for(int i = 0; i < adjacencies.Length; i++)
        {
            Vector2Int neighbour = cell + adjacencies[i];
            if(CombatMap.Instance.Grid.InGrid(neighbour))
            {
                Agent neighbourAgent = CombatMap.Instance.Grid.Get(neighbour).Agent;
                if(neighbourAgent != null && agent.Friendly == neighbourAgent.Friendly && neighbourAgent.Character.HP > 0)
                {
                    score++;
                }
            }
        }

        pathfinder.CalculateWalkableTiles(cell, 12, agent);
        var reachable = pathfinder.GetReachableCells();

        float minDist = 100;

        foreach(Vector2Int reachableCell in reachable)
        {
            for(int i = 0; i < adjacencies.Length; i++)
            {
                Vector2Int neighbour = reachableCell + adjacencies[i];
                if(CombatMap.Instance.Grid.InGrid(neighbour))
                {
                    Agent neighbourAgent = CombatMap.Instance.Grid.Get(neighbour).Agent;
                    if(neighbourAgent != null && agent.Friendly != neighbourAgent.Friendly && neighbourAgent.Character.HP > 0)
                    {
                        float dist = pathfinder.GetDistance(reachableCell);
                        if(minDist > dist)
                        {
                            minDist = dist;
                        }
                    }
                }
            }
        }

        cells.Get(cell).PositionScore = (score - minDist) * 0.001f;
    }

    private void Clear ()
    {
        foreach(var cell in cells)
        {
            cell.Clear();
        }
    }
}
