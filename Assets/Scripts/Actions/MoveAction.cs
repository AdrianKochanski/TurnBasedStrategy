using Game.Grid;
using Game.Units;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Actions
{
    public class MoveAction : BaseAction
    {
        [SerializeField] private float rotateSpeed = 10f;
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float stoppingDistance = .1f;
        [SerializeField] private float rotationTolerance = 1f;

        private State state = State.Rotating;

        private enum State
        {
            Rotating,
            Walking,
            Climbing
        }

        public override bool UpdateAction()
        {
            if (TryGetCurrentTargetWorldPosition(out Vector3 targetPosition))
            {
                GridPosition currentTargetPosition = CurrentTargetPosition();
                GridPosition currentUnitPosition = unit.GetGridPosition();
                GridPosition downLiftPosition = new GridPosition(currentTargetPosition.x, currentTargetPosition.z, currentUnitPosition.floor);
                Vector3 moveDirection = (targetPosition - transform.position).normalized;
                moveDirection.y = 0f;
                transform.forward = Vector3.Lerp(transform.forward, moveDirection, rotateSpeed * Time.deltaTime);

                switch (state)
                {
                    case State.Rotating:
                        Debug.Log($"Rotating: {transform.position}");
                        float angleDifference = Vector3.Angle(transform.forward, moveDirection);
                        if (angleDifference <= rotationTolerance)
                        {
                            state = State.Walking;
                        }
                        break;
                    case State.Walking:
                        Debug.Log($"Walking: {transform.position}");
                        // Lift logic
                        if(currentTargetPosition.floor < currentUnitPosition.floor 
                            && LevelGrid.Instance.TryGetWorldPositon(downLiftPosition, out Vector3 downLiftVectorPosition))
                        {
                            if(Vector3.Distance(transform.position, downLiftVectorPosition) > stoppingDistance)
                            {
                                moveDirection = (downLiftVectorPosition - transform.position).normalized;
                                transform.position += moveDirection * moveSpeed * Time.deltaTime;
                            }
                            else
                            {
                                state = State.Climbing;
                            }
                        }
                        else if(currentTargetPosition.floor > currentUnitPosition.floor)
                        {
                            state = State.Climbing;
                        }
                        else if(Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
                        {
                            transform.position += moveDirection * moveSpeed * Time.deltaTime;
                        }
                        else
                        {
                            transform.position = targetPosition;
                            state = State.Rotating;
                            return true;
                        }
                        break;
                    case State.Climbing:
                        Debug.Log($"Climbing: {transform.position} - {this.unit.transform.position}");
                        if (LevelGrid.Instance.TryGetWorldPositon(CurrentTargetPosition(), out Vector3 targetVector3)
                            && Mathf.Abs(targetVector3.y - this.unit.transform.position.y) > stoppingDistance)
                        {
                            Vector3 climbingDirection = (targetVector3.y - this.unit.transform.position.y) * Vector3.up;
                            transform.position += climbingDirection * moveSpeed * Time.deltaTime;
                        }
                        else
                        {
                            transform.position = new Vector3(transform.position.x, targetVector3.y, transform.position.z);
                            state = State.Walking;
                        }
                        break;
                }
            }

            return false;
        }

        public override bool TryStartAction(List<GridPosition> targetGridPositions)
        {
            targetGridPositions = targetGridPositions.SelectMany(p => Pathfinding.Instance.FindPath(unit.GetGridPosition(), p, GetPossibleActionsCountLimit(), out int pathLength)).ToList();
            return base.TryStartAction(targetGridPositions);
        }

        public override (bool, bool) IsValidGridPosition(GridPosition targetPosition, out float cost)
        {
            (bool validRange, bool validTarget) = base.IsValidGridPosition(targetPosition, out cost);

            if (!validRange || !validTarget) return (false, false);
            if (!LevelGrid.Instance.IsUnitInsideTheGrid(unit)) return (false, false);
            if (LevelGrid.Instance.HasAnyUnitOnGridPosition(targetPosition)) return (false, false);
            if (!Pathfinding.Instance.HasPath(unit.GetGridPosition(), targetPosition, GetPossibleActionsCountLimit(), out int pathLength)) return (false, false);
            
            cost = (float)pathLength / (float)Pathfinding.Instance.GetMoveCost();
            return (true, true);
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
