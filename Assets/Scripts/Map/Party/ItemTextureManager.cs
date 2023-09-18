using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTextureManager : MonoBehaviour
{
    public static ItemTextureManager Instance { get; private set; }

    public enum Type
    {
        Trash = 0,
        Bow = 1,
        Crossbow = 2,
        Hatchet = 3,
        Heal = 4,
        Longsword = 5,
        Scythe = 6,
        Shortsword = 7,
        Spear = 8,
        TowerShield = 9,
        Warhammer = 10,
    }

    [SerializeField]
    public List<Sprite> sprites;

    private void Awake ()
    {
        Instance = this;
    }

    public Sprite Get(Type type)
    {
        return sprites[(int)type];
    }

   
}
