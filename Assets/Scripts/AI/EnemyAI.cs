using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private Unit currentUnit;
    private int preReward;

    private void Awake()
    {
        state = State.WaitingForEnemyTurn;
    }

    private void Start()
    {
        if (!isEnemy)
        {
            if (PlayerPrefs.GetInt("isAuto") == 0)
            {
                enabled = false;
                return;
            }
            else
            {
                TurnSystem_OnTurnChanged(null, null);
            }
        }
        

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        UnitManager.Instance.OnAllEnemiesDead += Instance_OnAllEnemiesDead;
    }

    private void Instance_OnAllEnemiesDead(bool obj)
    {
        enabled = false;
        List<Unit> teamates = isEnemy ?
            UnitManager.Instance.GetEnemyUnitList() :
            UnitManager.Instance.GetFriendlyUnitList();
        foreach (var unit in teamates)
        {
            unit.Save();
        }
    }

    private void Update()
    {
        if (TurnSystem.Instance.IsPlayerTurn() == isEnemy)
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
        currentUnit.SetReward(preReward - CalculateReward());
        OnActionFinished?.Invoke();
        timer = 0.5f;
        state = State.TakingTurn;
    }

    private int CalculateReward()
    {
        //var friends = currentUnit.GetFriendsList();
        var enemies = currentUnit.GetEnemiesList();

        //float teammatesHealth = friends.Sum(x => x.GetHealthNormalized() * 100f);
        float enemiesHealth = enemies.Sum(x => x.GetHealthNormalized() * 100f);
        // calculating for 3 vs 3
        int reward = (int)enemiesHealth;
        return reward;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (!(!TurnSystem.Instance.IsPlayerTurn() ^ isEnemy))
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

        foreach (Unit unit in teamates)
        {
            currentUnit = unit;
            preReward = CalculateReward();
            if (TryTakeEnemyAIAction(unit, onEnemyAIActionComplete))
            {
                OnActionStarted?.Invoke(unit);
                return true;
            }
        }

        return false;
    }

    private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete)
    {
        if (!enabled) return false;

        EnemyAIAction bestEnemyAIAction = null;
        BaseAction bestBaseAction = null;

        //DefaultActionChoosing(enemyUnit, ref bestEnemyAIAction, ref bestBaseAction);
        BehaviorActionChoosing(enemyUnit, ref bestEnemyAIAction, ref bestBaseAction, out BaseBehavior choosenBehavior);

        if (bestEnemyAIAction != null && enemyUnit.TrySpendActionPointsToTakeAction(bestBaseAction))
        {
            enemyUnit.Log(TurnSystem.Instance.Turn, choosenBehavior);
            bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyAIActionComplete);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void DefaultActionChoosing(Unit enemyUnit, ref EnemyAIAction bestEnemyAIAction, ref BaseAction bestBaseAction)
    {
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
                if (bestEnemyAIAction != null) Debug.Log(baseAction.GetType() + ": " + bestEnemyAIAction.actionValue);
            }
            else
            {
                EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();
                if (testEnemyAIAction != null) Debug.Log(baseAction.GetType() + ": " + testEnemyAIAction.actionValue);
                if (testEnemyAIAction != null && testEnemyAIAction.actionValue > bestEnemyAIAction.actionValue)
                {
                    bestEnemyAIAction = testEnemyAIAction;
                    bestBaseAction = baseAction;
                }
            }
            // baseAction.GetBestEnemyAIAction();
        }
        //Debug.Log("BEST: "+ bestBaseAction.GetType() + ": " + bestEnemyAIAction.actionValue);
        //Debug.Log("-------------------------");
    }

    private void BehaviorActionChoosing(Unit enemyUnit, ref EnemyAIAction bestEnemyAIAction, ref BaseAction bestBaseAction, out BaseBehavior choosenBehavior)
    {
        enemyUnit.DebugAllBehaviors();

        choosenBehavior = ChooseBehaviorUsingSoftmax(enemyUnit.GetBaseBehaviorList());
        //BaseBehavior choosen = ChooseBehaviorUsingEpsilonGreedy(enemyUnit.GetBaseBehaviorList());
        int value = choosenBehavior.GetValue(out bestBaseAction);
        bestEnemyAIAction = bestBaseAction.GetBestEnemyAIAction();

        Debug.Log(">>> CHOOSED BEST: " + choosenBehavior.GetType() + " - " + bestBaseAction.GetType() + " - " + value);
    }

    public BaseBehavior ChooseBehaviorUsingSoftmax(List<BaseBehavior> baseBehaviorsList, float temperature = 1.0f)
    {
        // Обчислюємо значення Softmax для кожного поведінки
        float[] probabilities = new float[baseBehaviorsList.Count];
        float sum = 0f;

        for (int i = 0; i < baseBehaviorsList.Count; i++)
        {
            probabilities[i] = Mathf.Exp(baseBehaviorsList[i].GetValue(out _) / temperature);
            sum += probabilities[i];
        }

        // Нормалізуємо ймовірності
        for (int i = 0; i < probabilities.Length; i++)
        {
            probabilities[i] /= sum;
        }

        // Вибір випадкової поведінки на основі ймовірностей
        float randomValue = UnityEngine.Random.value;
        float cumulative = 0f;

        for (int i = 0; i < baseBehaviorsList.Count; i++)
        {
            cumulative += probabilities[i];
            if (randomValue <= cumulative)
            {
                return baseBehaviorsList[i];
            }
        }

        // Повертаємо останню поведінку за замовчуванням, якщо не вибрано жодну
        return baseBehaviorsList[baseBehaviorsList.Count - 1];
    }

    public BaseBehavior ChooseBehaviorUsingEpsilonGreedy(List<BaseBehavior> baseBehaviorsList, float epsilon = 0.1f)
    {
        float randomValue = UnityEngine.Random.value;

        if (randomValue < epsilon)
        {
            // Вибір випадкової поведінки
            int randomIndex = UnityEngine.Random.Range(0, baseBehaviorsList.Count);
            return baseBehaviorsList[randomIndex];
        }
        else
        {
            // Вибір найкращої поведінки на основі GetValue()
            BaseBehavior bestBehavior = baseBehaviorsList[0];
            int bestValue = bestBehavior.GetValue(out _);

            for (int i = 1; i < baseBehaviorsList.Count; i++)
            {
                int value = baseBehaviorsList[i].GetValue(out _);
                if (value > bestValue)
                {
                    bestBehavior = baseBehaviorsList[i];
                    bestValue = value;
                }
            }

            return bestBehavior;
        }
    }

    // greater value means player plays more aggressive now in conparison to average
    public int CompareAggressiveness()
    {
        int predictedAverageHealth = Prediction.Instance.GetAverageTotalPlayerHealth(TurnSystem.Instance.Turn-1);
        var currentAverageHealth = UnitManager.Instance.GetFriendlyUnitList()[0].GetHeuristic<TotalHealthHeuristic>().GetValue();
        
        return predictedAverageHealth - currentAverageHealth;
    }

}
