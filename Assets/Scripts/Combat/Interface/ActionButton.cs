using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class ActionButton : MonoBehaviour
{
    private RectTransform rectTransform;

    private Action action;
    private Agent agent;

    [SerializeField]
    private RawImage iconImage;
    [SerializeField]
    private RawImage ringImage;

    [SerializeField]
    private Texture attackIcon;
    [SerializeField]
    private Texture shieldIcon;
    [SerializeField]
    private Texture cancelIcon;

    [SerializeField]
    private Color clickColor = Color.white;

    [SerializeField]
    private Color hoverColor = Color.white;

    [SerializeField]
    private Color disabledColor = Color.grey;

    private int recentClicks = 0;

    private bool hovered = false;

    public enum Icon
    {
        Attack = 1,
        Shield = 2,
        Cancel = 3,
    }

    private void Awake ()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetPosition (Vector2 position)
    {
        rectTransform.anchorMin = position;
        rectTransform.anchorMax = position;
        //rectTransform.localPosition = Vector2.zero;
    }


    public void SetVisible (bool value)
    {
        gameObject.SetActive(value);
    }

    public void SetAction (Action action, Agent agent)
    {
        this.action = action;
        this.agent = agent;
    }

    public void Press ()
    {
        if(!IsActiveButton())
        {
            return;
        }

        StartCoroutine(ClickHighlightCoroutine());

        if(agent.SelectedAction != action)
        {
            agent.SelectAction(action);
        }
        else
        {
            agent.CancelAction();
        }
        
    }


    private IEnumerator ClickHighlightCoroutine ()
    {
        recentClicks += 1;
        yield return new WaitForSecondsRealtime(0.1f);
        recentClicks -= 1;
    }

    private void Update ()
    {
        UpdateIcon();
        UpdateColor();
    }

    private void UpdateColor ()
    {
        if(action == null)
        {
            return;
        }
        Color color = action.Effect.Color;

        bool clicked = recentClicks > 0;

        if(IsActiveButton())
        {
            color = Color.Lerp(color, clickColor, clicked ? clickColor.a : 0);
            iconImage.color = new Color(1f, 1f, 1f, 0.7f + (clicked ? 0.3f : 0));      
            
            if(hovered)
            {
                color = Color.Lerp(color, hoverColor, hoverColor.a);
                hovered = false;
            }

        }
        else
        {
            color = Color.Lerp(color, disabledColor, disabledColor.a);
            color = Color.Lerp(color, clickColor, clicked ? clickColor.a : 0);
            iconImage.color = new Color(1f, 1f, 1f, 0.2f);
        }

        ringImage.color = color;
        iconImage.color = color;
    }

    private bool IsActiveButton ()
    {
        return agent.IsPlayerControlled && agent.ActionsRemaining > 0 && agent.IsTurn;
    }

    private void UpdateIcon ()
    {
        if(agent == null || action == null)
        {
            return;
        }

        Texture texture = null;

        if(agent.SelectedAction == action)
        {
            texture = cancelIcon;
        }
        else
        {
            switch(action.Effect.Icon)
            {
                case Icon.Attack:
                    texture = ItemTextureManager.Instance.Get(agent.Character.Item.Texture).texture;
                    break;
                case Icon.Shield:
                    texture = ItemTextureManager.Instance.Get(agent.Character.Item.Texture).texture;
                    break;
                case Icon.Cancel:
                    texture = cancelIcon;
                    break;
                default:
                    break;
            }
        }

        iconImage.texture = texture;
    }

    private void OnMouseOver ()
    {
        hovered = true;
        ActionTooltip.Instance.Show(action.GetDescription());
    }
}
