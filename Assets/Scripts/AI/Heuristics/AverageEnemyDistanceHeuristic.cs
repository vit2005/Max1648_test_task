using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AverageEnemyDistanceHeuristic : BaseHeuristic
{
    public AverageEnemyDistanceHeuristic(Unit unit) : base(unit) { }

    public override int GetValue()
    {
        var unitPosition = unit.GetGridPosition();
        return (int)unit.GetEnemiesList()
        .Select(enemy => unitPosition.DistanceTo(enemy.GetGridPosition()))
        .Average();
    }
}
