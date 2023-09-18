using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class NextTurnButton : MonoBehaviour
{
    private Button button;

    private void Awake ()
    {
        button = GetComponent<Button>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        button.interactable = IsActive();
    }

    public void Press ()
    {
        if(IsActive())
        {
            TurnManager.Instance.IncrementTurn();
        }
    }

    private bool IsActive ()
    {
        var currentAgent = TurnManager.Instance.CurrentAgent;

        return currentAgent != null && currentAgent.IsPlayerControlled;
    }

    private void OnMouseOver ()
    {
        ActionTooltip.Instance.Show("Next Turn");
    }
}
