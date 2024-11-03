using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDefenciveAction : MoveOffenciveAction
{
    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        int targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);

        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 10 - targetCountAtGridPosition * 10,
        };
    }
}
