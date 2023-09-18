using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class ItemGenerator
{
    public static Item GetSword (Item.Rarity rarity)
    {
        int hp, sp, ignoreSP;

        switch(rarity)
        {
            case Item.Rarity.Common:
                hp = 1;
                sp = 1;
                ignoreSP = 1;
                break;
            case Item.Rarity.Uncommon:
                hp = 2;
                sp = 1;
                ignoreSP = 1;
                break;
            case Item.Rarity.Rare:
                hp = 2;
                sp = 1;
                ignoreSP = 2;
                break;
            case Item.Rarity.VeryRare:
                hp = 3;
                sp = 1;
                ignoreSP = 2;
                break;
            case Item.Rarity.Legendary:
                hp = 4;
                sp = 2;
                ignoreSP = 2;
                break;
            default:
                hp = 1;
                sp = 1;
                ignoreSP = 1;
                break;
        }

        Item item = new Item("Shortsword", ItemTextureManager.Type.Shortsword, rarity, false, GetAttack("Quick Slash", hp, sp, ignoreSP, 1, 0));
        item.AddAction(GetParry(1));

        return item;
    }

    public static Item GetLongSword (Item.Rarity rarity)
    {
        int hp, sp, ignoreSP;

        switch(rarity)
        {
            case Item.Rarity.Common:
                hp = 2;
                sp = 1;
                ignoreSP = 0;
                break;
            case Item.Rarity.Uncommon:
                hp = 2;
                sp = 1;
                ignoreSP = 0;
                break;
            case Item.Rarity.Rare:
                hp = 3;
                sp = 2;
                ignoreSP = 0;
                break;
            case Item.Rarity.VeryRare:
                hp = 3;
                sp = 2;
                ignoreSP = 0;
                break;
            case Item.Rarity.Legendary:
                hp = 4;
                sp = 2;
                ignoreSP = 0;
                break;
            default:
                hp = 1;
                sp = 1;
                ignoreSP = 1;
                break;
        }

        Item item = new Item("Longsword", ItemTextureManager.Type.Longsword, rarity, false, GetAttack("Heavy Slash", hp, sp, ignoreSP, 1, 1));
        item.AddAction(GetParry(2));

        return item;
    }

    public static Item GetBow (Item.Rarity rarity, bool cross)
    {
        int hp, sp, ignoreSP, range;
        string prefix = "Light ";

        switch(rarity)
        {
            case Item.Rarity.Common:
                hp = 1;
                sp = 1;
                ignoreSP = 0;
                range = 4;
                break;
            case Item.Rarity.Uncommon:
                hp = 2;
                sp = 1;
                ignoreSP = 0;
                range = 5;
                prefix = "Normal ";
                break;
            case Item.Rarity.Rare:
                hp = 2;
                sp = 1;
                ignoreSP = 0;
                range = 6;
                prefix = "Heavy ";
                break;
            case Item.Rarity.VeryRare:
                hp = 3;
                sp = 1;
                ignoreSP = 0;
                range = 8;
                prefix = "Powerful ";
                break;
            case Item.Rarity.Legendary:
                hp = 5;
                sp = 1;
                ignoreSP = 0;
                range = 20;
                prefix = "Deadly ";
                break;
            default:
                hp = 1;
                sp = 1;
                ignoreSP = 0;
                range = 4;
                break;
        }

        Item item; 

        if(cross)
        {
            item = new Item("Crossbow", ItemTextureManager.Type.Crossbow, rarity, false, GetAttack(prefix + "Bolt", hp, sp, ignoreSP, 0, range));
        }
        else
        {
            item = new Item("Bow", ItemTextureManager.Type.Bow, rarity, false, GetAttack(prefix + "Arrow", hp, sp, ignoreSP, range, 0));
        }

        return item;
    }

    public static Item GetHatchet (Item.Rarity rarity)
    {
        int hp, sp, ignoreSP;
        string prefix = "";

        switch(rarity)
        {
            case Item.Rarity.Common:
                hp = 1;
                sp = 1;
                ignoreSP = 1;
                break;
            case Item.Rarity.Uncommon:
                hp = 1;
                sp = 1;
                ignoreSP = 2;
                prefix = "Accurate ";
                break;
            case Item.Rarity.Rare:
                hp = 2;
                sp = 1;
                ignoreSP = 2;
                prefix = "Precise ";
                break;
            case Item.Rarity.VeryRare:
                hp = 2;
                sp = 1;
                ignoreSP = 3;
                prefix = "Excact ";
                break;
            case Item.Rarity.Legendary:
                hp = 3;
                sp = 1;
                ignoreSP = 4;
                prefix = "Surgical ";
                break;
            default:
                hp = 1;
                sp = 1;
                ignoreSP = 1;
                break;
        }

        Item item = new Item("Hatchet", ItemTextureManager.Type.Hatchet, rarity, false, GetAttack(prefix + "Cut", hp, sp, ignoreSP, 1, 1));

        return item;
    }

    public static Item GetTowerShield (Item.Rarity rarity)
    {
        int hp;
        int sp;

        switch(rarity)
        {
            case Item.Rarity.Common:
                hp = 1;
                sp = 3;
                break;
            case Item.Rarity.Uncommon:
                hp = 1;
                sp = 4;
                break;
            case Item.Rarity.Rare:
                hp = 1;
                sp = 5;
                break;
            case Item.Rarity.VeryRare:
                hp = 1;
                sp = 6;
                break;
            case Item.Rarity.Legendary:
                hp = 1;
                sp = 10;
                break;
            default:
                hp = 1;
                sp = 1;
                break;
        }

        Item item = new Item("Tower Shield", ItemTextureManager.Type.TowerShield, rarity, false, 
            new Action("Defensive Stance", Range.Self, new TempShieldEffect(sp)), 
            GetAttack("Bash", hp, 1, 0, 1, 0));

        return item;
    }

    public static Item GetSpear (Item.Rarity rarity)
    {
        int hp, sp, ignoreSP;
        int range = 2;

        switch(rarity)
        {
            case Item.Rarity.Common:
                hp = 1;
                sp = 1;
                ignoreSP = 0;
                break;
            case Item.Rarity.Uncommon:
                hp = 1;
                sp = 2;
                ignoreSP = 0;
                break;
            case Item.Rarity.Rare:
                hp = 1;
                sp = 3;
                ignoreSP = 0;
                break;
            case Item.Rarity.VeryRare:
                hp = 2;
                sp = 4;
                ignoreSP = 0;
                break;
            case Item.Rarity.Legendary:
                hp = 2;
                sp = 6;
                ignoreSP = 0;
                range = 3;
                break;
            default:
                hp = 1;
                sp = 1;
                ignoreSP = 1;
                break;
        }

        Item item = new Item("Spear", ItemTextureManager.Type.Spear, rarity, false, GetAttack("Lunge", hp, sp, ignoreSP, range, 0));

        return item;
    }

    public static Item GetRandomWeapon (Item.Rarity rarity)
    {
        float value = Random.value;

        return value < 0.2f ? GetSword(rarity)
            : value < 0.3f ? GetLongSword(rarity)
            : value < 0.4f ? GetBow(rarity, true)
            : value < 0.5f ? GetBow(rarity, false)
            : value < 0.7f ? GetHatchet(rarity)
            : value < 0.8f ? GetTowerShield(rarity)
            : GetSpear(rarity);
    }

    public static Item GetRandomWeapon ()
    {
        float value = Random.value;

        Item.Rarity rarity =
              value < 0.5f ? Item.Rarity.Common
            : value < 0.75f ? Item.Rarity.Uncommon
            : value < 0.9f ? Item.Rarity.Rare
            : value < 0.98f ? Item.Rarity.VeryRare
            : Item.Rarity.Legendary;

        return GetRandomWeapon(rarity);
    }

    private static Action GetAttack (string name, int hp, int sp, int ignoreSP, int ortho, int diagonal)
    {
        return new Action(name, Range.Ranged(ortho, diagonal), new DamageEffect(sp, hp, ignoreSP));
    }

    private static Action GetParry (int points)
    {
        return new Action("Parry", Range.Self, new TempShieldEffect(points));
    }

}

