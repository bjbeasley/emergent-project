using System;
using System.Collections.Generic;
using System.Linq;
using EmpiresCore;
using EmpiresCore.WorldGeneration;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
class PropManager : MonoBehaviour
{
    private List<Prop> props;
    private MeshFilter meshFilter;

    public float mountainWidth = 2;
    public float mountainPacking = 2;

    public PropUVPacker propUVPacker;

    private List<Vector3> vertices;
    private List<Vector2> uvs;
    private List<int> tris;

    private void Awake ()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    public void UpdateDetails (List<Province> provinces)
    {
        AddProps(provinces);

        AddMeshes();
        
    }

    private void AddProps (List<Province> provinces)
    {
        props = new List<Prop>();

        foreach(Province province in provinces)
        {
            if(province.Biome == Biome.Mountains)
            {
                AddMountains(province);
            }
        }
    }

    private void AddMeshes ()
    {
        vertices = new List<Vector3>();
        uvs = new List<Vector2>();
        tris = new List<int>();

        foreach(Prop prop in props)
        {
            int baseIndex = vertices.Count;

            foreach(Vector3 vertex in prop.GetVertices())
            {
                vertices.Add(vertex);
            }

            foreach(int index in prop.GetTris())
            {
                tris.Add(baseIndex + index);
            }

            foreach(Vector2 uv in prop.GetUVs())
            {
                uvs.Add(uv);
            }
        }

        Mesh mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(tris, 0);
        mesh.SetUVs(0, uvs);

        meshFilter.mesh = mesh;
    }

    private void AddMountains (Province province)
    {
        float scale = 1;

        bool samplesFound = false;

        List<Mountain> mountains = new List<Mountain>();

        while(!samplesFound && scale > 0.2f)
        {
            ProvinceSampler provinceSampler = new ProvinceSampler(province, mountainWidth * scale / mountainPacking,
                                mountainWidth * scale / mountainPacking, 0);

            IEnumerable<Vector3> samples = provinceSampler.AttemptToFindSamples(200)
                .Select(v => new Vector3(v.X, v.Y, -1f / 8f + province.MeanPos.Y / 4096));

            int i = 0;
            foreach(Vector3 sample in samples)
            {
                i++;
                samplesFound = true;

                if(propUVPacker == null)
                {
                    Debug.LogError("No Prop UV Packer for Prop Manager");
                    return;
                }

                PropUVMap uvMap = propUVPacker.GetPropUVMap(mountainWidth * scale, province.ID * (i + 1));

                if(uvMap != null)
                {
                    props.Add(new Prop(PropType.Mountain, province, uvMap, sample, mountainWidth));
                }

                

            }

            scale /= 2;
        }
    }

}

