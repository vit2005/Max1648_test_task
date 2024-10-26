using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootClosestEnemy : BaseBehavior
{
    public ShootClosestEnemy(Unit unit) : base(unit) { }

    public override int GetValue(out BaseAction action)
    {
        // check Closeness
        throw new System.NotImplementedException();
    }
}
