using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformGrid<T> : DataGrid<T>
{
    public Vector3 Origin { get; }
    private Vector3 xAxis;
    private Vector3 yAxis;
    private Matrix4x4 transformMatrix;
    private Matrix4x4 inverseTransform;

    public TransformGrid (Vector3 origin, Vector3 xAxis, Vector3 yAxis, int width, int height) : base(width, height)
    {
        Origin = origin;
        this.xAxis = xAxis;
        this.yAxis = yAxis;

        transformMatrix = new Matrix4x4(xAxis, yAxis, new Vector4(0,0,1,0), new Vector4(0, 0, 0, 1));
        inverseTransform = transformMatrix.inverse;
    }

    public Vector3 GridToWorldPos (Vector2Int gridCoords)
    {
        return GridToWorldPos(gridCoords.x, gridCoords.y);
    }

    public Vector3 GridToWorldPos (int x, int y)
    {
        Vector3 transformed = transformMatrix.MultiplyVector(new Vector4(x, y, 0, 0));
        return Origin + transformed;
    }

    public Vector2 WorldToGridPos (Vector3 worldPos)
    {
        Vector3 vec = worldPos - Origin;

        Vector3 invVec = inverseTransform.MultiplyVector(vec);
        return new Vector2(invVec.x, invVec.y);
    }

    public Vector2Int WorldToGridPosInt (Vector3 worldPos)
    {
        Vector2 pos = WorldToGridPos(worldPos);
        return new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
    }

    public bool InGrid (Vector3 worldPos)
    {
        return InGrid(WorldToGridPosInt(worldPos));
    }

    public T Get (Vector3 worldPos)
    {
        return Get(WorldToGridPosInt(worldPos));
    }

}
