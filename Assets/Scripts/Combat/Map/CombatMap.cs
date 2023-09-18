using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
public class CombatMap : MonoBehaviour
{
    public static CombatMap Instance { get; private set; }

    public TransformGrid<CombatTile> Grid { get; private set; }

    [SerializeField]
    private Vector3 origin = Vector3.zero;
    [SerializeField]
    private Vector3 xAxis = new Vector3(1,0,0);
    [SerializeField]
    private Vector3 yAxis = new Vector3(0,1,0);
    [SerializeField]
    private Vector2Int size = new Vector2Int(10, 10);


    private Vector3 zAxis;

    private MeshFilter meshFilter;
    public Agent SelectedAgent { get; private set; }

    [SerializeField]
    private MovementVisualisation movementVisualisation;
    [SerializeField]
    private ActionVisualisation attackVisualisation;
    public MovementVisualisation MovementVisualisation { get { return movementVisualisation; } }

    public ActionVisualisation ActionVisualisation { get { return attackVisualisation; } }

    [SerializeField]
    private Transform playerAgentPrefab;
    [SerializeField]
    private Transform computerAgentPrefab;

    private List<Agent> agents = new List<Agent>();

    public CombatAI AI { get; private set; }

    
    public enum ViewMode 
    {
        None = 0,
        Movement = 1,
        Action = 2,
    }

    public ViewMode CurrentViewMode { get; private set; }


    private void Awake ()
    {
        if(Instance != null)
        {
            Debug.Log("Multiple combat maps detected");
        }
        Instance = this;
        Grid = new TransformGrid<CombatTile>(origin, xAxis, yAxis, size.x, size.y);
        zAxis = Vector3.Cross(xAxis, yAxis);
        meshFilter = GetComponent<MeshFilter>();        
        InitGrid();
        GenerateMesh();

        movementVisualisation.Initialise(size.x, size.y, meshFilter.mesh);
        attackVisualisation.Initialise(size.x, size.y, meshFilter.mesh);
        AI = new CombatAI();
    }

    private void Start ()
    {
        TurnManager.Instance.OrderUpdated += (s,e) => SetViewMode(ViewMode.Movement);

        //DialogManager.Instance.CreateDialog("Test", "Testing", new DialogOption("Option 1", () => Debug.Log("Option 1")), new DialogOption("Option 2", () => Debug.Log("Option 2")));
    }

    private void InitGrid ()
    {
        foreach(var pos in Grid.GetIndices())
        {
            Grid.Set(pos, new CombatTile());
            Grid.Get(pos).Passable = true;// Random.value > 0.05f;
        }
    }

    public void SetViewMode (ViewMode mode)
    {
        CurrentViewMode = mode;
    }

    private void OnMouseUpAsButton ()
    {
        
    }

    public void SetSelected (Agent selection)
    {
        if(SelectedAgent != selection)
        {
            if(SelectedAgent != null)
            {
                SelectedAgent.OnDeselect();
            }

            SelectedAgent = selection;


            SelectedAgent.OnSelect();
        }
    }

    private void GenerateMesh ()
    {
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        List<Vector3> offsets = new List<Vector3>
        {
            xAxis / 2 + yAxis / 2,
            xAxis / 2 - yAxis / 2,
            - xAxis / 2 - yAxis / 2,
            - xAxis / 2 + yAxis / 2,
        };

        List<int> triOffsets = new List<int>
        {
            0,1,3, //First tri
            3,1,2, //Second tri
        };

        for(int x = 0; x < size.x; x++)
        {
            for(int y = 0; y < size.y; y++)
            {
                Vector3 pos = Grid.GridToWorldPos(x, y);

                int baseIndex = verts.Count;

                offsets.ForEach(o => verts.Add(pos + o));
                triOffsets.ForEach(o => tris.Add(baseIndex + o));

                if(Grid.Get(x,y).Passable)
                {     
                    uvs.Add(Vector2.one);
                    uvs.Add(Vector2.right);
                    uvs.Add(Vector2.zero);
                    uvs.Add(Vector2.up);
                }
                else
                {
                    for(int i = 0; i < 4; i++)
                    {
                        uvs.Add(Vector2.zero);
                    }
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

    }

    private void OnDrawGizmosSelected ()
    {
        if(Grid == null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(origin, 0.1f);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin, origin + xAxis);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(origin, origin + yAxis);
        }
        else
        {
            for(int i = 0; i < size.x; i++)
            {
                for(int j = 0; j < size.y; j++)
                {
                    Vector2Int gridPos = new Vector2Int(i, j);
                    Vector3 worldPos = Grid.GridToWorldPos(gridPos);
                    Gizmos.DrawSphere(worldPos, 0.1f);                 
                }
            }
        }
    }

    public bool SpaceAvailable (Vector2Int pos)
    {
        if(!Grid.InGrid(pos))
        {
            return false;
        }

        return Grid.Get(pos).Available;
    }

    public bool SpacePassable (Vector2Int pos, Agent passingAgent)
    {
        if(!Grid.InGrid(pos))
        {
            return false;
        }

        var cell = Grid.Get(pos);

        return cell.PassableTo(passingAgent);
    }

    public void SpawnAgent (Vector2Int gridPos, bool playerControlled, Character character, bool friendly)
    {
        Agent agent = Instantiate(playerControlled ? playerAgentPrefab : computerAgentPrefab).GetComponent<Agent>();

        if(!playerControlled)
        {
            (agent as ComputerAgent).SetFriendly(friendly);
        }


        if(agent == null)
        {
            throw new System.Exception("Agent prefab does not have an agent component attached");
        }

        agent.Initialise();

        agent.SetCharacter(character);
        agent.SetPosition(gridPos);
        agents.Add(agent);
    }

    
}
