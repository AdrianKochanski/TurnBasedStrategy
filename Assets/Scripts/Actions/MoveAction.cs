using Game.Grid;
using Game.Interactions;
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
        [SerializeField] private float rotationTolerance = 1f;

        // Move to Lift?
        public event Action onStopWalking;
        public event Action onStartWalking;

        private State state = State.Rotating;
        private InteractAction interactAction;

        private enum State
        {
            Rotating,
            Walking,
            CallTheLift,
            WaitForLift,
            GetIntoTheLift,
            InLift
        }

        protected override void Awake()
        {
            base.Awake();
            interactAction = gameObject.GetComponent<InteractAction>();
        }

        public override bool UpdateAction()
        {
            if (TryGetCurrentTargetWorldPosition(out Vector3 targetPosition))
            {
                GridPosition currentTargetPosition = CurrentTargetPosition();
                GridPosition currentUnitPosition = unit.GetGridPosition();
                bool foundLift = GetLiftAtGridPositions(currentUnitPosition, currentTargetPosition, out Lift lift, out GridPosition liftGrid);
                bool needsLift = currentUnitPosition.floor != currentTargetPosition.floor;
                Vector3 moveDirection = (targetPosition - transform.position).normalized;
                moveDirection.y = 0f;
                transform.forward = Vector3.Lerp(transform.forward, moveDirection, rotateSpeed * Time.deltaTime);

                Debug.Log(state);
                switch (state)
                {
                    case State.Rotating:
                        float angleDifference = Vector3.Angle(transform.forward, moveDirection);
                        if (angleDifference <= rotationTolerance)
                        {
                            state = State.Walking;
                        }
                        break;
                    case State.Walking:
                        if(needsLift)
                        {
                            state = State.CallTheLift;
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
                    case State.CallTheLift:
                        if(NeedsToCallTheLift(currentUnitPosition, currentTargetPosition, lift))
                        {
                            if(!UnitActionSystem.Instance.HandleChainedAction(interactAction, new List<GridPosition>() { liftGrid }))
                            {
                                state = State.Rotating;
                                return true;
                            }
                        }
                        onStopWalking?.Invoke();
                        state = State.WaitForLift;
                        break;
                    case State.WaitForLift:
                        if (CanGetIntoTheLift(currentUnitPosition, currentTargetPosition, lift))
                        {
                            state = State.GetIntoTheLift;
                            onStartWalking?.Invoke();
                        }
                        break;
                    case State.GetIntoTheLift:
                        if (Vector3.Distance(transform.position, lift.GetLiftPlatformPosition()) > stoppingDistance)
                        {
                            moveDirection = (lift.GetLiftPlatformPosition() - transform.position).normalized;
                            transform.position += moveDirection * moveSpeed * Time.deltaTime;
                        }
                        else
                        {
                            if (!UnitActionSystem.Instance.HandleChainedAction(interactAction, new List<GridPosition>() { currentUnitPosition }))
                            {
                                state = State.Rotating;
                                return true;
                            }
                            onStopWalking?.Invoke();
                            state = State.InLift;
                        }
                        break;
                    case State.InLift:
                        if (Math.Abs((targetPosition.y - lift.GetLiftPlatformPosition().y)) <= stoppingDistance)
                        {
                            onStartWalking?.Invoke();
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

        private bool GetLiftAtGridPositions(GridPosition unitPosition, GridPosition targetPosition, out Lift lift, out GridPosition liftGrid)
        {
            if(LevelGrid.Instance.TryGetInteractableAtGrid(unitPosition, out IInteractable interactable) && interactable is Lift)
            {
                liftGrid = unitPosition;
                lift = interactable as Lift;
                return true;
            }
            else if(LevelGrid.Instance.TryGetInteractableAtGrid(targetPosition, out interactable) && interactable is Lift)
            {
                liftGrid = targetPosition;
                lift = interactable as Lift;
                return true;
            }

            lift = null;
            liftGrid = new GridPosition();
            return false;
        }

        private bool CanGetIntoTheLift(GridPosition unitPosition, GridPosition targetPosition, Lift lift)
        {
            var liftState = lift.GetState();
            if (unitPosition.floor > targetPosition.floor)
            {
                return liftState == Lift.State.PositionUp;
            }
            else if (unitPosition.floor < targetPosition.floor)
            {
                return liftState == Lift.State.PositionDown;
            }
            return false;
        }

        private bool NeedsToCallTheLift(GridPosition unitPosition, GridPosition targetPosition, Lift lift)
        {
            var liftState = lift.GetState();
            if(unitPosition.floor > targetPosition.floor)
            {
                return liftState == Lift.State.PositionDown || liftState == Lift.State.MoveDown;
            }
            else if(unitPosition.floor < targetPosition.floor)
            {
                return liftState == Lift.State.PositionUp || liftState == Lift.State.MoveUp;
            }
            return false;
        }
    }
}
