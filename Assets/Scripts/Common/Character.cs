using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
    public Character (int maxHP, CharacterName name, Portrait portrait, int alertness, int movementSpeed)
    {
        MaxHP = maxHP;
        HP = maxHP;
        Name = name;
        Portrait = portrait;
        Alertness = alertness;
        Movement = movementSpeed;
    }

    public int MaxHP { get; set; }
    public int HP { get; set; }

    public bool Awake { get { return HP > 0; } }

    public int MaxShieldPoints { get; set; } = 3;

    public int ShieldPoints { get; set; }
    public int TempShieldPoints { get; set; }

    public int TotalShieldPoints { get { return ShieldPoints + TempShieldPoints; } }

    public int Alertness { get; set; }

    public int Movement { get; set; } = 4;

    public CharacterName Name { get; set; }

    public Portrait Portrait { get; set; }

    public Item Item { get; set; }

    public IEnumerable<Action> Actions { get { return Item != null ? Item.Actions : new List<Action>(); } }
}
