using EmpiresCore;
using EmpiresCore.WorldGeneration;
using EmpiresCore.WorldGeneration.Structures;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class DetailManager : MonoBehaviour
{

    private MeshFilter meshFilter;

    public float mountainWidth = 2;
    public float minMountainHeight = 2;
    public float maxMountainHeight = 4;
    public int mountainVerts = 10;
    public float mountainYVariation = 0.3f;
    public float mountainLineWidth = 0.1f;

    public Color outlineColor = Color.black;
    public Color mountainDarkColor = Color.gray;
    public Color mountainLightColor = Color.white;
    public Color treeColor = Color.green;

    public float mountainPacking = 2;

    private Vector2 outlineUV;
    private Vector2 mountainDarkUV;
    private Vector2 mountainLightUV;
    public Vector2 treeUV;

    public float treeWidth = 0.6f;
    public float treeHeight = 1f;
    public float treeLineWidth = 0.1f;
    public float trunkHeight = 0.3f;

    List<Province> provinces;

    private List<Mesh> meshes = new List<Mesh>();

    private void Awake ()
    {
        meshFilter = GetComponent<MeshFilter>();
        IdentifyUVs();
    }

    private void IdentifyUVs()
    {
        outlineUV = ColorLookup.GetTexCoords(outlineColor);
        mountainDarkUV = ColorLookup.GetTexCoords(mountainDarkColor);
        mountainLightUV = ColorLookup.GetTexCoords(mountainLightColor);
        treeUV = ColorLookup.GetTexCoords(treeColor);
    }

    public void UpdateMesh(List<Province> provinces)
    {
        this.provinces = provinces;
        AddDetails();
        meshFilter.mesh = MeshCombiner.Combine(meshes);
    }

    private void AddDetails ()
    {
        foreach(Province province in provinces)
        {
            if(province.Biome == Biome.Mountains)
            {
                AddMountains(province);
            }
            else if(Random.value > 0.8f)
            {
                AddTrees(province);
            }
        }
    }

    private void AddMountains (Province province)
    {
        float scale = GetBaseScale(province.MeanPos.Y);

        bool samplesFound = false;

        List<Mountain> mountains = new List<Mountain>();

        while(!samplesFound && scale > 0.2f)
        {
            ProvinceSampler provinceSampler = new ProvinceSampler(province, mountainWidth * scale / mountainPacking,
                                mountainWidth * scale * 0.65f, 0);

            IEnumerable<Vector3> samples = provinceSampler.AttemptToFindSamples(200)
                .Select(v => new Vector3(v.X, v.Y, -1f / 8f + province.MeanPos.Y / 4096));

            int i = 0;
            foreach(Vector3 sample in samples)
            {
                i++;
                samplesFound = true;

                float height = Random.Range(minMountainHeight, maxMountainHeight);

                mountains.Add(new Mountain(province.ID + i, mountainWidth * scale, height * scale,
                    mountainVerts, mountainLineWidth, mountainYVariation, sample,
                    mountainLightUV, mountainDarkUV, outlineUV));
            }

            scale /= 2;
        }

        meshes.AddRange(mountains.Select(m => m.GenerateMesh()));
    }

    private void AddTrees (Province province)
    {
        float scale = GetBaseScale(province.MeanPos.Y);
        ProvinceSampler provinceSampler = new ProvinceSampler(province, treeWidth / 2 * scale, 0, 0);

        IEnumerable<Vector3> samples = provinceSampler.AttemptToFindSamples(400)
                .Select(v => new Vector3(v.X, v.Y, -1f / 8f + province.MeanPos.Y / 4096));

        foreach(Vector3 sample in samples)
        {
            float height = Random.Range(treeHeight * 0.8f, treeHeight * 1.2f);
            float width = Random.Range(treeWidth * 0.8f, treeWidth * 1.2f);
            float trunk = Random.Range(trunkHeight * 0.75f, trunkHeight * 1.5f);

            Tree tree = new Tree(width * scale, height * scale, sample, outlineUV, treeUV, treeLineWidth, trunk);

            meshes.Add(tree.GenerateMesh());
        }
    }

    private float GetBaseScale (float y)
    {
        return ProjectionUtilities.CalculateMercatorScaleFactor(y / 256);
    }
}
