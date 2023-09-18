using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CombatCamera : CameraController
{
    public static CombatCamera Instance { get; private set; }    

    protected override void Awake ()
    {
        base.Awake();
        if(Instance != null)
        {
            Debug.Log("Multiple combat cameras detected");
        }
        Instance = this;
        
    }

    
}
