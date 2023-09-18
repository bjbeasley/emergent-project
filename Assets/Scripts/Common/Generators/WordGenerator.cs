using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


[CreateAssetMenu(fileName = "WordGenerator", menuName = "ScriptableObjects/WordGenerator", order = 1)]
public class WordGenerator : ScriptableObject
{
    

    [SerializeField]
    public TextAsset baseWordsCSV;

    private List<string> baseWords;

    [SerializeField]
    public TextAsset substitutionsCSV;

    private Dictionary<string, List<string>> substitutions;
    
    private void UpdateBaseWords ()
    {
        if(substitutionsCSV == null)
        {
            return;
        }
        string baseData = baseWordsCSV.text;
        string[] lines = baseData.Split('\n');

        baseWords = new List<string>();

        for(int i = 0; i < lines.Length; i++)
        { 
            baseWords.Add(lines[i].Trim().ToLower());
        }
    }

    private void UpdateSubstitutions ()
    {
        if(substitutionsCSV == null)
        {
            return;
        }
        string subData = substitutionsCSV.text;

        string[] lines = subData.Split('\n');

        substitutions = new Dictionary<string, List<string>>();

        for(int i = 0; i < lines.Length; i++)
        {
            string[] split = lines[i].Split(',');

            if(split.Length > 1)
            {
                string key = split[0].Trim().ToLower();
                string value = split[1].Trim().ToLower();
                if(substitutions.TryGetValue(key, out List<string> list))
                {
                    list.Add(value);
                }
                else
                {
                    substitutions.Add(key, new List<string> { value });
                }
            }
        }
    }

    private void OnValidate ()
    {
        Debug.Log("Word Generator Validated");
        UpdateBaseWords();
        UpdateSubstitutions();
    }

    private void Awake ()
    {
        Debug.Log("Word Generator Awoken");
        UpdateBaseWords();
        UpdateSubstitutions();
    }

    public string GetWord ()
    {
        string word = GetRandomBaseWord();        

        for(int i = 0; i < Random.Range(1, 3); i++)
        {
            word = ApplyRandomSubstitution(word);
        }

        return word;
    }

    private string GetRandomBaseWord ()
    {
        int baseIndex = Random.Range(0, baseWords.Count);

        return baseWords[baseIndex];
    }

    private string ApplyRandomSubstitution (string word)
    {

        List<string> substrings = new List<string>();

        for(int subSize = 1; subSize < word.Length; subSize++)
        {
            for(int index = 0; index < word.Length - subSize + 1; index++)
            {
                substrings.Add(word.Substring(index, subSize));
            }
        }

        int remainingTries = 20;

        while(substrings.Count > 0 && remainingTries > 0)
        {
            remainingTries--;
            int index = Random.Range(0, substrings.Count);
            if(substitutions.TryGetValue(substrings[index], out List<string> options))
            {
                int index2 = Random.Range(0, options.Count);
                return word.Replace(substrings[index], options[index2]);
            }
            substrings.RemoveAt(index);
        }

        return word;
    }





}

