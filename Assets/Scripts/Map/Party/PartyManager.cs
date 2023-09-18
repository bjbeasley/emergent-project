using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class PartyManager : MonoBehaviour
{

    [SerializeField]
    private Vector2 leftPosition;

    [SerializeField]
    private Vector2 rightPosition;

    [SerializeField]
    private Vector3 iconSize = new Vector3(100, 100, 100);

    [SerializeField]
    private Transform partyIconPrefab;

    private List<PartyIcon> tokens = new List<PartyIcon>();

    private RectTransform rectTransform;

    [SerializeField]
    private Transform parent;

    private void Awake ()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start ()
    {
        Storyteller.Instance.PartyUpdated += OnPartyUpdated;
        OnPartyUpdated(null, null);
    }

    private void OnPartyUpdated (object sender, System.EventArgs e)
    {
        Clear();

        var party = Storyteller.Instance.Party.ToList();

        int i = 0;

        foreach(Character character in party)
        {
            float t = party.Count > 1 ? (float)i / (party.Count - 1) : 0.5f;

            float t2 = Mathf.Lerp(0.5f, t, party.Count / 10f);

            Vector2 position = Vector2.Lerp(leftPosition, rightPosition, t2);

            var token = Instantiate(partyIconPrefab, position, Quaternion.identity, parent).GetComponent<PartyIcon>();
            token.AssignCharacter(character);
            token.SetPosition(position);           

            tokens.Add(token);
            i++;
        }

        
    }

    private void Clear ()
    {
        foreach(var token in tokens)
        {
            if(token != null)
            {
                Destroy(token.gameObject);
            }
        }
        tokens.Clear();
    }

    private void OnDestroy ()
    {
        Storyteller.Instance.PartyUpdated -= OnPartyUpdated;
    }

}
