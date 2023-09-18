using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using EmpiresCore;

public class Storyteller : MonoBehaviour
{
    public static Storyteller Instance { get; private set; }

    [SerializeField]
    private CharacterGenerator characterGenerator;

    public CharacterGenerator CharacterGenerator { get { return characterGenerator; } }


    [SerializeField]
    private WorldBuilder worldBuilder;

    [SerializeField]
    private int mainSceneBuildIndex = 0;
    [SerializeField]
    private int combatSceneBuildIndex = 1;

    private List<Character> party;
    public IEnumerable<Character> Party { get { return party; } }

    private EventGenerator eventGenerator;

    private CombatEvent combatEvent;

    public event EventHandler PartyUpdated;

    public enum SceneType
    {
        Map = 1,
        Combat = 2,
    }

    public SceneType CurrentScene { get; private set; } = SceneType.Map;


    private void Awake ()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        party = new List<Character>();
        Instance = this;
        DontDestroyOnLoad(this);
        eventGenerator = new EventGenerator(CharacterGenerator);
        
    }

    private void Start ()
    {
        eventGenerator.BasicIntro();        
    }

    private void Update ()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            AddMember(characterGenerator.GetCharacter());
        }
        if(Input.GetKeyDown(KeyCode.X))
        {
            if(combatEvent != null)
            {
                EndCombat(true);
            }
            else
            {
                eventGenerator.GetAmbush(party.Count);
            }
            
        }

        if(Input.GetKeyUp(KeyCode.I))
        {
            ItemSlot.PickupItem(ItemGenerator.GetRandomWeapon(), null);
        }
    }

    public void StartCombat (CombatEvent combatEvent)
    {
        this.combatEvent = combatEvent;
        StartCoroutine(CombatCoroutine());
    }

    public void EndCombat (bool victory)
    {
        if(combatEvent == null)
        {
            return;
        }
        StartCoroutine(EndCombatCoroutine(victory, combatEvent));
        combatEvent = null;
    }

    private IEnumerator CombatCoroutine ()
    {
        worldBuilder.gameObject.SetActive(false);
        CaravanController.Instance.gameObject.SetActive(false);
        SceneManager.LoadScene(combatSceneBuildIndex, LoadSceneMode.Single);
        CurrentScene = SceneType.Combat;

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        combatEvent.SpawnGroups();
    }

    private IEnumerator EndCombatCoroutine (bool victory, CombatEvent combatEvent)
    {
        yield return new WaitForSecondsRealtime(0.3f);
        combatEvent.End(victory);
        yield return new WaitForSecondsRealtime(0.1f);
        SceneManager.LoadScene(mainSceneBuildIndex);
        worldBuilder.gameObject.SetActive(true);
        CaravanController.Instance.gameObject.SetActive(true);
        CurrentScene = SceneType.Map;
        yield return null;
        MapCameraController.Instance.ZoomOnPlayer();

    }

    public void AddMember (Character character)
    {
        if(party.Count < 16)
        {
            party.Add(character);
            party = party.OrderByDescending(c => c.Alertness).ToList();
            PartyUpdated?.Invoke(this, new EventArgs());
        }
        else
        {
            DialogManager.Instance.CreateDialog("Party Full", 
                character.Name + " sees that you already have 16 members in your group. Travelling in any larger numbers is likely to attract unwanted attention, so they decide to go their seperate way.",
                new DialogOption("Goodbye"));
        }
        
    }

    public void RemoveDownedMembers ()
    {
        party.RemoveAll(c => !c.Awake);
        PartyUpdated?.Invoke(this, new EventArgs());
    }

    public void WakeAllParty ()
    {
        foreach(Character character in party)
        {
            if(character.HP == 0)
            {
                character.HP = 1;
            }
        }
    }

    public void DamageAll(int damage)
    {
        int upIndex = 0;
        int i = 0;
        foreach(Character character in party)
        {            
            if(character.HP > 0)
            {
                upIndex = i;
            }
            i++;
            character.HP = Mathf.Max(0, character.HP - damage);
        }

        if(party.All(c => !c.Awake))
        {
            party[upIndex].HP = 1;
        }
    }


    public void GetProvinceEvent (Province province)
    {
        eventGenerator.GetProvinceEvent(province);
    }

    public void Rest ()
    {
        bool safe = ResourceManager.Instance.IsRestSafe(CaravanController.Instance.Province, true);

        if(!safe)
        {
            eventGenerator.RestEvent(CaravanController.Instance.Province);
        }


        foreach(Character character in Party)
        {
            if(character.HP < character.MaxHP)
            {
                character.HP++;
            }
        }

        DayCounter.Instance.IncrementPhase();
    }
}
