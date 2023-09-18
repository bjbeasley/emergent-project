using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DayLabel : MonoBehaviour
{

    private TextMeshProUGUI text;

    private enum Mode
    {
        Day = 1,
        Time = 2,
    }

    [SerializeField]
    Mode mode = Mode.Day;

    private void Awake ()
    {
        text = GetComponent<TextMeshProUGUI>();
    }


    private void LateUpdate ()
    {
        string label = DayCounter.Instance.Count.ToString();

        if(mode == Mode.Time)
        {
            switch(DayCounter.Instance.TimePhase)
            {
                case DayCounter.Phase.Day:
                    label = "Day";
                    break;
                case DayCounter.Phase.Night:
                    label = "Night";
                    break;
                default:
                    break;
            }
        }

        text.text = label;
    }
}
