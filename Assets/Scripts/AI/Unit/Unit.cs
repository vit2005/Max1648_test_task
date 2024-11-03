using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private const int ACTION_POINTS_MAX = 2;

    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;


    [SerializeField] private bool isEnemy;
    public bool IsEnemy => isEnemy;

    private GridPosition gridPosition;
    private HealthSystem healthSystem;
    private BaseAction[] baseActionArray;
    private List<BaseHeuristic> baseHeuristicArray;
    private List<BaseBehavior> baseBehaviorsArray;
    private int actionPoints = ACTION_POINTS_MAX;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        baseActionArray = GetComponents<BaseAction>();
        baseHeuristicArray = new List<BaseHeuristic>
        {
            new ClosenessHeuristic(this),
            new ClusterHeuristic(this),
            new CoverHeuristic(this),
            new HealthHeuristic(this),
            new SuperiorityHeuristic(this)
        };
        baseBehaviorsArray = new List<BaseBehavior>()
        {
            new HideBehavior(this),
            new HuntBehavior(this),
            new RushBehavior(this),
            new ShootClosestEnemyBehavior(this),
        };
    }
    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        healthSystem.OnDead += HealthSystem_OnDead;

        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
    }
    private void Update()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != gridPosition)
        {
            GridPosition oldGridPosition = gridPosition;
            gridPosition = newGridPosition;
            LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newGridPosition);

        }
    }

    public T GetAction<T>() where T : BaseAction
    {
        foreach (BaseAction baseAction in baseActionArray)
        {
            if (baseAction is T)
            {
                return (T)baseAction;
            }
        }
        return null;
    }

    public T GetHeuristic<T>() where T : BaseHeuristic
    {
        foreach (BaseHeuristic baseHeuristic in baseHeuristicArray)
        {
            if (baseHeuristic is T)
            {
                return (T)baseHeuristic;
            }
        }
        return null;
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    public BaseAction[] GetBaseActionArray()
    {
        return baseActionArray;
    }

    public void DebugBestBehavior()
    {
        Debug.Log(gameObject.name);
        foreach(var behavior in baseBehaviorsArray)
        {
            Debug.Log(behavior.GetType() + ":  " + behavior.GetValue(out _));
        }
        Debug.Log("------------------------");
    }

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if (CanSpendActionPointsToTakeAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointsCost());
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction)
    {
        // return actionPoints >= baseAction.GetActionPointsCost();
        if (actionPoints >= baseAction.GetActionPointsCost())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SpendActionPoints(int amount)
    {
        actionPoints -= amount;

        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetActionPoints()
    {
        return actionPoints;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if ((IsEnemy && !TurnSystem.Instance.IsPlayerTurn()) ||
            (!IsEnemy && TurnSystem.Instance.IsPlayerTurn()))
        {
            actionPoints = ACTION_POINTS_MAX;

            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public List<Unit> GetEnemiesList()
    {
        return IsEnemy ? 
            UnitManager.Instance.GetFriendlyUnitList() : 
            UnitManager.Instance.GetEnemyUnitList();
    }

    public Unit GetClosestEnemy()
    {
        List<Unit> enemiesList = GetEnemiesList();

        var currPos = GetGridPosition();
        var closest = enemiesList.OrderBy(unit => currPos.DistanceTo(unit.GetGridPosition())) // Сортуємо за відстанню
            .FirstOrDefault(); // Повертаємо найближчого юніта або null, якщо список порожній

        return closest;
    }

    public void Damage(int damageAmount)
    {
        healthSystem.Damage(damageAmount);
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);

        Destroy(gameObject);


        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalized()
    {
        return healthSystem.GetHealthNormalized();
    }
}
