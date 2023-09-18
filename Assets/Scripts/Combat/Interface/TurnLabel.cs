using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnLabel : MonoBehaviour
{

    [SerializeField]
    private TMP_Text text;

    void LateUpdate()
    {
        var agent = TurnManager.Instance.CurrentAgent;

        if(agent != null && agent.Character != null)
        {
            text.text = agent.Character.Name.GetFull();
        }
    }
}
