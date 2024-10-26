using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseHeuristic
{
    protected Unit unit;

    public BaseHeuristic(Unit unit)
    {
        this.unit = unit;
    }

    // Метод, який буде повертати значення оцінки.
    public abstract int GetValue();
}
