using Game.Core;
using Game.Grid;
using Game.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Game.Actions
{
    public class MoveAction : BaseAction
    {
        [SerializeField] private float rotateSpeed = 10f;
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float stoppingDistance = .1f;


        public override bool UpdateAction()
        {
            Vector3 targetPosition = CurrentTargetVectorPosition();
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, rotateSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
            {
                transform.position += moveDirection * moveSpeed * Time.deltaTime;
            }
            else
            {
                return true;
            }

            return false;
        }

        public override bool TryStartAction(List<GridPosition> targetGridPositions)
        {
            targetGridPositions = targetGridPositions.SelectMany(p => Pathfinding.Instance.FindPath(unit.GetGridPosition(), p, out int pathLength)).ToList();
            return base.TryStartAction(targetGridPositions);
        }

        public override bool IsValidGridPosition(GridPosition targetPosition, out float cost)
        {
            if (!base.IsValidGridPosition(targetPosition, out cost)) return false;
            if (!LevelGrid.Instance.IsUnitInsideTheGrid(unit)) return false;
            if (LevelGrid.Instance.HasAnyUnitOnGridPosition(targetPosition)) return false;
            if (!Pathfinding.Instance.IsWalkableGridPosition(targetPosition)) return false;
            if (!Pathfinding.Instance.HasPath(unit.GetGridPosition(), targetPosition, out int pathLength)) return false;

            cost = (float)pathLength / (float)Pathfinding.Instance.GetMoveCost();
            return true;
        }

        public override string GetActionName()
        {
            return "MOVE";
        }

        public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
        {
            ShootAction shootAction = unit.GetAction<ShootAction>();
            return new EnemyAIAction()
            {
                action = this,
                gridPosition = gridPosition,
                actionValue = shootAction.GetTargetGridPositonCount() * 10
            };
        }
    }
}
