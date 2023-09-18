using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class RegionWrapper : MonoBehaviour
{
    private MeshFilter meshFilter;

    private void Awake ()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    public void UpdateMesh(Mesh mesh)
    {
        meshFilter.mesh = mesh;
    }
}
