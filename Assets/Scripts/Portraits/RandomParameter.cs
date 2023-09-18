using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class RandomParameter
{
    [SerializeField]
    private float min = 0;

    [SerializeField]
    private float max = 1;

    
    public float GetValue ()
    {
        return UnityEngine.Random.Range(min, max);
    }
}
