using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitClosestEnemyBehavior : BaseBehavior
{
    public HitClosestEnemyBehavior(Unit unit) : base(unit) { }

    // return from 0 to 50
    public override int GetValue(out BaseAction action)
    {
        // check Closeness
        var distance = unit.GetHeuristic<ClosestEnemyDistanceHeuristic>().GetValue();
        SwordAction swordAction = unit.GetAction<SwordAction>();
        action = swordAction;
        var maxSwordDistance = swordAction.MaxSwordDistance;
        Debug.Log($"ShootClosestEnemyBehavior: \nd:{distance}, maxSword:{maxSwordDistance}");
        float percentage = (float)distance / maxSwordDistance;
        if (swordAction.GetTargetCountAtPosition() > 0)
            return (int)(percentage * 50);


        ShootAction shootAction = unit.GetAction<ShootAction>();
        action = shootAction;
        var maxShootDistance = shootAction.MaxShootDistance;
        Debug.Log($"ShootClosestEnemyBehavior: \nd:{distance}, maxShoot:{maxShootDistance}");
        percentage = (float)distance / maxShootDistance;
        if (shootAction.GetTargetCountAtPosition(unit.GetGridPosition()) > 0) 
            return (int)(percentage * 50);
        action = unit.GetAction<SpinAction>();


        return 0;
    }
}
