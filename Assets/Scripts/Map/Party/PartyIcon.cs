using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class PartyIcon : MonoBehaviour
{

    [SerializeField]
    private PortraitToken token;

    private Character character;
    private RectTransform rectTransform;

    [SerializeField]
    private ItemSlot itemSlot;

    private void Awake ()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void AssignCharacter (Character character)
    {
        this.character = character;
        token.AssignPortrait(character, true);
        itemSlot.SetLocation(character);
    }

    public void SetPosition (Vector2 pos)
    {
        rectTransform.anchorMin = pos;
        rectTransform.anchorMax = pos;
        rectTransform.anchoredPosition = Vector2.zero;
        var position = rectTransform.localPosition;
        position.z = 100;
        rectTransform.localPosition = position;
        rectTransform.localScale = new Vector3(1, 1, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseOver ()
    {
        ActionTooltip.Instance.Show(character.Name.First 
            + "\nAlertness: " + character.Alertness 
            + "\nShield Points: " + character.MaxShieldPoints
            + "\nMovment Speed: " + character.Movement);
        if(Input.GetKeyDown(KeyCode.K))
        {
            character.HP = 0;
        }
    }
}
