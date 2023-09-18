using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DamageEffect : Effect
{
    public int ShieldDamage { get; }
    public int HealthDamage { get; }
    public int IgnoreShield { get; }

    public override ActionButton.Icon Icon { get { return ActionButton.Icon.Attack; } }

    public override Color Color { get { return new Color(0.8f, 0.25f, 0.25f); } }

    public DamageEffect (int shieldDamage, int healthDamage, int ignoreShield = 0)
    {
        ShieldDamage = shieldDamage;
        HealthDamage = healthDamage;
        IgnoreShield = ignoreShield;
    }

    public override void Apply (Vector2Int location)
    {
        CombatTile tile = CombatMap.Instance.Grid.Get(location);

        Agent agent = tile.Agent;

        if(agent != null)
        {
            agent.Damage(ShieldDamage, HealthDamage, IgnoreShield);
        }
    }

    public override float GetEfficacy (Agent from, Agent target)
    {
        int hp = target.Character.HP;
        int sp = target.Character.TotalShieldPoints;

        if(hp == 0)
        {
            return 0;
        }

        if(sp > 0)
        {
            return Mathf.Clamp01(Mathf.Min(sp, ShieldDamage) * (sp == ShieldDamage ? 2 : 1) * ShieldDamage * 0.1f);
        }
        else
        {
            if(hp == HealthDamage)
            {
                return 1;
            }
            if(hp > HealthDamage)
            {
                return Mathf.Clamp01(hp * HealthDamage * 0.1f);
            }
            if(hp < HealthDamage)
            {
                int wasted = HealthDamage - hp;
                return 1 - Mathf.Clamp01(wasted / 5f) / 10f;
            }
            return hp == HealthDamage ? 1 : Mathf.Clamp01(Mathf.Abs(hp - HealthDamage) * HealthDamage);
        }
    }

    public override string GetDescription ()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("Deal damage to an enemy unit");
        sb.Append("\nShield Damage:<space=0.2em> <b>");
        sb.Append(ShieldDamage);
        sb.Append("</b>");
        sb.Append("\nHealth Damage: <b>");
        sb.Append(HealthDamage);
        sb.Append("</b> <i>(if target has no shield)</i>");

        if(IgnoreShield > 0)
        {
            sb.Append("\n<b>Ignores ");
            sb.Append(IgnoreShield);
            sb.Append(" shield points</b>");
        }

        return sb.ToString();
    }
}