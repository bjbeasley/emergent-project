using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class LiteLineRenderer : MonoBehaviour
{
    private Mesh mesh;

    private List<Vector3> vertices;
    private List<int> tris;
    private List<Vector2> uvs;

    private Vector3 edgeVector;
    private Vector3 perpVec;
    private Vector3 position;
    private Vector3 rightOffset;

    private const float sqrt3over2 = 0.86602540378f;

    private void Awake ()
    {
        Reset();
    }

    public void Reset ()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        vertices = new List<Vector3>();
        tris = new List<int>();
        uvs = new List<Vector2>();
    }

    public void AddLine (Vector3 start, Vector3 end, float leftWidth, float rightWidth, 
        float rightZOffset = 0, bool join = false, bool startCap = false, bool endCap = false)
    {
        edgeVector = (end - start).normalized;
        perpVec = new Vector3(-edgeVector.y, edgeVector.x) * (rightWidth - leftWidth) / 2;
        AddLine(start - perpVec, end - perpVec, (leftWidth + rightWidth) / 2, rightZOffset, join,
            startCap, endCap);
    }

    public void AddLine (Vector3 start, Vector3 end, float radius,
        float rightZOffset = 0, bool join = false, bool startCap = false, bool endCap = false)
    {
        

        position = end;

        if(start == end)
        {
            return;
        }

        rightOffset = new Vector3(0, 0, rightZOffset);

        edgeVector = (end - start).normalized * radius;
        perpVec = new Vector3(-edgeVector.y, edgeVector.x);      

        Vector3 a = start + perpVec;
        Vector3 b = start - perpVec + rightOffset;
        Vector3 c = end + perpVec;
        Vector3 d = end - perpVec + rightOffset;

        if(!join && startCap)
        {
            AddStartCap(a, b, start);
        }
        else
        {
            vertices.Add(a);
            uvs.Add(Vector2.zero);
            vertices.Add(b);
            uvs.Add(Vector2.one);
        }


        vertices.Add(c);
        uvs.Add(Vector2.zero);
        vertices.Add(d);
        uvs.Add(Vector2.one);

        AddLineRect();

        if(join)
        {
            AddJoin();
        }

        if(endCap)
        {
            AddEndCap();
        }
    }

    private void AddLineRect ()
    {
        tris.Add(vertices.Count - 2);
        tris.Add(vertices.Count - 3);
        tris.Add(vertices.Count - 4);

        tris.Add(vertices.Count - 3);
        tris.Add(vertices.Count - 2);
        tris.Add(vertices.Count - 1);
    }

    private void AddJoin ()
    {
        tris.Add(vertices.Count - 4);
        tris.Add(vertices.Count - 3);
        tris.Add(vertices.Count - 5);

        tris.Add(vertices.Count - 3);
        tris.Add(vertices.Count - 4);
        tris.Add(vertices.Count - 6);
    }

    private void AddEndCap ()
    {
        Vector3 a = position + 0.5f * perpVec + sqrt3over2 * edgeVector + rightOffset * 0.25f;
        Vector3 b = position - 0.5f * perpVec + sqrt3over2 * edgeVector + rightOffset * 0.75f;

        vertices.Add(a);
        uvs.Add(Vector2.one * 0.25f);
        vertices.Add(b);
        uvs.Add(Vector2.one * 0.75f);

        tris.Add(vertices.Count - 3);
        tris.Add(vertices.Count - 2);
        tris.Add(vertices.Count - 1);

        tris.Add(vertices.Count - 4);
        tris.Add(vertices.Count - 2);
        tris.Add(vertices.Count - 3);

    }

    private void AddStartCap (Vector3 a, Vector3 b, Vector3 startPos)
    {
        Vector3 capA = startPos + 0.5f * perpVec - sqrt3over2 * edgeVector + rightOffset * 0.25f;
        Vector3 capB = startPos - 0.5f * perpVec - sqrt3over2 * edgeVector + rightOffset * 0.75f;

        vertices.Add(capA);
        uvs.Add(Vector2.one * 0.25f);
        vertices.Add(capB);
        uvs.Add(Vector2.one * 0.75f);
        vertices.Add(a);
        uvs.Add(Vector2.zero);
        vertices.Add(b);
        uvs.Add(Vector2.one);

        tris.Add(vertices.Count - 3);
        tris.Add(vertices.Count - 2);
        tris.Add(vertices.Count - 1);

        tris.Add(vertices.Count - 4);
        tris.Add(vertices.Count - 2);
        tris.Add(vertices.Count - 3);
    }



    public void Apply ()
    {
        mesh.vertices = vertices.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateBounds();
    }
}
