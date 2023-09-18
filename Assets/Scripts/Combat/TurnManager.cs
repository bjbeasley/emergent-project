using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    private List<Agent> turnOrder = new List<Agent>();

    private int turnIndex = -1;

    [SerializeField]
    private TurnInterface turnInterface;

    public IEnumerable<Agent> currentOrder;

    public event EventHandler<OrderUpdatedArgs> OrderUpdated;

    public Agent CurrentAgent { get { return (turnIndex < turnOrder.Count && turnIndex >= 0) ? turnOrder[turnIndex] : null; } }

    private void Awake ()
    {
        if(Instance != null)
        {
            Debug.LogError("Multiple turn managers detected");
        }
        Instance = this;
    }


    private void UpdateOrder ()
    {
        if(turnIndex == -1)
        {
            if(currentOrder != null)
            {
                OrderUpdated?.Invoke(this, new OrderUpdatedArgs(null, currentOrder));
                currentOrder = null;
            }
            return;
        }

        List<Agent> newOrder = new List<Agent>();

        for(int i = 0; i < turnOrder.Count; i++)
        {
            int index = (turnIndex + i) % turnOrder.Count;

            newOrder.Add(turnOrder[index]);
        }

        OrderUpdated?.Invoke(this, new OrderUpdatedArgs(newOrder, currentOrder));
        currentOrder = newOrder;

        if(currentOrder.All(t => t.Friendly) || currentOrder.All(t => !t.Friendly))
        {
            Storyteller.Instance.EndCombat(CurrentAgent.Friendly);
        }
    }

    public void RegisterAgent (Agent agent)
    {
        if(!agent.Character.Awake)
        {
            return;
        }
        turnOrder.Add(agent);
        turnOrder = turnOrder.OrderByDescending(a => a.Character.Alertness + UnityEngine.Random.value / 2).ToList();
        UpdateOrder();
    }

    public void RemoveAgent (Agent agent)
    {
        int index = turnOrder.IndexOf(agent);
        if(index < 0)
        {
            return;
        }
        if(index < turnIndex)
        {
            turnIndex--;
        }
        turnOrder.RemoveAt(index);
        UpdateOrder();
    }


    public void IncrementTurn ()
    {
        if(turnOrder.Count == 0)
        {
            return;
        }
        if(CurrentAgent != null)
        {
            turnOrder[turnIndex].OnTurnEnd();
        }

        turnIndex = (turnIndex + 1) % turnOrder.Count;

        turnOrder[turnIndex].OnTurnStart();
        CombatMap.Instance.SetSelected(turnOrder[turnIndex]);
        UpdateOrder();
    }

    // Update is called once per frame
    void Update()
    {
        if(CurrentAgent == null)
        {
            IncrementTurn();
        }
    }
}
