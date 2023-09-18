using EmpiresCore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventGenerator
{
    private CharacterGenerator characterGenerator;


    private int restAmbush = 1;

    public EventGenerator (CharacterGenerator characterGenerator)
    {
        this.characterGenerator = characterGenerator;
    }

    public void BasicIntro ()
    {
        Character character = characterGenerator.GetCharacter(hp: 5, itemRarity: Item.Rarity.Common);
        character.Alertness = 100;

        Storyteller.Instance.AddMember(character);

        float value = Random.value;
        float count = 7f;

        string background =
            value < 1 / count ? "cattle farmer, you were tending to you cows when " :
            value < 2 / count ? "hunter, you were chasing down a doe when " :
            value < 3 / count ? "fisher, you were walking back from town, having sold all of your fish for the day when " :
            value < 5 / count ? "mercenary, you were travelling to your old guild to re-enlist when " :
            value < 6 / count ? "priest, you were on a pilgrimage when " :
                                "war veteran, you were wandering the county in search of work when ";


        DialogManager.Instance.CreateDialog(
                "A Call to Adventure",
                "You are " + character.Name + ", a " + background + "you noticed the sky growing dark overhead. \n\n\"<i>But sunset shouldn't be for hours yet...</i>\" you mutter to yourself. \n\nSuddenly, you hear screams from a nearby farmhouse and you see flames erupting from the thatch roof.",
                new DialogOption("Investigate", () => IntroInvestigate()),
                new DialogOption("Run the other way!"));

    }


    public void IntroInvestigate ()
    {
        List<Character> victims = new List<Character>()
        {
            characterGenerator.GetCharacter(),
            characterGenerator.GetCharacter(),
        };


        DialogManager.Instance.CreateDialog(
                "A Fight is At Hand",
                "As you approach the farmhouse you see 2 people under attack from a third. The attacker looks... off somehow. \n\nA flash of flames from the burning building illuminates their face for a brief moment and you see that their eyes are a deep black, without iris or sclera.",
                new DialogOption("Fight!", () =>
                {
                    List<Character> attackers = new List<Character>()
                    {
                        characterGenerator.GetCharacter(true)
                    };

                    CombatEvent combatEvent = new CombatEvent(
                        () => DialogManager.Instance.CreateDialog(
                            "Victory",
                            "As you brush yourself down the people you helped introduce themselves as " + victims[0].Name + " and " + victims[1].Name + ". \n\nTheir house is beyond saving, you watch as the last beam crumbles, causing a huge plume of ash. \n\n<i>\"I guess it's best if we all stick together now then\"</i>  " + victims[0].Name + " says.",
                            new DialogOption("Agreed", () => victims.ForEach(v => Storyteller.Instance.AddMember(v))),
                            new DialogOption("I work alone")),
                        () => CombatDefeat(),
                        new CombatCharacterGroup(CombatCharacterGroup.Position.South, Storyteller.Instance.Party, true, true),
                        new CombatCharacterGroup(CombatCharacterGroup.Position.Center, victims, true, true),
                        new CombatCharacterGroup(CombatCharacterGroup.Position.Center, attackers, false, false));

                    Storyteller.Instance.StartCombat(combatEvent);

                }),
                new DialogOption("Sprint away!"));
    }


    public void RiskAreaEntered ()
    {
        DialogManager.Instance.CreateDialog(
                "The sky grows dark",
                "The sky grows dark overhead and you sense an iminent danger. Nothing happens immediately but you sense you need to leave this area soon.",
                new DialogOption("Press on"));
    }

    public void MemberJoins (Character character = null)
    {
        if(character == null)
        {
            character = characterGenerator.GetCharacter();
        }

        DialogManager.Instance.CreateDialog(
                character.Name + " joins your party",
                character.Name + " is willing to join you in your struggle. Would you like to allow them?",
                new DialogOption("Yes, we could use an extra hand around here", () => Storyteller.Instance.AddMember(character)),
                new DialogOption("No, we're better without extra mouths to feed."));
    }

    public void WandererJoins (Character character = null)
    {
        if(character == null)
        {
            character = characterGenerator.GetCharacter();
        }

        float value = Random.value;

        string reason =
            value < 0.2f ? " their village was burned down by rioters and they have nowhere else to go." :
            value < 0.4f ? " their family was infected by the curse and they were the only surivor." :
            value < 0.6f ? " they're looking for adventure. They look young and unequiped for the horrors you'll be facing." :
            value < 0.8f ? " they were seperated from their family when escaping their village" :
                           " they are good with a sword and might be able to help you.";

        DialogManager.Instance.CreateDialog(
                "A wanderer approaches",
                "The wanderer introduces themselves as " + character.Name + ". They say " + reason + "\nThey don't appear to have any equipment that will be of any use in combat.",
                new DialogOption("Invite them to join your party.", () => Storyteller.Instance.AddMember(character)),
                new DialogOption("I'm sorry, we cannot help you."));
    }


    public void GetAmbush (int size = 3, bool infected = false)
    {
        List<Character> attackers = new List<Character>();
        for(int i = 0; i < size; i++)
        {
            attackers.Add(characterGenerator.GetCharacter(infected: true));
        }

        CombatEvent combatEvent = new CombatEvent(
            () => CombatVictory(),
            () => CombatDefeat(),
            new CombatCharacterGroup(CombatCharacterGroup.Position.Center, Storyteller.Instance.Party, true, true),
            new CombatCharacterGroup(CombatCharacterGroup.Position.AllBorders, attackers, false, false));

        DialogManager.Instance.CreateDialog(
                "Ambush!",
                infected ? "The soulless have found you while you were resting. Prepary to fight." : "You have been ambushed by a group of bandits, they make no demands before attacking you.",
                new DialogOption("If it's a fight that you want, it's a fight you'll get", () => Storyteller.Instance.StartCombat(combatEvent)));
    }

    private void CombatVictory ()
    {
        DialogManager.Instance.CreateDialog(
            "Victory!",
            "You have won this battle, but there will be more to come",
            new DialogOption("Onwards"));
    }

    private void CombatDefeat ()
    {
        DialogManager.Instance.CreateDialog(
            "Defeat!",
            "Your journey has come to an end.\nYou managed to survive for " + DayCounter.Instance.Count + " days",
            new DialogOption("Exit", QuitGame));
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void GetProvinceEvent (Province province)
    {
        if(ResourceManager.Instance.IsExpended(province))
        {
            return;
        }

        //Use ID based random value so province is continuous
        float randomValue = (float)new System.Random(province.ID).NextDouble();

        var resource = ResourceManager.Instance.GetProvinceResource(province);

        bool infected = Infection.Instance.IsProvinceInfected(province);

        switch(resource)
        {
            case ResourceManager.Resource.None:
                GetNoneEvent(province, randomValue, infected);
                break;
            case ResourceManager.Resource.Town:
                GetTownEvent(province, randomValue, infected);
                break;
            case ResourceManager.Resource.Loot:
                GetLootEvent(province, randomValue, infected);
                break;
            case ResourceManager.Resource.Fort:
                GetFortEvent(province, randomValue, infected);
                break;
            case ResourceManager.Resource.Food:
                break;
            case ResourceManager.Resource.Other:
                GetOtherEvent(province, randomValue, infected);
                break;
            default:
                break;
        };
    }

    private void GetNoneEvent (Province province, float value, bool infected)
    {
        if(value < 0.05f && !infected)
        {
            GetAmbush();
            return;
        }
        else if(value < 0.1f && !infected)
        {
            DialogManager.Instance.CreateDialog(
               "Soulless Scouts",
               "While travelling you are spotted by an advance party of the souless. They charge at you.",
               new DialogOption("Fight!", () =>
               {
                   var attackers = GetEnemies(true, Mathf.Min(3, Storyteller.Instance.Party.Count()), 1, 2);
                   CombatEvent combatEvent = new CombatEvent(
                       () => CombatVictory(),
                       () => CombatDefeat(),
                       new CombatCharacterGroup(CombatCharacterGroup.Position.South, Storyteller.Instance.Party, true, true),
                       new CombatCharacterGroup(CombatCharacterGroup.Position.North, attackers, false, false));

                   Storyteller.Instance.StartCombat(combatEvent);

               }));
            return;
        }
        else if(value < 0.75f && infected)
        {
            DialogManager.Instance.CreateDialog(
               "Journeying in the Land of the Soulless",
               "With the dark sky overhead, and the strange chill in the air, you knew it wouldn't be long until they found you.\n\nA group of soulless line up ahead of you. The fight is unavoidable. ",
               new DialogOption("Fight!", () =>
               {
                   var attackers = GetEnemies(true, Mathf.Min(3, Storyteller.Instance.Party.Count()), 1, 4);
                   CombatEvent combatEvent = new CombatEvent(
                       () => CombatVictory(),
                       () => CombatDefeat(),
                       new CombatCharacterGroup(CombatCharacterGroup.Position.South, Storyteller.Instance.Party, true, true),
                       new CombatCharacterGroup(CombatCharacterGroup.Position.North, attackers, false, false));

                   Storyteller.Instance.StartCombat(combatEvent);

               }));
            return;
        }
        else if(value < 0.8f && infected)
        {
            int count = Mathf.Max(2, Storyteller.Instance.Party.Count() / 3);
            int hp = 20 / count;

            DialogManager.Instance.CreateDialog(
               "Soulless Warleaders",
               "With the dark sky overhead, and the strange chill in the air, you knew it wouldn't be long until they found you.\n\n" + count + " powerful looking soulless line up ahead of you. The fight is unavoidable. ",
               new DialogOption("Fight!", () =>
               {
                   var attackers = GetEnemies(true, count, hp, 4);
                   CombatEvent combatEvent = new CombatEvent(
                       () => CombatVictory(),
                       () => CombatDefeat(),
                       new CombatCharacterGroup(CombatCharacterGroup.Position.South, Storyteller.Instance.Party, true, true),
                       new CombatCharacterGroup(CombatCharacterGroup.Position.North, attackers, false, false));

                   Storyteller.Instance.StartCombat(combatEvent);
               }));
            return;
        }
    }

    private void GetLootEvent (Province province, float value, bool infected)
    {
        if(value < 0.33f)
        {
            DialogManager.Instance.CreateDialog(
                "A Lone Chest",
                "You find a lone chest, it was either poorly hidden, or placed here with the intention of being found.",
                new DialogOption("Open It", () => GetItemEvent(province)));
            return;
        }
        if(value < 0.66f)
        {
            string partyMember = Storyteller.Instance.Party.Last().Name.First;
            string name = characterGenerator.GetCharacter().Name.First;
            Item item = ItemGenerator.GetRandomWeapon(Item.Rarity.VeryRare);

            DialogManager.Instance.CreateDialog(
                "A Fallen Warrior",
                "You find a the skeleton of a soldier. " + partyMember + " says they recognise them, they were " + name + ", a folk hero around these parts, clutched in their hands is their faithful " + item.Name + ".",
                new DialogOption("Take it", () => ItemSlot.PickupItem(item, province)),
                new DialogOption("It's theirs to keep, even in death", () => ResourceManager.Instance.ExpendResource(province)));
            return;
        }

        if(value < 1.1f)
        {
            Item item = ItemGenerator.GetRandomWeapon(Item.Rarity.VeryRare);

            int count = Storyteller.Instance.Party.Count();

            CombatEvent combatEvent = new CombatEvent(() => GetItemEvent(province, item), () => CombatDefeat(),
                new CombatCharacterGroup(CombatCharacterGroup.Position.Center, GetEnemies(infected, count, 1, 3), false, false),
                GetParty(CombatCharacterGroup.Position.South));

            DialogManager.Instance.CreateDialog(
                "A Guarded Chest",
                "You find a lone chest, but around it are " + count + " bandits trying to crack it open. Whatever is inside must be very rare indeed.",
                new DialogOption("The treasure will be ours. Attack!", () => Storyteller.Instance.StartCombat(combatEvent)),
                new DialogOption("It's not worth the risk"));
            return;
        }
    }

    private void GetFortEvent (Province province, float value, bool infected)
    {

        if(value < 0.33f)
        {
            DialogManager.Instance.CreateDialog(
                "An Abandoned Fort",
                "You find the fort abandoned. Whoever was here left in a hurry. You can rest here for 1 day without fear of ambush.",
                new DialogOption("Ok"));

            ResourceManager.Instance.ExpendResource(province);
            ResourceManager.Instance.AddSafeRests(province, 2);
            return;
        }
        if(value < 0.66f && !infected)
        {
            DialogManager.Instance.CreateDialog(
                "An Friendly Fort",
                "The inhabitants of the fort see you and quickly usher you inside. You can rest here for 2 days without fear of ambush.",
                new DialogOption("Ok"));

            ResourceManager.Instance.ExpendResource(province);
            ResourceManager.Instance.AddSafeRests(province, 4);

            return;
        }

        CombatEvent combatEvent = new CombatEvent(() => FortVictory(province), () => CombatDefeat(),
            new CombatCharacterGroup(CombatCharacterGroup.Position.East, GetEnemies(infected, 5, 1, 3), false, false),
            GetParty(CombatCharacterGroup.Position.West));

        DialogManager.Instance.CreateDialog(
                "An Hostile Fort",
                "The 5 inhabitants of the fort begin to shoot as you as you get near. You can either attack them and take the fort as your own, or leave them be.",
                new DialogOption("Attack", () => Storyteller.Instance.StartCombat(combatEvent)),
                new DialogOption("Try to flee (you will be damaged in the process)", () => Storyteller.Instance.DamageAll(1)));
    }

    public void GetTownEvent (Province province, float value, bool infected)
    {
        if(value < 0.5f)
        {
            DialogManager.Instance.CreateDialog(
                "An Abandoned Town",
                "You find the town abandoned. Whoever was here left in a hurry. You can rest here for half a day without fear of ambush.",
                new DialogOption("Ok"));

            ResourceManager.Instance.ExpendResource(province);
            ResourceManager.Instance.AddSafeRests(province, 1);
            return;
        }
        if(value < 0.9f && !infected)
        {
            DialogManager.Instance.CreateDialog(
                "An Friendly Town",
                "The inhabitants of the town seem friendly enough. They take your warnings about the oncoming evil seriously. You can rest here for 1 day without fear of ambush.",
                new DialogOption("Ok", () =>
                {
                    Character character = characterGenerator.GetCharacter(false, hasItem: false, hp: Random.Range(3, 6));

                    DialogManager.Instance.CreateDialog("Potential Recruit",
                        "A member of the town approaches you: \n\"Greetings, my name is " + character.Name.First + " I am a " + GetBackground() + " here, but I sense that I won't be able to continue that job for much longer. I have no weapon, but if you take me with you on your journey and I will gladly fight by your side.\"",
                        new DialogOption("Your help would be welcome!", () => Storyteller.Instance.AddMember(character)),
                        new DialogOption("You're better off here with your fellow townsmen."));

                }));

            ResourceManager.Instance.ExpendResource(province);
            ResourceManager.Instance.AddSafeRests(province, 2);

            return;
        }

        CombatEvent combatEvent = new CombatEvent(() => FortVictory(province, "town"), () => CombatDefeat(),
            new CombatCharacterGroup(CombatCharacterGroup.Position.East, GetEnemies(infected, 3, 1, 2), false, false),
            GetParty(CombatCharacterGroup.Position.West));

        DialogManager.Instance.CreateDialog(
                "An Hostile Town",
                "The 3 inhabitants of the town begin to shoot as you as you get near. You can either attack them and then rest in the town, or leave them be.",
                new DialogOption("Attack", () => Storyteller.Instance.StartCombat(combatEvent)),
                new DialogOption("Try to flee (you will be damaged in the process)", () => Storyteller.Instance.DamageAll(1)));
    }

    private void FortVictory (Province province, string name = "fort")
    {
        DialogManager.Instance.CreateDialog(
                "The Fort is Yours",
                "You have succesfully taken the " + name + ". You can rest here for 2 days without fear of ambush.",
                new DialogOption("Ok"));

        ResourceManager.Instance.ExpendResource(province);
        ResourceManager.Instance.AddSafeRests(province, 4);
    }

    public void GetOtherEvent (Province province, float value, bool infected)
    {
        if(value < 0.33f)
        {
            string finding = infected ? "fallen cartographer, it looks like the soulless got him recently. Clutched in their hand is" : "friendly cartographer, they offer you";

            DialogManager.Instance.CreateDialog(
                "A Map of the Area",
                "You find a " + finding + " a map of the local area. \n\nThe nearby points of interest have been marked down on your map.",
                new DialogOption("Ok"));

            ResourceManager.Instance.Explore(province, 5);
            ResourceManager.Instance.RemoveResource(province);

            return;
        }
        if(value < 0.45f && !infected)
        {
            Character warrior = characterGenerator.GetCharacter(false, 11, true, Item.Rarity.Rare);

            CombatEvent warriorEvent = new CombatEvent(() => WarriorJoins(warrior), () => WarriorRefuses(),
                new CombatCharacterGroup(CombatCharacterGroup.Position.North, new List<Character>() { warrior }, false, false),
                GetParty(CombatCharacterGroup.Position.South));

            DialogManager.Instance.CreateDialog(
                "A Great Warrior",
                "You come across a great warrior, their mannerisms exude the fortitude of an experienced fighter, one used to getting into scrapes. They say: \n\n\"Greetings travellers! I'm in need of a party to fight alongside, if you can best me in combat, I shall join you\"",
                new DialogOption("Then we shall fight!", () => Storyteller.Instance.StartCombat(warriorEvent)),
                new DialogOption("We cannot afford unnecessary fighting today"));

            ResourceManager.Instance.RemoveResource(province);

            return;
        }
        if(value < 0.5f && !infected)
        {
            WandererJoins();
        }

        Character character = characterGenerator.GetCharacter(true, Storyteller.Instance.Party.Count() * 2, true, Item.Rarity.Rare);

        CombatEvent combatEvent = new CombatEvent(() => GetItemEvent(province, character.Item), () => CombatDefeat(),
            new CombatCharacterGroup(CombatCharacterGroup.Position.Center, new List<Character>() { character }, false, false),
            GetParty(CombatCharacterGroup.Position.AllBorders));

        DialogManager.Instance.CreateDialog(
            "A Soulless Warrior",
            "You come across a tall figure, as you draw near you see their eyes are a deep black. There's no use running now. You must fight.",
            new DialogOption("Form a perimiter!", () => Storyteller.Instance.StartCombat(combatEvent)));

        ResourceManager.Instance.RemoveResource(province);

    }

    private void WarriorJoins (Character character)
    {
        Storyteller.Instance.AddMember(character);
        Storyteller.Instance.WakeAllParty();
        character.HP = character.MaxHP;

        DialogManager.Instance.CreateDialog(
                "Warrior Joins",
                "\"A deal is a deal\"\n\nYou all tend to your wounds after the fight and ensure no-one is downed.",
                new DialogOption("Welcome to the party"));
    }

    private string GetBackground ()
    {
        float value = Random.value;

        return value < 0.1f ? "farmer"
            : value < 0.1f ? "taylor"
            : value < 0.2f ? "tanner"
            : value < 0.3f ? "carpenter"
            : value < 0.5f ? "cooper"
            : value < 0.6f ? "cobbler"
            : value < 0.8f ? "innkeeper"
            : value < 0.9f ? "entertainer"
            : "leatherworker";

    }

    private void WarriorRefuses ()
    {
        Storyteller.Instance.WakeAllParty();
        DialogManager.Instance.CreateDialog(
                "Warrior Refuses",
                "\"It's dangerous out there and I'd hoped you'd be tougher.\"\n\nThe warrior tends to your wounds before leaving.",
                new DialogOption("Ok"));
    }

    private IEnumerable<Character> GetEnemies (bool infected, int count, int hpMin, int hpMax)
    {
        for(int i = 0; i < count; i++)
        {
            yield return characterGenerator.GetCharacter(infected, Random.Range(hpMin, hpMax));
        }
    }

    private CombatCharacterGroup GetParty (CombatCharacterGroup.Position position)
    {
        return new CombatCharacterGroup(position, Storyteller.Instance.Party, true, true);
    }

    private void GetItemEvent (Province province, Item item = null)
    {
        if(item == null)
        {
            item = ItemGenerator.GetRandomWeapon();
        }

        DialogManager.Instance.CreateDialog("Item!", "You have found a " + Item.RarityName(item.ItemRarity) + " " + item.Name + "!",
            new DialogOption("Ok"));

        ItemSlot.PickupItem(item, province);
    }

    public void RestEvent (Province province)
    {
        bool infected = Infection.Instance.IsProvinceInfected(province);
        float value = Random.value;
        if(infected && value < 0.8f)
        {
            GetAmbush(restAmbush, true);
            restAmbush++;
        }
        else if (!infected && value < 0.4f)
        {
            GetAmbush(restAmbush);
            restAmbush++;
        }

    }

}
