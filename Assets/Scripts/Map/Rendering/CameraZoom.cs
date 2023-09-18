using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraZoom : MonoBehaviour
{
    public static CameraZoom Instance { get; private set; }

    private new Camera camera;

    public float zoom = 1;    

    [SerializeField]
    private float minY;
    public float MinY { get => minY; set => minY = value; }

    [SerializeField]
    private float maxY;
    public float MaxY { get => maxY; set => maxY = value; }

    [SerializeField]
    private float softMinZoom = 1.1f;
    public float SoftMinZoom { get => softMinZoom; set => softMinZoom = value; }

    [SerializeField]
    private float softMaxZoom = 1.2f;
    public float SoftMaxZoom { get => softMaxZoom; set => softMaxZoom = value; }

    public float Height { get => maxY - minY; }

    [SerializeField]
    private Material lineMaterial;
    public Material LineMaterial { get => lineMaterial; set => lineMaterial = value; }
    


    private void Awake ()
    {
        Instance = this;
        camera = GetComponent<Camera>();
    }

    private void Update ()
    {
    }

    public void AdjustZoom (float delta, Vector2? pixelCoordLock = null)
    {
        SetZoom(zoom + delta, pixelCoordLock);
    }

    public void SetZoom (float value, Vector2? pixelCoordLock = null)
    {
        if(value < 1)
        {
            value = 1;
        }
        if(zoom == value)
        {
            return;
        }

        zoom = value;

        float newSize = Height / Mathf.Exp(zoom) * Mathf.Exp(1);

        if(pixelCoordLock.HasValue)
        {
            Vector2 screenSpace = 2 * new Vector2(pixelCoordLock.Value.x / camera.pixelWidth
                , pixelCoordLock.Value.y / camera.pixelWidth) - Vector2.one;

            Vector2 worldSpaceLock = camera.ScreenToWorldPoint(pixelCoordLock.Value);
            camera.orthographicSize = newSize;
            Vector2 newWorldSpace = camera.ScreenToWorldPoint(pixelCoordLock.Value);
            //Vector2 screenSpaceLock = (2 * pixelCoordLock / camera.pixelHeight - (new Vector2(camera.aspect / 2, 0.5f)));

            // = screenSpaceLock * camera.orthographicSize;
            //screenSpaceLock * newSize;

            transform.position += (Vector3)(worldSpaceLock - newWorldSpace);
        }
        else
        {
            camera.orthographicSize = newSize;
        }
    }


}
