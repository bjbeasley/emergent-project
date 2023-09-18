using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class CombatTile
{
    public Agent Agent { get; set; }
    public bool Passable { get; set; } = true;

    public bool Available { get { return Passable && Agent == null; } }
    public void RemoveAgent (Agent agent)
    {
        if(agent == Agent)
        {
            Agent = null;
        }
    }

    public bool PassableTo (Agent agent)
    {
        return Passable && (Agent == null || Agent.PassableTo(agent));
    }

    public bool IsCover (Agent ignoreAgent = null)
    {
        return !(Agent == null || Agent == ignoreAgent);
    }


}

