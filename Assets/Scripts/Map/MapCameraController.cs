using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCameraController : CameraController
{
    public static MapCameraController Instance { get; private set; }

    [SerializeField]
    private Bounds cameraBounds;

    protected override void Awake ()
    {
        base.Awake();
        Instance = this;
    }

    protected override float GetZoom ()
    {
        float old = camera.orthographicSize;
        float log = Mathf.Log(old) - Input.mouseScrollDelta.y * zoomSpeed;

        return Mathf.Exp(log);
    }

    public void ZoomOnPlayer ()
    {
        var pos = CaravanController.Instance.Province.MeanPos;
        transform.position = new Vector3(pos.X, pos.Y, transform.position.z);
        camera.orthographicSize = minOrthographicsSize;
    }


    protected override void ClampCamera ()
    {
        base.ClampCamera();

        Vector3 pos = transform.position;

        float height = camera.orthographicSize;

        pos.y = Mathf.Clamp(pos.y, cameraBounds.min.y + height, cameraBounds.max.y - height);
        pos.x = Mathf.Repeat(pos.x + cameraBounds.size.x / 2 - cameraBounds.center.x, cameraBounds.size.x) - cameraBounds.size.x / 2 + cameraBounds.center.x;

        transform.position = pos;
        lastMousePosition = GetMousePosition();
    }

    private void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(cameraBounds.center, cameraBounds.size);
    }

    public Vector3 GetWrappedPosition(Vector3 pos)
    {
        float x = pos.x;
        float x2 = x;
        float dx = Mathf.Abs(transform.position.x - x);
        float dxPlus = Mathf.Abs(x + 256 - transform.position.x);
        if(dxPlus < dx)
        {
            x2 = x + 256;
            dx = dxPlus;
        }
        if (Mathf.Abs(x - 256 - transform.position.x) < dx)
        {
            x2 = x - 256;
        }
        return new Vector3(x2, pos.y, pos.z);
    }
}
