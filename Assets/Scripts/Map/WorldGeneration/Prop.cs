using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using EmpiresCore;


public class Prop
{
    public PropType Type { get; }
    public Province Province { get; }
    public Vector3 Position { get; }

    private float scale;

    private PropUVMap uvMap;

    public Prop (PropType type, Province province, PropUVMap uvMap, Vector3 position, float scale)
    {
        Type = type;
        Province = province;
        this.uvMap = uvMap;
        Position = position;
        this.scale = scale;
    }

    public IEnumerable<Vector3> GetVertices ()
    {
        foreach(Vector2 vertex in uvMap.uvs)
        {
            Vector2 uvPos = vertex - uvMap.uvs[0] - (uvMap.uvs[1] - uvMap.uvs[0]) / 2;
            Vector3 basePosition = ((Vector3)uvPos) * scale * 2;
            basePosition.z = -basePosition.y;
            yield return basePosition + Position;
        }
    }

    public IEnumerable<Vector2> GetUVs ()
    {
        return uvMap.uvs;
    }

    public IEnumerable<int> GetTris ()
    {
        for(int i = 2; i < uvMap.uvs.Count; i++)
        {
            yield return 0;
            yield return i - 1;
            yield return i;
        }
    }


}

public enum PropType
{
    Mountain = 0,
};

