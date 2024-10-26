using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthHeuristic : BaseHeuristic
{
    public HealthHeuristic(Unit unit) : base(unit) { }

    public override int GetValue()
    {
        // якщо здоров'€ низьке, евристика даЇ менше значенн€
        return (int)unit.GetHealthNormalized();
    }
}
