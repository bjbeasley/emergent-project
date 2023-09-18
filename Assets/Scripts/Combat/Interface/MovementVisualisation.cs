using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MovementVisualisation : MonoBehaviour
{

    private Vector2[] uvs;

    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private Agent current;



    public void Initialise (int width, int height, Mesh meshToCopy)
    {
        mesh = new Mesh();

        mesh.vertices = meshToCopy.vertices;
        mesh.triangles = meshToCopy.triangles;

        uvs = new Vector2[mesh.vertices.Length];
        mesh.uv = uvs;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter.mesh = mesh;
    }

    public void ShowMovement (Agent agent)
    {
        UpdateUV(agent);
        current = agent;
    }

    public void HideMovement (Agent agent = null)
    {
        if(agent == null || agent == current)
        {
            meshFilter.mesh = null;
            current = null;
        }
    }

    private void LateUpdate ()
    {
        meshRenderer.enabled = CombatMap.Instance.CurrentViewMode == CombatMap.ViewMode.Movement;
    }

    private void UpdateUV (Agent agent)
    {
        int index = 0;        

        foreach(var cell in CombatMap.Instance.Grid.GetIndices())
        {
            if(agent.Pathfinder.CellReachable(cell))
            {
                uvs[index + 0] = Vector2.one;
                uvs[index + 1] = Vector2.right;
                uvs[index + 2] = Vector2.zero;
                uvs[index + 3] = Vector2.up;
            }
            else
            {
                uvs[index + 0] = Vector2.zero;
                uvs[index + 1] = Vector2.zero;
                uvs[index + 2] = Vector2.zero;
                uvs[index + 3] = Vector2.zero;
            }

            index += 4;
        }

        mesh.SetUVs(0, uvs);
        meshFilter.mesh = mesh;
    }
}
