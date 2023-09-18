using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionPanel : MonoBehaviour
{
    public static ActionPanel Instance { get; private set; }

    private const int minButtonSpaces = 8;
    private Agent currentAgent = null;

    private readonly int maxButtons = 10;

    private List<ActionButton> buttons;

    [SerializeField]
    private Vector2 leftPos;
    [SerializeField]
    private Vector2 rightPos;


    [SerializeField]
    private Transform actionButtonPrefab;

    [SerializeField]
    private RectTransform actionTooltip;

    private void Awake ()
    {
        if(Instance != null)
        {
            Debug.LogError("Multiple action panels detected!");
        }

        Instance = this;

        buttons = new List<ActionButton>();

        for(int i = 0; i < maxButtons; i++)
        {
            var button = Instantiate(actionButtonPrefab, transform).GetComponent<ActionButton>();
            buttons.Add(button);
        }
    }

    private void Update ()
    {
        if(CombatMap.Instance.SelectedAgent != currentAgent)
        {
            UpdateButtons();
        }
    }

    void UpdateButtons()
    {
        currentAgent = CombatMap.Instance.SelectedAgent;

        List<Action> actions = currentAgent.Character.Actions?.ToList();

        for(int i = 0; i < maxButtons; i++)
        {
            float spacingCount = Mathf.Max(minButtonSpaces, actions.Count - 1);

            if(i < actions?.Count)
            {
                buttons[i].SetVisible(true);
                buttons[i].SetAction(actions[i], CombatMap.Instance.SelectedAgent);
                buttons[i].SetPosition(Vector2.Lerp(leftPos, rightPos, i / spacingCount));
            }
            else
            {
                buttons[i].SetVisible(false);
            }

        }


    }
}
