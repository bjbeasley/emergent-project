using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    private float currentPhase = 0;
    private float phaseVelocity = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {

        float targetPhase = DayCounter.Instance.TimeCount;

        currentPhase = Mathf.SmoothDamp(currentPhase, targetPhase, ref phaseVelocity, 0.2f);

        float t = Mathf.Repeat(currentPhase, 2) / 2;


        var pos = new Vector3(-t * 256 + WorldBuilder.Instance.TimezoneOffset, 0, transform.position.y);

        transform.position = MapCameraController.Instance.GetWrappedPosition(pos);
    }
}
