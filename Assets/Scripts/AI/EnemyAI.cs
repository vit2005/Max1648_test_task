using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] bool isEnemy;
    public event Action<Unit> OnActionStarted;
    public event Action OnActionFinished;
    private enum State
    {
        WaitingForEnemyTurn,
        TakingTurn,
        Busy
    }
    private State state;
    private float timer;

    private void Awake()
    {
        state = State.WaitingForEnemyTurn;
    }

    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void Update()
    {
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }
        switch (state)
        {
            case State.WaitingForEnemyTurn:
                break;
            case State.TakingTurn:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    if (TryTakeEnemyAIAction(SetStateTakingTurn))
                    {
                        state = State.Busy;
                    }
                    else
                    {
                        TurnSystem.Instance.NextTurn();
                    }
                    //TurnSystem.Instance.NextTurn();
                }
                break;
            case State.Busy:
                break;

        }

    }

    private void SetStateTakingTurn()
    {
        OnActionFinished?.Invoke();
        timer = 0.5f;
        state = State.TakingTurn;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            state = State.TakingTurn;
            timer = 2f;
        }
    }

    private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)
    {
        //Debug.Log("Take Enemy AI Action");
        List<Unit> teamates = isEnemy ?
            UnitManager.Instance.GetEnemyUnitList() :
            UnitManager.Instance.GetFriendlyUnitList();

        foreach (Unit enemyUnit in teamates)
        {
            if (TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete))
            {
                OnActionStarted?.Invoke(enemyUnit);
                return true;
            }
        }

        return false;
    }

    private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete)
    {
        EnemyAIAction bestEnemyAIAction = null;
        BaseAction bestBaseAction = null;

        enemyUnit.DebugBestBehavior();

        foreach (BaseAction baseAction in enemyUnit.GetBaseActionArray())
        {
            if (!enemyUnit.CanSpendActionPointsToTakeAction(baseAction))
            {
                continue;
            }

            if (bestEnemyAIAction == null)
            {
                bestEnemyAIAction = baseAction.GetBestEnemyAIAction();
                bestBaseAction = baseAction;
            }
            else
            {
                EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();
                if (testEnemyAIAction != null && testEnemyAIAction.actionValue > bestEnemyAIAction.actionValue)
                {
                    bestEnemyAIAction = testEnemyAIAction;
                    bestBaseAction = baseAction;
                }
            }

            // baseAction.GetBestEnemyAIAction();
        }

        if (bestEnemyAIAction != null && enemyUnit.TrySpendActionPointsToTakeAction(bestBaseAction))
        {
            bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyAIActionComplete);
            return true;
        }
        else
        {
            return false;
        }

        /* SpinAction spinAction = enemyUnit.GetSpinAction();

         GridPosition actionGridPosition = enemyUnit.GetGridPosition();

             if (!spinAction.IsValidActionGridPosition(actionGridPosition))
             {
                 return false;
             }

             if (!enemyUnit.TrySpendActionPointsToTakeAction(spinAction))
             {
                 return false;
             }

             Debug.Log("Spin Action!");
             spinAction.TakeAction(actionGridPosition, onEnemyAIActionComplete);
             return true;*/

    }
}
