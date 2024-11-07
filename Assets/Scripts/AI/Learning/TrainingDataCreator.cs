using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrainingDataCreator
{
    public TrainingData Generate(int turn, Unit unit, BaseBehavior choosenBehavior)
    {
        HeuristicData[] heuristicDatas = GetHeuristicDataArray(unit);
        BehaviorData[] behaviorDatas = GetBehaviorDataArray(unit);
        TrainingData result = new TrainingData(turn, unit.GetGridPosition(), 
            heuristicDatas, behaviorDatas, GetBehaviorType(choosenBehavior));
        return result;
    }

    #region Heuristics
    public HeuristicData[] GetHeuristicDataArray(Unit unit)
    {
        var heuristics = unit.GetBaseHeuristicsList();
        return heuristics.Select(heuristic => GetHeuristicData(heuristic)).ToArray();
    }

    private HeuristicData GetHeuristicData(BaseHeuristic heuristic)
    {
        return new HeuristicData() { heuristic = GetHeuristicType(heuristic), value = heuristic.GetValue() };
    }

    public HeuristicType GetHeuristicType(BaseHeuristic heuristic)
    {
        if (heuristic is AverageEnemyDistanceHeuristic)
            return HeuristicType.AverageEnemyDistance;
        else if (heuristic is AverageTeamatesDistanceHeuristic)
            return HeuristicType.AverageTeamatesDistance;
        else if (heuristic is ClosestEnemyDistanceHeuristic)
            return HeuristicType.ClosestEnemyDistance;
        else if (heuristic is ClusterHeuristic)
            return HeuristicType.Cluster;
        else if (heuristic is CoverHeuristic)
            return HeuristicType.Cover;
        else if (heuristic is HealthHeuristic)
            return HeuristicType.Health;
        else if (heuristic is SuperiorityHeuristic)
            return HeuristicType.Superiority;
        else if (heuristic is TotalHealthHeuristic)
            return HeuristicType.TotalHealth;
        else
            throw new ArgumentException("Unknown heuristic type");
    }
    #endregion

    #region Behaviors
    public BehaviorData[] GetBehaviorDataArray(Unit unit)
    {
        var behaviors = unit.GetBaseBehaviorList();
        return behaviors.Select(behavior => GetBehaviorData(behavior)).ToArray();
    }

    private BehaviorData GetBehaviorData(BaseBehavior behavior)
    {
        return new BehaviorData() { behavior = GetBehaviorType(behavior), value = behavior.GetValue(out _) };
    }

    public BehaviorType GetBehaviorType(BaseBehavior behavior)
    {
        if (behavior is HideBehavior)
            return BehaviorType.Hide;
        else if (behavior is HitClosestEnemyBehavior)
            return BehaviorType.HitClosestEnemy;
        else if (behavior is HuntBehavior)
            return BehaviorType.Hunt;
        else if (behavior is RushBehavior)
            return BehaviorType.Rush;
        else
            throw new ArgumentException("Unknown behavior type");
    }
    #endregion
}
