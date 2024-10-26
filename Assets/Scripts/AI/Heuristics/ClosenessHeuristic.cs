using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClosenessHeuristic : BaseHeuristic
{
    public ClosenessHeuristic(Unit unit) : base(unit) { }

    public override int GetValue()
    {
        return (int)unit.GetClosestEnemy().GetGridPosition().DistanceTo(unit.GetGridPosition());  // ��� ������ �����, ��� ����, ���� ��'���� ��������
    }
}