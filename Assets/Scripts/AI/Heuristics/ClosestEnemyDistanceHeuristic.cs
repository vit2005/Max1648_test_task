using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClosestEnemyDistanceHeuristic : BaseHeuristic
{
    public ClosestEnemyDistanceHeuristic(Unit unit) : base(unit) { }

    public override int GetValue()
    {
        return (int)unit.GetClosestEnemy().GetGridPosition().DistanceTo(unit.GetGridPosition());  // Чим ближче ворог, тим гірше, тому від'ємне значення
    }
}
