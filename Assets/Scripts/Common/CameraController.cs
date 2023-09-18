using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    protected new Camera camera;
    public Camera Camera { get { return camera; } }

    protected Vector3 lastMousePosition;

    [SerializeField]
    protected float zoomSpeed = 1;

    [SerializeField]
    protected float minOrthographicsSize = 3;
    [SerializeField]
    protected float maxOrthographicSize = 15;

    protected virtual void Awake ()
    {
        camera = GetComponent<Camera>();
    }


    public Vector3 GetMousePosition ()
    {
        return camera.ScreenToWorldPoint(Input.mousePosition);
    }

    void Update ()
    {
        if(Input.GetMouseButton(2))
        {
            transform.position = transform.position + lastMousePosition - GetMousePosition();
        }


        Vector3 preScrollMousePosition = GetMousePosition();

        camera.orthographicSize = Mathf.Clamp(
            GetZoom(),
            minOrthographicsSize,
            maxOrthographicSize);

        Vector3 delta = GetMousePosition() - preScrollMousePosition;

        transform.position = transform.position - delta;

        ClampCamera();

        lastMousePosition = GetMousePosition();
    }

    protected virtual void ClampCamera ()
    {

    }

    protected virtual float GetZoom ()
    {
        return camera.orthographicSize - Input.mouseScrollDelta.y * zoomSpeed;
    }
}
