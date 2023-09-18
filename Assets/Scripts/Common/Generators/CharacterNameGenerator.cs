
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterNameGenerator", menuName = "ScriptableObjects/CharacterNameGenerator", order = 1)]
public class CharacterNameGenerator : ScriptableObject
{
    private System.Random random;

    [SerializeField]
    private WordGenerator masculineFirstNameGen;
    [SerializeField]
    private WordGenerator feminineFirstNameGen;

    public CharacterName GetName (bool masculine)
    {
        var gen = (masculine ? masculineFirstNameGen : feminineFirstNameGen);
        return new CharacterName(ToNameCase(gen.GetWord()), "s" + Random.Range(0,100).ToString(), "", "");
    }

    private string ToNameCase (string name)
    {
        return name[0].ToString().ToUpper() + name.Substring(1);
    }
}
