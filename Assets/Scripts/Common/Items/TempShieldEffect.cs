using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TempShieldEffect : Effect
{
    public int ShieldPoints { get; }

    public override Color Color { get { return new Color(.3f, .45f, .7f); } }

    public override ActionButton.Icon Icon { get { return ActionButton.Icon.Shield; } }

    public TempShieldEffect (int shieldPoints)
    {
        ShieldPoints = shieldPoints;
    }

    public override void Apply (Vector2Int location)
    {
        CombatTile tile = CombatMap.Instance.Grid.Get(location);

        Agent agent = tile.Agent;

        if(agent != null)
        {
            agent.Character.TempShieldPoints += ShieldPoints;
        }
    }

    public override float GetEfficacy (Agent from, Agent target)
    {
        int sp = target.Character.TotalShieldPoints;
        int hp = target.Character.HP;

        if(sp == 0)
        {
            return Mathf.Clamp01(ShieldPoints * (4 - hp) * 0.01f);
        }
        else
        {
            return 0.001f * Mathf.Max(4 - sp, 0) * ShieldPoints;
        }
    }

    public override string GetDescription ()
    {
        return "Gain <b>" + ShieldPoints + "</b> temporary shield point(s) \n(they will be lost at the start of your next turn)";
    }
}