using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TurnInterface : MonoBehaviour
{

    [SerializeField]
    private Transform turnIconPrefab;

    [SerializeField]
    private int maxIconCount = 5;

    [SerializeField]
    private Vector2 leftPosition;

    [SerializeField]
    private Vector2 rightPosition;

    private Dictionary<Agent, TurnIcon> agentIcons = new Dictionary<Agent, TurnIcon>();


    private bool initialised = false;


    [SerializeField]
    private float transitionTime = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        TurnManager.Instance.OrderUpdated += (sender, eventArgs) => UpdateOrder(eventArgs.Order, eventArgs.LastOrder);
    }

    private void UpdateOrder (IEnumerable<Agent> newOrder, IEnumerable<Agent> lastOrder)
    {
        if(newOrder == null)
        {
            return;
        }        
        List<Agent> newOrderList = newOrder.ToList();

        if(lastOrder != null)
        {
            List<Agent> lastOrderList = lastOrder.ToList();

            if(newOrderList.Count < lastOrderList.Count)
            {
                foreach(var agent in lastOrderList.Except(newOrderList))
                {
                    if(agentIcons.TryGetValue(agent, out TurnIcon icon))
                    {
                        icon.SetIndex(-1, newOrderList.Count);
                    }
                }
            }
        }

        float duration = initialised ? transitionTime : 0;
        initialised = true;

        CreateAnyNewAgents(newOrderList);

        int count = Mathf.Min(maxIconCount, newOrderList.Count); 

        for(int i = 0; i < newOrderList.Count; i++)
        {
            var icon = agentIcons[newOrderList[i]];

            icon.SetIndex(i < maxIconCount ? i : -1, count, transitionTime);
        }
    }

    public Vector2 GetIconIndexPosition (int index)
    {
        Vector2 delta = rightPosition - leftPosition;

        Vector2 increments = delta / maxIconCount;

        return leftPosition + index * increments;
    }

    private void CreateAnyNewAgents (IEnumerable<Agent> agents)
    {
        foreach(var agent in agents)
        {
            if(!agentIcons.ContainsKey(agent))
            {
                TurnIcon newIcon = Instantiate(turnIconPrefab, transform).GetComponent<TurnIcon>();
                
                if(newIcon == null)
                {
                    Debug.LogError("Turn Icon prefab has no turn icon component attached.");
                    return;
                }

                newIcon.Initialize(agent, this);

                agentIcons.Add(agent, newIcon);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
