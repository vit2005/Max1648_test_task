using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideBehavior : BaseBehavior
{
    public HideBehavior(Unit unit) : base(unit) { }

    public override int GetValue(out BaseAction action)
    {
        // TODO: check Superiority and Health heuristics, than Cover to SpinAction or MoveAction
        throw new System.NotImplementedException();
    }
}
