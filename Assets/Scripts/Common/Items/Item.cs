using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;

public class Item
{
    public bool TwoHanded { get; }

    private List<Action> actions;

    public IEnumerable<Action> Actions { get { return actions; } }

    public string Name { get; }

    public ItemTextureManager.Type Texture { get; }

    public enum Rarity
    {
        Common = 1,
        Uncommon = 2,
        Rare = 3,
        VeryRare = 4,
        Legendary = 5,
    }

    public Rarity ItemRarity { get; }

    public Item (string name, ItemTextureManager.Type texture, Rarity rarity, bool twoHanded, params Action[] actions)
    {
        ItemRarity = rarity;
        this.Name = name;
        this.Texture = texture;
        this.actions = actions.ToList();
        TwoHanded = twoHanded;
    }

    public void AddAction (Action action)
    {
        actions.Add(action);
    }

    public string GetDescription ()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<b><u>");
        sb.Append(Name.ToUpper());
        sb.Append("</b></u> (");
        sb.Append(ItemRarity);
        sb.Append(")\n\n");
        foreach(Action action in actions)
        {
            sb.Append(action.GetDescription());
            sb.Append("\n\n");
        }
        return sb.ToString();
    }

    public static string RarityName (Rarity rarity)
    {
        switch(rarity)
        {
            case Rarity.Common:
                return "Common";
            case Rarity.Uncommon:
                return "Uncommon";
            case Rarity.Rare:
                return "Rare";
            case Rarity.VeryRare:
                return "Very Rare";
            case Rarity.Legendary:
                return "Legendary";
            default:
                return "Unknown";
        }
    }

    public Color GetColor ()
    {
        return GetColor(ItemRarity);
    }

    public static Color GetColor (Rarity rarity)
    {
        switch(rarity)
        {
            case Rarity.Common:
                return new Color(.7f, .7f, .7f);
            case Rarity.Uncommon:
                return new Color(.9f, .86f, .3f);
            case Rarity.Rare:
                return new Color(.96f, .38f, .38f);
            case Rarity.VeryRare:
                return new Color(.51f, .95f, .38f);
            case Rarity.Legendary:
                return new Color(.847f, .5f, .92f);
            default:
                return new Color(1, 1, 1);
        }
    }
}
