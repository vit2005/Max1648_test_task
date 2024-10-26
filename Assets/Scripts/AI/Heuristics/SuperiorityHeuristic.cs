using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperiorityHeuristic : BaseHeuristic
{
    public SuperiorityHeuristic(Unit unit) : base(unit) { }

    // returns int that represents enemy superiority: negative number means enemies more than friendly
    public override int GetValue()
    {
        int friendlyUnits = UnitManager.Instance.GetFriendlyUnitList().Count;
        int enemyUnits = UnitManager.Instance.GetEnemyUnitList().Count;

        int superiority = friendlyUnits - enemyUnits;

        if (unit.IsEnemy) superiority *= -1;

        return superiority;
    }
}
