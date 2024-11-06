using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AverageTeamatesDistanceHeuristic : BaseHeuristic
{
    public AverageTeamatesDistanceHeuristic(Unit unit) : base(unit) { }

    public override int GetValue()
    {
        var unitPosition = unit.GetGridPosition();
        return (int)unit.GetFriendsList()
        .Select(enemy => unitPosition.DistanceTo(enemy.GetGridPosition()))
        .Average();
    }
}
