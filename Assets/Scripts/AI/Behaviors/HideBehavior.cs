using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideBehavior : BaseBehavior
{
    public HideBehavior(Unit unit) : base(unit) { }

    public override int GetValue(out BaseAction action)
    {
        // TODO: check Superiority and Health heuristics, than Cover to SpinAction or MoveAction
        action = unit.GetAction<MoveDefenciveAction>();
        var superiority = unit.GetHeuristic<SuperiorityHeuristic>().GetValue();
        var health = unit.GetHeuristic<HealthHeuristic>().GetValue();

        Debug.Log($"HideBehavior: \ns:{superiority}, h:{health}");

        if (superiority < -3 || health <= 31)
        {
            var cover = unit.GetHeuristic<CoverHeuristic>().GetValue();
            if (cover == 0) return 70;

            action = unit.GetAction<SpinAction>();
            return 10;
        }

        return 10;
    }
}
