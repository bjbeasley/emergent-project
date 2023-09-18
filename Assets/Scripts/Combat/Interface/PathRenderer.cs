using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(LineRenderer))]
public class PathRenderer : MonoBehaviour
{
    private LineRenderer lineRenderer;

    public bool Visible { get; set; }


    private void Awake ()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void ShowPath (IEnumerable<Vector2Int> gridPositions)
    {
        var grid = CombatMap.Instance.Grid;

        ShowPath(gridPositions.Select(t => grid.GridToWorldPos(t)));
    }

    public void ShowPath (IEnumerable<Vector3> worldPositions)
    {
        Visible = true;

        var array = worldPositions.ToArray();

        lineRenderer.positionCount = array.Length;
        lineRenderer.SetPositions(array);

    }


    void LateUpdate()
    {
        lineRenderer.enabled = Visible;
    }
}
