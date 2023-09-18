using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ActionVisualisation : MonoBehaviour
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

    public void ShowAction (Agent agent, Action action)
    {
        UpdateUV(agent, action);
        current = agent;
    }

    public void HideAction (Agent agent = null)
    {
        if(agent == null || agent == current)
        {
            meshFilter.mesh = null;
            current = null;
        }
    }

    private void LateUpdate ()
    {
        meshRenderer.enabled = CombatMap.Instance.CurrentViewMode == CombatMap.ViewMode.Action;
    }

    private void UpdateUV (Agent agent, Action action)
    {

        for(int i = 0; i < uvs.Length; i+=4)
        {
            uvs[i + 0] = Vector2.one / 2;
            uvs[i + 1] = Vector2.right / 2;
            uvs[i + 2] = Vector2.zero;
            uvs[i + 3] = Vector2.up / 2;
        }

        if(action == null)
        {
            return;
        }

        foreach(var target in action.Range.GetTargetAgents(agent))
        {
            OffsetUVs(target.GridPos, Vector2.one / 2);
        }

        foreach(var inRange in action.Range.GetTargetPositions(agent))
        {
            OffsetUVs(inRange, Vector2.right / 2);
        }

        mesh.SetUVs(0, uvs);
        meshFilter.mesh = mesh;
    }

    private void OffsetUVs (Vector2Int gridPos, Vector2 offset)
    {
        int index = GetIndex(gridPos);

        for(int i = 0; i < 4; i++)
        {
            uvs[index + i] += offset;
        }
    }

    private int GetIndex (Vector2Int gridPos)
    {
        return (gridPos.x * CombatMap.Instance.Grid.Height + gridPos.y) * 4;
    }
}
