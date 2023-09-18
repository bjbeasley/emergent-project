using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasCameraSwitch : MonoBehaviour
{

    [SerializeField]
    private Canvas canvas;

    // Update is called once per frame
    void LateUpdate()
    {
        if(canvas.worldCamera == null)
        {
            if(CombatCamera.Instance != null)
            {
                canvas.worldCamera = CombatCamera.Instance.Camera;
            }
            else if(MapCameraController.Instance != null)
            {
                canvas.worldCamera = MapCameraController.Instance.Camera;
            }
        }
    }
}
