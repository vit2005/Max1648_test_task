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

    // �����, ���� ���� ��������� �������� ������.
    public abstract int GetValue();
}
