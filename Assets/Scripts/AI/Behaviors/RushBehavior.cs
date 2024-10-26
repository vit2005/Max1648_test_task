using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RushBehavior : BaseBehavior
{
    public RushBehavior(Unit unit) : base(unit) { }

    public override int GetValue(out BaseAction action)
    {
        // TODO: check Closeness and then Cluster heuristic to MoveAction, Grenade or SwordAction
        throw new System.NotImplementedException();
    }
}
