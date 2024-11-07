using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveOffenciveAction : BaseAction
{
    [SerializeField] private int maxMoveDistance = 4;

    public override bool IsPlayerPlayable => true;
    private float moveSpeed = 10f;
    private float rotateSpeed = 10f;
    public int MaxMoveDistance => maxMoveDistance;

    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;

    private List<Vector3> positionList;
    private int currentPositionIndex;

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        Vector3 targetPosition = positionList[currentPositionIndex];
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);

        float stoppingDistance = .1f;
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
        else
        {
            currentPositionIndex++;
            if (currentPositionIndex >= positionList.Count)
            {
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                ActionComplete();
            }
        }
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);
        currentPositionIndex = 0;
        positionList = new List<Vector3>();

        foreach (GridPosition pathGridPosition in pathGridPositionList)
        {
            positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }

        OnStartMoving?.Invoke(this, EventArgs.Empty);
        ActionStart(onActionComplete);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {

        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                if (unitGridPosition == testGridPosition)
                {
                    continue;
                }

                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    continue;
                }

                if (!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition))
                {
                    continue;
                }

                if (!Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition))
                {
                    continue;
                }

                int pathfindingDistanceMultiplier = 10;
                if (Pathfinding.Instance.GetPathLength(unitGridPosition, testGridPosition) > maxMoveDistance * pathfindingDistanceMultiplier)
                {
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
                // Debug.Log(testGridPosition);
            }
        }

        return validGridPositionList;

    }

    protected List<GridPosition> ShortestPathToEnemy()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();

        List<List<GridPosition>> pathes = new List<List<GridPosition>>();
        foreach (var enemy in unit.GetEnemiesList())
        {
            GridPosition enemyPosition = enemy.GetGridPosition();

            List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(unitGridPosition, enemyPosition, out int pathLength);
            if (pathGridPositionList != null) pathes.Add(pathGridPositionList);
        }

        List<GridPosition> shortest = pathes.OrderBy(x => x.Count).FirstOrDefault();
        if (shortest == null) return new List<GridPosition>();

        return shortest.Take(maxMoveDistance * 10).ToList();
    }

    public override string GetActionName()
    {
        return "Move";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        int actionValue = 0;

        int targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);

        if (targetCountAtGridPosition == 0)
        {
            var bestPath = ShortestPathToEnemy();
            bestPath.Reverse();
            int index = bestPath.IndexOf(gridPosition);
            if (index != -1)
                actionValue = (int)(10 * (bestPath.Count / index));
            else
                actionValue = 0;
        }
        else
        {
            actionValue = targetCountAtGridPosition * 10;
        }

        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = actionValue,
        };
    }
}
