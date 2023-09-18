using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Effect
{
    public abstract ActionButton.Icon Icon { get; }

    public abstract Color Color { get; }

    public abstract float GetEfficacy (Agent from, Agent target);

    public abstract void Apply (Vector2Int location);

    public abstract string GetDescription ();
}