using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class MountainDemo : MonoBehaviour
{
    Mountain[] mountains = new Mountain[100];
    private MeshFilter meshFilter;

    public float width = 10;
    public float height = 10;
    public int numVertices = 9;
    public float stripeMultiplier = 10;
    public float yVariation = 0.3f;

    private void Awake ()
    {
        meshFilter = GetComponent<MeshFilter>();
        GenerateMountains();
        meshFilter.mesh = mountains[0].GenerateMesh();
    }

    private void GenerateMountains ()
    {
        for(int i = 0; i < 100; i++)
        {
            //mountains[i] = new Mountain(Random.Range(0, int.MaxValue),
            //    width, height, numVertices, stripeMultiplier, yVariation,
            //    new Vector3(i % 20 * 5 + i/2 * 1, i / 19 * 3 + i % 2 * 2, i / 19 * 3 + i % 2 * 2));
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            GenerateMountains();
            List<Mesh> meshes = new List<Mesh>();
            for(int i = 0; i < 100; i++)
            {
                meshes.Add(mountains[i].GenerateMesh());
            }
            meshFilter.mesh = MeshCombiner.Combine(meshes);
            

        }
        for(int i = 0; i < 10; i++)
        {
            for(int j = 0; j < 10; j++)
            {
                if(mountains[i * 10 + j] != null)
                {
                    mountains[i*10 + j].DrawDebugLines(new Vector2(i * 100, j * 50));
                }
            }
        }
    }
}
