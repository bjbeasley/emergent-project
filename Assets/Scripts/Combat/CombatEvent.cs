using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatEvent
{
    public List<CombatCharacterGroup> groups;
    System.Action victoryResult;
    System.Action defeatResult;

    public CombatEvent (System.Action victoryResult, System.Action defeatResult, params CombatCharacterGroup[] groups)
    {
        this.groups = groups.ToList();
        this.victoryResult = victoryResult;
        this.defeatResult = defeatResult;
    }

    public void SpawnGroups ()
    {
        foreach(CombatCharacterGroup group in groups)
        {
            group.Spawn();
        }
    }

    public void End (bool victory)
    {
        if(victory)
        {
            victoryResult();
        }
        else
        {
            defeatResult();
        }
    }
}
