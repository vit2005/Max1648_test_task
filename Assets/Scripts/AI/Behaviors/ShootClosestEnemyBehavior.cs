using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootClosestEnemyBehavior : BaseBehavior
{
    public ShootClosestEnemyBehavior(Unit unit) : base(unit) { }

    // return from 0 to 50
    public override int GetValue(out BaseAction action)
    {
        // check Closeness
        var distance = unit.GetHeuristic<ClosenessHeuristic>().GetValue();
        ShootAction shootAction = unit.GetAction<ShootAction>();
        action = shootAction;
        var maxDistance = shootAction.MaxShootDistance;

        Debug.Log($"ShootClosestEnemyBehavior: \nd:{distance}, max:{maxDistance}");
        float percentage = (float)distance / maxDistance;
        if (distance < maxDistance) return (int)(percentage * 50);
        action = unit.GetAction<SpinAction>();
        return 0;
    }
}
