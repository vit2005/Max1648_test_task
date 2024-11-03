using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideBehavior : BaseBehavior
{
    public HideBehavior(Unit unit) : base(unit) { }

    public override int GetValue(out BaseAction action)
    {
        // TODO: check Superiority and Health heuristics, than Cover to SpinAction or MoveAction
        action = unit.GetAction<SpinAction>();
        var superiority = unit.GetHeuristic<SuperiorityHeuristic>().GetValue();
        var health = unit.GetHeuristic<HealthHeuristic>().GetValue();

        Debug.Log($"HideBehavior: \ns:{superiority}, h:{health}");

        if (superiority < -3 || health <= 0.31f)
        {
            var cover = unit.GetHeuristic<CoverHeuristic>().GetValue();
            if (cover == 1) return 0;

            action = unit.GetAction<MoveOffenciveAction>();
            return 70;
        }

        return 0;
    }
}
