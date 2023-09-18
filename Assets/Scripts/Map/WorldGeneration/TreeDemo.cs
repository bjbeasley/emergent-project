using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TreeDemo : MonoBehaviour
{
    private List<Tree> trees;
    private MeshFilter meshFilter;


    public Color outlineColor = Color.black;
    public Color mainColor = Color.white;

    public float width = 1;
    public float height = 1;
    public float lineWidth = 0.1f;
    public float trunkHeight = 0.3f;

    private void Awake ()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            GenerateTrees();
        }
    }

    private void GenerateTrees ()
    {
        trees = new List<Tree>();

        for(int x = 0; x < 10; x++)
        {
            for(int y = 0; y < 10; y++)
            {
                trees.Add(new Tree(width, height, new Vector2(x, y), 
                    ColorLookup.GetTexCoords(outlineColor), ColorLookup.GetTexCoords(mainColor), lineWidth, trunkHeight));
            }
        }

        meshFilter.mesh = MeshCombiner.Combine(trees.Select(t => t.GenerateMesh()));
    }
}
