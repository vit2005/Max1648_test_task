using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntBehavior : BaseBehavior
{
    public HuntBehavior(Unit unit) : base(unit) { }

    public override int GetValue(out BaseAction action)
    {
        action = unit.GetAction<SpinAction>();
        // Check Superiority to MoveAction (InteractAction)
        var superiority = unit.GetHeuristic<SuperiorityHeuristic>().GetValue();
        Debug.Log($"HuntBehavior: \ns:{superiority}");
        if (superiority > 4)
        {
            action = unit.GetAction<MoveOffenciveAction>();
            return 50;
        }

        return 0;
    }
}
