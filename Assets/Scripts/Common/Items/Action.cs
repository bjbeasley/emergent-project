using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Action
{
    public Range Range { get; }

    public Effect Effect { get; }

    public string Name { get; }

    public Action (string name, Range range, Effect effect)
    {
        Name = name;
        Range = range;
        Effect = effect;
    }

    public string GetDescription ()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("<b><u>");
        sb.Append(Name);
        sb.Append("</u> - 1 Action</b>\n");

        sb.Append("<b>Range:</b> ");
        sb.Append(Range.GetDescription());
        sb.Append("\n<b>Effect:</b>\n");

        sb.Append(Effect.GetDescription());

        return sb.ToString();
    }

}
