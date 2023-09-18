using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterGenerator", menuName = "ScriptableObjects/CharacterGenerator", order = 1)]
public class CharacterGenerator : ScriptableObject
{
    [SerializeField]
    private CharacterNameGenerator nameGenerator;

    [SerializeField]
    private PortraitProfile malePortraitProfile;
    [SerializeField]
    private PortraitProfile femalePortraitProfile;

    public Character GetCharacter (bool infected = false, int hp = -1, bool hasItem = true, Item.Rarity? itemRarity = null)
    {
        bool masculine = Random.value > 0.5f;
        var name = nameGenerator.GetName(masculine);
        var portrait = (masculine ? malePortraitProfile : femalePortraitProfile).GetPortrait(infected);

        hp = (hp > 0 ? hp : Random.Range(1, 5));

        Character character = new Character(hp, name, portrait, Random.Range(0, 100), Random.Range(3,6));

        if(hasItem)
        {
            if(itemRarity == null)
            {
                character.Item = ItemGenerator.GetRandomWeapon();
            }
            else
            {
                character.Item = ItemGenerator.GetRandomWeapon(itemRarity.Value);
            }
        }

        return character; 
    }
}
