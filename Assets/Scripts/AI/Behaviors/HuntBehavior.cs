using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntBehavior : BaseBehavior
{
    public HuntBehavior(Unit unit) : base(unit) { }

    public override int GetValue(out BaseAction action)
    {
        // Check Superiority to MoveAction (InteractAction) or StayAction
        throw new System.NotImplementedException();
    }
}
