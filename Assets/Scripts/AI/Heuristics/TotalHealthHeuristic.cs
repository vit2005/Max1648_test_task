using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotalHealthHeuristic : BaseHeuristic
{
    public TotalHealthHeuristic(Unit unit) : base(unit) { }

    public override int GetValue()
    {
        float total = 0f;
        foreach (var item in unit.GetFriendsList())
        {
            total += item.GetHealthNormalized() * 100f;
        }
        return (int)total;
    }
}
