using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClosenessHeuristic : BaseHeuristic
{
    public ClosenessHeuristic(Unit unit) : base(unit) { }

    // distance to nearest enemy
    public override int GetValue()
    {
        return (int)unit.GetClosestEnemy().GetGridPosition().DistanceTo(unit.GetGridPosition());  // Чим ближче ворог, тим гірше, тому від'ємне значення
    }
}
