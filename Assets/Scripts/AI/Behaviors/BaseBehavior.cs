using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseBehavior
{
    protected Unit unit;
    
    public BaseBehavior(Unit unit)
    {
        this.unit = unit;
    }

    // �����, ���� ���� ��������� �������� ������.
    public abstract int GetValue(out BaseAction action);
}
