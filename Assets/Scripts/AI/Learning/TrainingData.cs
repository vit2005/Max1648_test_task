using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public enum ActionType
{
    Grenade = 0,
    Interact = 1,
    MoveDefencive = 2,
    MoveOffencive = 3,
    Shoot = 4,
    Spin = 5,
    Sword = 6,

}

[Serializable]
public enum HeuristicType
{
    AverageEnemyDistance = 0,
    AverageTeamatesDistance = 1,
    ClosestEnemyDistance = 2,
    Cluster = 3,
    Cover = 4,
    Health = 5,
    Superiority = 6,

}

[Serializable]
public enum BehaviorType
{
    Hide = 0,
    HitClosestEnemy = 1,
    Hunt = 2,
    Rush = 3,

}

[Serializable]
public class HeuristicData
{
    public HeuristicType heuristic;
    public int value;
}

[Serializable]
public class BehaviorData
{
    public BehaviorType behavior;
    public int value;
}

[Serializable]
public class TrainingData
{
    public int turn;
    public GridPosition position;
    public HeuristicData[] heuristicDatas;
    public BehaviorData[] behaviorDatas;
    public BehaviorType behaviorChosen;
    public float reward;

    public TrainingData(int turn, GridPosition position, HeuristicData[] heuristicDatas,
        BehaviorData[] behaviorDatas, BehaviorType behaviorChosen)
    {
        this.turn = turn;
        this.position = position;
        this.heuristicDatas = heuristicDatas;
        this.behaviorDatas = behaviorDatas;
        this.behaviorChosen = behaviorChosen;
    }

    public void SetReward(float reward)
    {
        this.reward = reward;
    }
}

[Serializable]
public class TrainingDataWrapper
{
    public List<TrainingData> trainingDataList;

    public TrainingDataWrapper(List<TrainingData> trainingDataList)
    {
        this.trainingDataList = trainingDataList;
    }
}
