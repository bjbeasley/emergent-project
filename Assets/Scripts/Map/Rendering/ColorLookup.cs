using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorLookup
{
    public static Vector2 GetTexCoords (Vector3 color)
    {
        return GetTexCoords(color.x, color.y, color.z);
    }

    public static Vector2 GetTexCoords (float r, float g, float b)
    {
        float xInSquare = r / 8;
        float yInSquare = g / 8;
        int squareIndex = (int)(64 * b);
        float xOffset = (squareIndex % 8) / 8.0f;
        float yOffset = (squareIndex / 8) / 8.0f;

        return new Vector2(xInSquare + xOffset, 1 - (yInSquare + yOffset));
    }

    public static Vector3 RandomColor (int seed)
    {
        System.Random random = new System.Random(seed);
        return new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
    }

    public static Vector2 GetTexCoords (Color color)
    {
        return GetTexCoords(color.r, color.g, color.b);
    }
}
