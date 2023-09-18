using EmpiresCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot : MonoBehaviour
{
    private static Item[] inventory = new Item[6];

    public bool Available { get { return GetItem() == null; } }

    [SerializeField]
    private int inventoryLocation = -1;
    private Character characterLocation;

    [SerializeField]
    private SpriteRenderer itemRenderer;

    private static System.Action heldItemPickupAction;

    private void Start ()
    {
        DayCounter.Instance.PhaseIncremented += ClearHeldAction;
    }

    private void ClearHeldAction (object sender, EventArgs e)
    {
        heldItemPickupAction = null;
    }

    private void LateUpdate ()
    {
        UpdateSprite();
    }

    public static void PickupItem (Item item, Province source)
    {
        heldItemPickupAction = null;
        bool picked = false;

        for(int i = 0; i < inventory.Length; i++)
        {
            if(inventory[i] == null)
            {
                inventory[i] = item;
                i = inventory.Length + 1;
                picked = true;
            }
        }

        if(!picked)
        {
            DialogManager.Instance.CreateDialog("Inventory Full",
                "You have no spare slots in your inventory, you must delete an item before you next move if you want to pick up this one.",
                new DialogOption("Okay", () => HoldPickup(item, source)));
        }
        else
        {
            if(source != null)
            {
                ResourceManager.Instance.RemoveResource(source);
            }
        }        
    }

    private static void HoldPickup(Item item, Province province)
    {
        heldItemPickupAction = () => PickupItem(item, province);
    }

    public void Put (Item item)
    {
        if(!Available)
        {
            Debug.LogError("New item in slot has overwritten previous item.");
        }
        SetItem(item);
    }

    public void SetLocation (Character character)
    {
        characterLocation = character;
        inventoryLocation = -1;
    }

    public void SetLocation(int inventoryIndex)
    {
        if(inventoryIndex > inventory.Length || inventoryIndex < -1)
        {
            throw new IndexOutOfRangeException();
        }
        characterLocation = null;
        inventoryLocation = inventoryIndex;
    }

    public Item GetItem ()
    {
        if(characterLocation != null)
        {
            return characterLocation.Item;
        }
        if(inventoryLocation < 0)
        {
            return null;
        }
        return inventory[inventoryLocation];
    }

    private void SetItem (Item item)
    {
        if(characterLocation != null)
        {
            characterLocation.Item = item;
        }
        else if(inventoryLocation < 0)
        {
            //Do nothing, destroy item
            heldItemPickupAction?.Invoke();
        }
        else if(inventoryLocation < inventory.Length)
        {
            inventory[inventoryLocation] = item;
        }
        else
        {
            throw new IndexOutOfRangeException();
        }
        UpdateSprite();
    }

    public void Empty ()
    {
        SetItem(null);
    }

    private void OnMouseOver ()
    {
        Item item = GetItem();
        if(item != null)
        {
            ActionTooltip.Instance.Show(item.GetDescription());

            if(Input.GetMouseButtonDown(0))
            {
                HeldItem.Instance.Pickup(item, this);
                Empty();
            }
        }

        if(Input.GetMouseButtonUp(0))
        {
            if(HeldItem.Instance.HasItem())
            {
                if(Available)
                {
                    Put(HeldItem.Instance.Drop());
                }
                else
                {
                    HeldItem.Instance.Swap(this);
                }
                
            }
        }
    }

    private void UpdateSprite ()
    {
        Item item = GetItem();
        if(characterLocation == null && inventoryLocation < 0)
        {
            itemRenderer.sprite = ItemTextureManager.Instance.Get(ItemTextureManager.Type.Trash);
        }
        else if(item == null)
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
