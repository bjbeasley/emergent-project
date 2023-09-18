using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class ActionTarget
{
    public ActionTarget (Action action, Agent target)
    {
        Action = action;
        Target = target;
    }

    public Agent Target { get; }
    public Action Action { get; }

}

