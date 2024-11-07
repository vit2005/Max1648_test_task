using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
    private List<BaseHeuristic> baseHeuristicsList;
    private List<BaseBehavior> baseBehaviorsList;
    private int actionPoints = ACTION_POINTS_MAX;
    private TrainingDataCreator trainingDataCreator;
    private List<TrainingData> trainingDataList;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        baseActionArray = GetComponents<BaseAction>();
        baseHeuristicsList = new List<BaseHeuristic>
        {
            new AverageTeamatesDistanceHeuristic(this),
            new AverageEnemyDistanceHeuristic(this),
            new ClosestEnemyDistanceHeuristic(this),
            new CoverHeuristic(this),
            new HealthHeuristic(this),
            new SuperiorityHeuristic(this),
            new TotalHealthHeuristic(this)
        };
        baseBehaviorsList = new List<BaseBehavior>()
        {
            new HideBehavior(this),
            new HuntBehavior(this),
            new RushBehavior(this),
            new HitClosestEnemyBehavior(this),
        };
        trainingDataCreator = new TrainingDataCreator();
        trainingDataList = new List<TrainingData>();
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
        foreach (BaseHeuristic baseHeuristic in baseHeuristicsList)
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

    public BaseAction[] GetPlayerPlayableBaseActionArray()
    {
        return baseActionArray.Where(x => x.IsPlayerPlayable).ToArray();
    }

    public List<BaseHeuristic> GetBaseHeuristicsList()
    {
        return baseHeuristicsList;
    }

    public List<BaseBehavior> GetBaseBehaviorList()
    {
        return baseBehaviorsList;
    }

    public void DebugAllBehaviors()
    {
        Debug.Log(gameObject.name);
        foreach(var behavior in baseBehaviorsList)
        {
            Debug.Log(behavior.GetType() + ":  " + behavior.GetValue(out BaseAction action) + ": " + action.GetType());
        }
        Debug.Log("------------------------");
    }

    public void Log(int turn, BaseBehavior behavior)
    {
        var trainingData = trainingDataCreator.Generate(turn, this, behavior);
        trainingDataList.Add(trainingData);
    }

    public void SetReward(int reward)
    {
        var trainingData = trainingDataList.LastOrDefault();
        if (trainingData != null) trainingData.SetReward(reward);
    }

    public void Save()
    {
        Database.SaveTrainingData(gameObject.name, trainingDataList);
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

    public List<Unit> GetFriendsList()
    {
        return IsEnemy ?
            UnitManager.Instance.GetEnemyUnitList() :
            UnitManager.Instance.GetFriendlyUnitList();
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
        Save();
        Destroy(gameObject);


        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalized()
    {
        return healthSystem.GetHealthNormalized();
    }
}
