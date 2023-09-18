using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using EmpiresCore;
using EmpiresCore.WorldGeneration;
using EmpiresCore.WorldGeneration.Structures;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Region : MonoBehaviour
{
    private List<Province> provinces = new List<Province>();

    private Mesh mesh;
    private List<Vec2> meshVertices = new List<Vec2>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();
    private List<Vector2> uv2s = new List<Vector2>();

    private Dictionary<int, (int, int)> provinceUVRanges = new Dictionary<int, (int, int)>();

    private float minX = 1000;
    private float maxX = -1000;
    private float midX;
    private float minY = 1000;
    private float maxY = -1000;

    public Color mountainColor = Color.gray;
    private Vector2 mountainColorUV;

    private void Awake ()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        IdentifyColorUVs();
    }

    private void IdentifyColorUVs ()
    {
        mountainColorUV = ColorLookup.GetTexCoords(mountainColor);
    }


    public void AddProvince (Province province)
    {
        lock(this)
        {
            provinces.Add(province);
            if(province.IsLand)
            {
                AddProvinceMesh(province);  
            }            
        }       
    }

    private void LateUpdate ()
    {
        if(MapCameraController.Instance == null)
        {
            return;
        }
        float x = MapCameraController.Instance.transform.position.x;

        float d0 = Mathf.Abs(x - midX - transform.position.x);
        float d1 = Mathf.Abs(x - midX - 256 - transform.position.x);
        float d2 = Mathf.Abs(x - midX + 256 - transform.position.x);

        while(d1 < d0)
        {
            transform.position += new Vector3(256, 0, 0);
            d0 = Mathf.Abs(x - midX - transform.position.x);
            d1 = Mathf.Abs(x - midX - 256 - transform.position.x);
        }
        while(d2 < d0 && d2 < d1)
        {
            transform.position -= new Vector3(256, 0, 0);
            d0 = Mathf.Abs(x - midX - transform.position.x);
            d1 = Mathf.Abs(x - midX - 256 - transform.position.x);
            d2 = Mathf.Abs(x - midX + 256 - transform.position.x);
        }
    }



    private void AddProvinceMesh (Province province)
    {
        List<Vec2> triVerts = province.GetVertices(true, 1).ToList();

        Triangulator triangulator = new Triangulator(triVerts);
        int[] newTriangles = triangulator.Triangulate();

        int startIndex = meshVertices.Count;

        for(int i = 0; i < newTriangles.Length; i++)
        {
            newTriangles[i] += startIndex;
        }

        Vector2 uv;
        if(!province.IsLand)
        {
            uv = ColorLookup.GetTexCoords(0.6f, 0.86f, 0.99f);
        }
        else
        {

            Vector3 color = (ColorLookup.RandomColor(province.ID) + Vector3.one) / 2;
            color.x *= 0.6f;
            color.z *= 0.5f;

            if(province.MeanPos.Y < 88 || province.MeanPos.Y > 168)
            {
                float factor = (province.MeanPos.Y - 128f) / 90;
                factor *= factor;
                factor *= factor;
                color = Vector3.Lerp(color, new Vector3(0.99f, 0.99f, 0.99f), factor);
            }

            if(province.MeanPos.Y > 88 && province.MeanPos.Y < 168)
            {
                Vector3 desertColor = color * 0.3f;
                desertColor.y *= 0.6f;

                desertColor.x += 0.8f;
                desertColor.y += 0.7f;
                desertColor.z += 0.55f;

                float factor = (province.MeanPos.Y - 128f) / 30;
                factor *= factor;
                color = Vector3.Lerp(desertColor, color, factor);
            }

            uv = ColorLookup.GetTexCoords(color);

            if(province.Biome == Biome.Mountains)
            {
                uv = mountainColorUV;
            }

            

        }

        for(int i = 0; i < triVerts.Count; i++)
        {
            meshVertices.Add(triVerts[i]);
            uvs.Add(uv);
            uv2s.Add(new Vector2(0.03f,0.03f));
        }
        triangles.AddRange(newTriangles);
        lock(provinceUVRanges)
        {
            provinceUVRanges.Add(province.ID, (startIndex, meshVertices.Count - 1));
        }
        
    }

    public void UpdateMesh ()
    {
        mesh.SetVertices(meshVertices.Select(v => new Vector3(v.X, v.Y, 0)).ToList());
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvs);
        mesh.SetUVs(1, uv2s);
        foreach(Vec2 vert in meshVertices)
        {
            if(vert.X < minX)
            {
                minX = vert.X;
            }
            if(vert.X > maxX)
            {
                maxX = vert.X;
            }
            if(vert.Y < minY)
            {
                minY = vert.Y;
            }
            if(vert.Y > maxY)
            {
                maxY = vert.Y;
            }
        }

        midX = (minX + maxX) / 2;

        mesh.RecalculateBounds();

        RegionWrapper[] regionWrapper = GetComponentsInChildren<RegionWrapper>();

        foreach(RegionWrapper rw in regionWrapper)
        {
            rw.UpdateMesh(mesh);
        }

        PropManager propManager = GetComponentInChildren<PropManager>();

        if(propManager != null)
        {
            propManager.UpdateDetails(provinces);
        }
    }

    public void UpdateBorders ()
    {
        BorderRenderer[] borderRenderers = GetComponentsInChildren<BorderRenderer>();

        foreach(BorderRenderer br in borderRenderers)
        {
            br.UpdateBorders(provinces);
        }
    }

    public enum ProvinceUV
    {
        Zero = 0,
        One = 1,
        WorldSpace = 2,
    }


    public void SetUVs (Province province, ProvinceUV uv)
    {
        if(provinceUVRanges.TryGetValue(province.ID, out (int, int)range))
        {
            var (uvMin, uvMax) = range;
            for(int i = uvMin; i <= uvMax; i++)
            {
               
                Vec2 vert = meshVertices[i];

                Vector2 pos;

                switch(uv)
                {
                    case ProvinceUV.Zero:
                        pos = Vector2.one * 0.02f;
                        break;
                    case ProvinceUV.One:
                        pos = Vector2.one * 0.98f;
                        break;
                    case ProvinceUV.WorldSpace:
                        pos = new Vector2(vert.X * 2, vert.Y * 2);
                        break;
                    default:
                        pos = Vector2.zero;
                        break;
                }

                

                uv2s[i] = pos;
            }
            mesh.SetUVs(1, uv2s);
        }
    }

    private void Update ()
    {
        //if(mesh.vertices.Length > 0 && Random.value > 0.9f)
        //{
        //    int index = Random.Range(0, provinces.Count);

        //    SetUVs(provinces[index]);
        //}
    }

    public Province GetProvinceAtPosition (Vector2 pos)
    {
        if(!PointInsideRegion(pos))
        {
            return null;
        }
        foreach(Province p in provinces)
        {
            if(p.PointInsideProvince(new Vec2(pos.x, pos.y)))
            {
                return p;
            }

            var pos2 = new Vec2(pos.x + (pos.x > 128 ? -256 : 256), pos.y);

            if(p.PointInsideProvince(pos2))
            {
                return p;
            }
            
        }
        return null;
    }

    public bool PointInsideRegion (Vector2 point, bool tryShifted = true)
    {
        if(point.x > minX && point.x < maxX &&
           point.y > minY && point.y < maxY)
        {
            return true;
        }
        else if(tryShifted)
        {
            var pos2 = new Vector2(point.x + (point.x > 128 ? -256 : 256), point.y);

            return PointInsideRegion(pos2, false);
        }
        return false;
    }
}
