using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Agent : MonoBehaviour
{
    private Vector2Int gridPos;
    public Vector2Int GridPos { get { return gridPos; } }

    public bool IsTurn { get { return TurnManager.Instance != null && TurnManager.Instance.CurrentAgent == this; } }
    public bool IsSelected { get { return CombatMap.Instance.SelectedAgent == this; } }

    public float AvailableMovement { get; private set; } = 4;

    public AgentPathfinder Pathfinder { get; private set; }

    public Character Character { get; private set; }

    public abstract bool Friendly { get; }
    public abstract bool IsPlayerControlled { get; }

    [SerializeField]
    private PortraitToken token;

    protected virtual void OnTurnStarted () { }
    protected virtual void OnTurnEnded () { }
    protected virtual void OnSelected () { }
    protected virtual void OnDeselected () { }


    private float blinkTimer = 0;

    private Action currentAction = null;

    public Action SelectedAction { get { return currentAction; } }

    private int actionsRemaining = 0;
    public int ActionsRemaining { get { return actionsRemaining; } }

    public void Initialise()
    {
        blinkTimer = Random.value * 4;
        AvailableMovement = 0;
        Pathfinder = new AgentPathfinder(CombatMap.Instance.Grid.Width, CombatMap.Instance.Grid.Height, this);

        

        SetPosition(GridPos);

        Pathfinder.Update();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBlink();

        if(CombatMap.Instance.Grid.Get(gridPos).Agent != this)
        {
            gridPos = CombatMap.Instance.Grid.FindNearest(GridPos, t => t.Available);
            SetPosition(gridPos);
        }
    }

    private void UpdateBlink ()
    {
        if(Character.HP > 0)
        {
            if(blinkTimer <= 0 && Character != null)
            {
                bool open = !Character.Portrait.EyesOpen;
                Character.Portrait.EyesOpen = open;
                Character.Portrait.Generate();
                blinkTimer = (open ? (Random.value * 2 + 6) : (Random.value * 0.05f + 0.1f));

            }
            else
            {
                blinkTimer -= Time.deltaTime;
            }
        }
        else
        {
            if(Character.Portrait.EyesOpen)
            {
                Character.Portrait.EyesOpen = false;
                Character.Portrait.Generate();
            }
        }
    }

    public void SetCharacter (Character character)
    {
        Character = character;
        Character.TempShieldPoints = 0;
        Character.ShieldPoints = Character.MaxShieldPoints;
        if(token != null)
        {
            token.AssignPortrait(character, Friendly);
        }
        TurnManager.Instance.RegisterAgent(this);
    }

    public void OnTurnEnd ()
    {
        OnTurnEnded();
    }

    public void OnTurnStart ()
    {
        Character.TempShieldPoints = 0;
        actionsRemaining = 1;
        AvailableMovement = Character.Movement;
        Pathfinder.Update();
        TryShowMovement();

        OnTurnStarted();
    }


    private void OnMouseUpAsButton ()
    {
        var mode = CombatMap.Instance.CurrentViewMode;
        if(mode == CombatMap.ViewMode.Movement)
        {
            CombatMap.Instance.SetSelected(this);
        }
        else if(mode == CombatMap.ViewMode.Action)
        {
            var attacker = TurnManager.Instance.CurrentAgent;
            if(attacker.IsPlayerControlled && attacker.CanTargetAgent(this))
            {
                attacker.PerformSelectedAction(this);   
            }
                
        }
    }

    private bool CanTargetAgent (Agent target)
    {
        return currentAction != null && currentAction.Range.CanTarget(this, target);
    }

    public void SelectAction (Action action)
    {
        if(actionsRemaining < 1)
        {
            Debug.LogWarning("Action selected when agent has no remaining actions");
            return;
        }

        currentAction = action;
        CombatMap.Instance.ActionVisualisation.ShowAction(this, action);
        CombatMap.Instance.SetViewMode(CombatMap.ViewMode.Action);

        if(action.Range.Mode == Range.RangeMode.Self)
        {
            PerformSelectedAction(this);
        }
    }

    public void CancelAction ()
    {
        currentAction = null;
        CombatMap.Instance.SetViewMode(CombatMap.ViewMode.Movement);
    }

    protected void PerformSelectedAction (Agent target)
    {
        actionsRemaining--;

        if(currentAction == null)
        {
            return;
        }

        KickAnimation((target.transform.position - transform.position));

        currentAction.Effect.Apply(target.gridPos);

        CombatMap.Instance.SetViewMode(CombatMap.ViewMode.Movement);

        currentAction = null;
    }

    public void KickAnimation (Vector2 direction)
    {
        StartCoroutine(AttackKickCoroutine(direction.normalized));
    }

    private IEnumerator AttackKickCoroutine (Vector2 dir)
    {
        float time = 0;

        Vector3 startPos = transform.position;

        while(time < 0.1f)
        {
            float t = time * 10;
            float y = 2 * t - 1;
            y = 1 - y * y;

            transform.position = startPos + (Vector3)(dir * y * 0.2f);

            time += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.position = CombatMap.Instance.Grid.GridToWorldPos(GridPos);
    }

    public void Damage (int shieldDamage, int healthDamage, int ignoreShield = 0)
    {
        StartCoroutine(DamageShakeCoroutine());


        if(Character.ShieldPoints + Character.TempShieldPoints > ignoreShield)
        {
            DamageShield(shieldDamage);
        }
        else
        {
            Character.HP = Mathf.Max(Character.HP - healthDamage, 0);
        }

        if(Character.HP <= 0)
        {
            TurnManager.Instance.RemoveAgent(this);
        }
        
    }

    private void DamageShield (int shieldDamage)
    {
        if(Character.TempShieldPoints > 0)
        {
            Character.TempShieldPoints -= shieldDamage;
            if(Character.TempShieldPoints < 0)
            {
                shieldDamage = -Character.TempShieldPoints;
                Character.TempShieldPoints = 0;
            }
            else
            {
                return;
            }
        }
        Character.ShieldPoints = Mathf.Max(Character.ShieldPoints - shieldDamage, 0);
    }

    private IEnumerator DamageShakeCoroutine ()
    {
        float time = 0;
        Vector3 startPos = transform.position;

        yield return new WaitForSecondsRealtime(0.05f);

        while(time < 0.2f)
        {
            float t = time * 5;
            float offset = Mathf.Sin(t * 15);
            float scaled = offset * (1 - t);

            transform.position = startPos + (Vector3.right * scaled * 0.3f);

            time += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }

        transform.position = CombatMap.Instance.Grid.GridToWorldPos(GridPos);
    }

    public void OnSelect ()
    {
        TryShowMovement();
        OnSelected();
    }

    public void OnDeselect ()
    {
        CombatMap.Instance.MovementVisualisation.HideMovement(this);
        OnDeselected();
    }

    private bool TryShowMovement ()
    {
        if(IsTurn && IsSelected)
        {
            CombatMap.Instance.MovementVisualisation.ShowMovement(this);
            CombatMap.Instance.ActionVisualisation.ShowAction(this, Character.Actions.FirstOrDefault());
            return true;
        }
        return false;
    }

    public bool PassableTo (Agent agent)
    {
        return agent == null || agent.Friendly == Friendly;
    }

    public void SetPosition (Vector2Int newGridPos)
    {
        
        CombatMap.Instance.Grid.Get(gridPos).RemoveAgent(this);

        gridPos = newGridPos;


        ResetPosition();
        CombatMap.Instance.Grid.Get(gridPos).Agent = this;



        Pathfinder.Update();
        TryShowMovement();
    }


    public bool TryMove (Vector2Int newGridPos)
    {
        if(!CombatMap.Instance.Grid.InGrid(newGridPos) || !Pathfinder.CellReachable(newGridPos))
        {
            return false;                     
        }

        AvailableMovement -= Pathfinder.GetDistance(newGridPos);
        SetPosition(newGridPos);

        return true;
    }

    public void ResetPosition ()
    {                
        transform.position = CombatMap.Instance.Grid.GridToWorldPos(gridPos);
    }


    private void OnMouseOver ()
    {
        token.OnHover();
    }
}
