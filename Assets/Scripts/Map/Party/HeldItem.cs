using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class HeldItem : MonoBehaviour
{
    public static HeldItem Instance { get; private set; }

    private Item item;
    private ItemSlot fromSlot;


    private RectTransform rectTransform;

    [SerializeField]
    private SpriteRenderer itemRenderer;

    private void Awake ()
    {
        Instance = this;
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start ()
    {
        UpdateSprite();
    }

    public void Pickup (Item item, ItemSlot fromSlot)
    {
        this.item = item;
        this.fromSlot = fromSlot;
        UpdateSprite();
    }

    public Item Drop ()
    {
        Item temp = item;
        item = null;
        fromSlot = null;
        UpdateSprite();
        return temp;
    }

    public void Swap (ItemSlot slot)
    {
        if(fromSlot == null)
        {
            return;
        }
        else
        {
            fromSlot.Empty();
            fromSlot.Put(slot.GetItem());

            slot.Empty();
            slot.Put(item);
            fromSlot = null;
            item = null;
        }
        UpdateSprite();
    }

    public bool HasItem ()
    {
        return item != null;
    }

    private void LateUpdate ()
    {
        if(item != null)
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 pos = new Vector2(mousePos.x / Screen.width, mousePos.y / Screen.height);

            rectTransform.anchorMin = pos;
            rectTransform.anchorMax = pos;

            if(Input.GetMouseButtonUp(0))
            {
                Reset();
            }
        }
    }

    private void Reset ()
    {
        if(item != null)
        {
            fromSlot.Put(item);
            item = null;
        }
        UpdateSprite();
    }

    private void UpdateSprite ()
    {
        if(item == null)
        {
            itemRenderer.sprite = null;
        }
        else
        {
            itemRenderer.sprite = ItemTextureManager.Instance.Get(item.Texture);
            itemRenderer.color = item.GetColor();
        }
    }
}
