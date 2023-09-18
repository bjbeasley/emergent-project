using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WordDemo : MonoBehaviour
{
    [SerializeField]
    WordGenerator generator;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(generator.GetWord());
        }
    }
}
