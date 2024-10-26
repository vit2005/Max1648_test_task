using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClusterHeuristic : BaseHeuristic
{
    private readonly float detectionRadius = 2;

    public ClusterHeuristic(Unit unit) : base(unit) { }

    public override int GetValue()
    {
        int enemiesWithNeighborsCount = 0;

        // �������� ������ ��� ������
        List<Unit> enemies = unit.GetEnemiesList();
        var curPos = unit.GetGridPosition();
        int moveDistance = unit.GetAction<MoveAction>().MaxMoveDistance;
        var visibleWalkableEnemies = enemies.Where(x => 
        x.GetGridPosition().DistanceTo(unit.GetGridPosition()) <= moveDistance &&
        Pathfinding.Instance.HasPath(curPos, x.GetGridPosition())).ToList();

        foreach (var visibleEnemy in visibleWalkableEnemies)
        {
            foreach (var enemy in enemies)
            {
                if (visibleEnemy.GetGridPosition().DistanceTo(enemy.GetGridPosition()) <= detectionRadius)
                {
                    enemiesWithNeighborsCount++;
                    break;
                }
            }
        }

        // ��� ����� ������ ����� ����� ���������, ��� ���� ������ (����� ������������ ��������)
        return enemiesWithNeighborsCount * 10;  // ����� ������������ ������� ��� ������������
    }
}
