using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogOption
{
    public string Text { get; }
    System.Action result;

    public DialogOption(string text, System.Action result = null)
    {
        Text = text;
        this.result = result;
    }

    public void Execute ()
    {
        if(result != null)
        {
            result();
        }        
    }
}
