using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveDefenciveAction : MoveOffenciveAction
{
    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        int actionValue = 0;

        int targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);

        var bestPath = ShortestPathToEnemy();
        GridPosition bestPathEnd = bestPath.LastOrDefault();
        actionValue = (int)bestPathEnd.DistanceTo(gridPosition);
        if (!unit.IsEnemy) actionValue = -actionValue;

        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = actionValue,
        };
    }
}
