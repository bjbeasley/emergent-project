using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class MeshCombiner
{
    public static Mesh Combine (IEnumerable<Mesh> meshes)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        List<Vector2> uv1s = new List<Vector2>();

        int triOffset = 0;

        foreach(Mesh mesh in meshes)
        {
            vertices.AddRange(mesh.vertices);
            tris.AddRange(mesh.triangles.Select(t => t + triOffset));
            uvs.AddRange(mesh.uv);
            uv1s.AddRange(mesh.uv2);
            triOffset = vertices.Count;
        }

        Mesh combinedMesh = new Mesh ();
        combinedMesh.SetVertices(vertices);
        combinedMesh.SetTriangles(tris, 0);
        combinedMesh.SetUVs(0, uvs);
        if(uv1s.Count == uvs.Count)
        {
            combinedMesh.SetUVs(1, uv1s);
        }

        return combinedMesh;
    }
}

