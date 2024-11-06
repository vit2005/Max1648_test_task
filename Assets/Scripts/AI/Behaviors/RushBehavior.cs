using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RushBehavior : BaseBehavior
{
    public RushBehavior(Unit unit) : base(unit) { }

    public override int GetValue(out BaseAction action)
    {
        // TODO: check Closeness and then Cluster heuristic to MoveAction, Grenade or SwordAction
        var distance = unit.GetHeuristic<ClosestEnemyDistanceHeuristic>().GetValue();
        action = unit.GetAction<ShootAction>();
        var maxDistance = (action as ShootAction).MaxShootDistance;

        Debug.Log($"RushBehavior: \nd:{distance}, max:{maxDistance}");
        float percentage = (float)distance / maxDistance;
        if (distance < maxDistance && (action as ShootAction).GetTargetCountAtPosition(unit.GetGridPosition()) > 0) 
            return (int)((1f - percentage) * 50);
        action = unit.GetAction<SpinAction>();
        return 0;
    }
}
