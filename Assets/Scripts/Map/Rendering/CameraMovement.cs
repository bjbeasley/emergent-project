using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMovement : MonoBehaviour
{
    public static CameraMovement Instance { get; private set; }
    private new Camera camera;

    public Camera Camera { get { return camera; } }

    public bool LockPosition { get; set; } = true;
    public bool lockPosition = true;

    public Vector3 velocity;

    public float dragCoefficient = 0.15f;

    public float minVelocity = 0.1f;

    public float minY;
    public float maxY;

    public float minWrapX;
    public float maxWrapX;

    public float CameraHalfWidth { get { return camera.orthographicSize * camera.aspect; } }
    public float LeftBound { get { return camera.transform.position.x - CameraHalfWidth; } }
    public float RightBound { get { return camera.transform.position.x + CameraHalfWidth; } }
    public Vector3 Position { get { return camera.transform.position; } }

    private void Awake ()
    {
        camera = GetComponent<Camera>();
        Instance = this;
    }

    private void LateUpdate ()
    {
        lockPosition = LockPosition;

        if(!LockPosition)
        {
            transform.position = transform.position + velocity * Time.deltaTime;
            velocity -= velocity * Time.deltaTime * Time.deltaTime * dragCoefficient;
            if(velocity.sqrMagnitude < minVelocity)
            {
                velocity = Vector3.zero;
            }
        }

        BoundCamera();
    }

    public void TranslateAbsolute (Vector3 delta)
    {
        velocity = delta / Time.deltaTime;
        transform.position = transform.position + delta;
    }

    public void TranslateScreenSpace (Vector3 delta)
    {
        TranslateAbsolute(delta * 2 * camera.orthographicSize / camera.pixelHeight);
    }

    private void BoundCamera ()
    { 
        if(transform.position.y > maxY - camera.orthographicSize)
        {
            transform.position = new Vector3(
                transform.position.x,
                maxY - camera.orthographicSize,
                transform.position.z);
        }
        if(transform.position.y < minY + camera.orthographicSize)
        {
            transform.position = new Vector3(
                transform.position.x,
                minY + camera.orthographicSize,
                transform.position.z);
        }

        float cameraWidth = camera.orthographicSize * camera.aspect;

        if(transform.position.x < minWrapX - cameraWidth)
        {
            transform.position = new Vector3(
                transform.position.x + 256,
                transform.position.y,
                transform.position.z);
        }
        if(transform.position.x > maxWrapX - cameraWidth)
        {
            transform.position = new Vector3(
                transform.position.x - 256,
                transform.position.y,
                transform.position.z);
        }

    }
}
