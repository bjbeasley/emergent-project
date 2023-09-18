using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAgent : Agent
{
    public bool IsDragging { get; private set; }

    public override bool Friendly { get { return true; } }

    public override bool IsPlayerControlled { get { return true; } }

    [SerializeField]
    private PathRenderer pathRenderer;

    private void OnMouseDrag ()
    {
        if(IsTurn && CombatMap.Instance.CurrentViewMode == CombatMap.ViewMode.Movement)
        {
            Vector3 v = CombatCamera.Instance.GetMousePosition();
            transform.position = new Vector3(v.x, v.y, transform.position.z);

            if(pathRenderer != null)
            {
                Vector2Int target = CombatMap.Instance.Grid.WorldToGridPosInt(v);

                if(CombatMap.Instance.Grid.InGrid(target) && Pathfinder.CellReachable(target))
                {
                    var path = Pathfinder.GetPath(target);

                    pathRenderer.ShowPath(path);
                }
                else
                {
                    pathRenderer.Visible = false;
                }
            }

            if(!IsDragging)
            {
                IsDragging = true;
                CombatMap.Instance.SetSelected(this);
            }            
        }    
        else if(IsDragging)
        {
            IsDragging = false;
            if(pathRenderer != null)
            {
                pathRenderer.Visible = false;
            }
            ResetPosition();
        }
    }

    private void OnMouseUp ()
    {
        if(IsDragging)
        {
            IsDragging = false;

            if(pathRenderer != null)
            {
                pathRenderer.Visible = false;
            }

            Vector2Int target = CombatMap.Instance.Grid.WorldToGridPosInt(CombatCamera.Instance.GetMousePosition());

            if(!TryMove(target))
            {
                ResetPosition();
            }
        }
    }
}
