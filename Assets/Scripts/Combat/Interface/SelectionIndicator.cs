using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionIndicator : MonoBehaviour
{
    enum IndicatorMode 
    {
        Selection = 0,
        Turn = 1,
    };

    [SerializeField]
    IndicatorMode mode = IndicatorMode.Selection;

    [SerializeField]
    private bool followTransform = false;

    [SerializeField]
    private float zOffset = -1;

    void LateUpdate()
    {
        Agent selected = mode == IndicatorMode.Selection ? CombatMap.Instance.SelectedAgent : TurnManager.Instance.CurrentAgent;

        if(selected != null)
        {
            transform.position = (followTransform 
                ? selected.transform.position
                : CombatMap.Instance.Grid.GridToWorldPos(selected.GridPos))
                + transform.forward * zOffset;
        }
        else
        {
            transform.position = CombatCamera.Instance.transform.position * 2;
        }
    }
}
