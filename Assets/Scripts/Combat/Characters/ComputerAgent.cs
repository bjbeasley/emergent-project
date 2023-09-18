using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ComputerAgent : Agent
{
    public override bool Friendly { get { return friendly; } }
    private bool friendly = false;

    public override bool IsPlayerControlled { get { return false; } }

    protected override void OnTurnStarted ()
    {
        base.OnTurnStarted();

        StartCoroutine(Turn());
    }

    public void SetFriendly (bool value)
    {
        friendly = value;
    }

    private void RandomMove ()
    {
        var cells = Pathfinder.GetReachableCells().ToList();

        if(cells.Count > 0)
        {
            int index = Random.Range(0, cells.Count);

            TryMove(cells[index]);
        }

        
    }

    private void BestMove ()
    {
        
    }

    IEnumerator Turn ()
    {
        yield return new WaitForSecondsRealtime(1.01f);

         var (move, actionTarget) = CombatMap.Instance.AI.GetBestMove(this);

        TryMove(move);

        if(actionTarget != null)
        {
            SelectAction(actionTarget.Action);
        }

        yield return new WaitForSecondsRealtime(1.01f);

        if(actionTarget != null && actionTarget.Target != null)
        {
            PerformSelectedAction(actionTarget.Target);
        }

        

        if(IsTurn)
        {
            TurnManager.Instance.IncrementTurn();
        }
    }
}
