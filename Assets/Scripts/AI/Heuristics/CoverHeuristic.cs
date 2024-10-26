using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// returns 1 if unit is covered from nearest enemy
public class CoverHeuristic : BaseHeuristic
{
    public CoverHeuristic(Unit unit) : base(unit) { }

    public override int GetValue()
    {
        Vector3 unitWorldPosition = unit.GetWorldPosition();
        Vector3 shootDir = unit.GetClosestEnemy().GetWorldPosition() - unitWorldPosition;

        float unitShoulderHeight = 1.7f;
        if (Physics.Raycast(
            unitWorldPosition + Vector3.up * unitShoulderHeight,
            shootDir.normalized,
            shootDir.magnitude,
            unit.GetAction<ShootAction>().ObstaclesLayerMask))
        {
            return 1;
        }

        return 0;
    }
}
