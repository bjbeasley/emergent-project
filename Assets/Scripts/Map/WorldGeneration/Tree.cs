using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tree
{
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> tris = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();

    private float height;
    private float width;
    private Vector3 position;
    private Vector2 outlineUV;
    private Vector2 mainUV;
    private float outlineWidth;
    private float trunkHeight;

    private const float outlineZOffset = 0.001f;

    public Tree (float width, float height, Vector3 position, Vector2 outlineUV, Vector2 mainUV, float outlineWidth, float trunkHeight)
    {
        this.height = height;
        this.width = width;
        this.position = position;
        this.outlineUV = outlineUV;
        this.mainUV = mainUV;
        this.outlineWidth = outlineWidth;
        this.trunkHeight = trunkHeight;
    }

    public Mesh GenerateMesh ()
    {
        Mesh mesh = new Mesh();
        AddLeaveTri(outlineWidth, false);
        AddLeaveTri(0, true);
        AddTrunk();
        AddShadow();
        ApplyPsuedo3DEffect();

        AddPositionAsOffset();

        mesh.SetVertices(vertices);
        mesh.SetTriangles(tris, 0);
        mesh.SetUVs(0, uvs);
        mesh.SetUVs(1, uvs.Select(v => (Vector2)position).ToList());

        return mesh;
    }

    private void AddLeaveTri (float shrinkAmount, bool isOutline)
    {
        float z = isOutline ? outlineZOffset : 0;

        float xShadowMultiplier = isOutline ? 2 : 2.5f;

        vertices.Add(new Vector3(-width / xShadowMultiplier + shrinkAmount, trunkHeight + shrinkAmount, z));
        vertices.Add(new Vector3(0, height - shrinkAmount, z));
        vertices.Add(new Vector3(width / 2 - shrinkAmount, trunkHeight + shrinkAmount, z));
        
        AddUVs(3, isOutline);
        AddLast3VerticesAsTri();
    }

    private void AddTrunk ()
    {
        vertices.Add(new Vector3(-width / 8, 0, outlineZOffset));
        vertices.Add(new Vector3(0, height, outlineZOffset));
        vertices.Add(new Vector3(width / 8, 0, outlineZOffset));

        AddUVs(3, true);
        AddLast3VerticesAsTri();
    }

    private void AddShadow ()
    {
        vertices.Add(new Vector3(-width / 3, -outlineWidth / 2, outlineZOffset));        
        vertices.Add(new Vector3(0, outlineWidth / 2, outlineZOffset));
        vertices.Add(new Vector3(width / 3, -outlineWidth / 2, outlineZOffset));

        AddUVs(3, true);
        AddLast3VerticesAsTri();
    }

    private void AddPositionAsOffset ()
    {
        for(int i = 0; i < vertices.Count; i++)
        {
            vertices[i] += position;
        }
    }

    private void AddLast3VerticesAsTri ()
    {
        tris.Add(vertices.Count - 3);
        tris.Add(vertices.Count - 2);
        tris.Add(vertices.Count - 1);
    }

    private void AddUVs (int count, bool outline)
    {
        for(int i = 0; i < count; i++)
        {
            uvs.Add(outline ? outlineUV : mainUV);
        }
    }

    private void ApplyPsuedo3DEffect ()
    {
        for(int i = 0; i < vertices.Count; i++)
        {
            vertices[i] -= new Vector3(0, 0, vertices[i].y);
        }
    }
}
