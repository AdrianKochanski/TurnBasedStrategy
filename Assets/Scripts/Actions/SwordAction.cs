using Game.Grid;
using Game.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Game.Actions
{
    public class SwordAction : BaseAction
    {
        [SerializeField] private float beforeHitTime = .7f;
        [SerializeField] private float afterHitTime = .5f;
        [SerializeField] private float rotateAimingSpeed = 10f;
        [SerializeField] private int damageAmount = 100;

        MoveAction moveAction;
        public static event Action OnAnySwordHit; 

        private State state;
        private float stateTimer;
        private enum State
        {
            BeforeHit,
            AfterHit
        }

        protected override void Awake()
        {
            base.Awake();
            moveAction = GetComponent<MoveAction>();
        }

        public override string GetActionName()
        {
            return "Sword";
        }

        public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
        {
            return new EnemyAIAction()
            {
                action = this,
                gridPosition = gridPosition,
                actionValue = 200
            };
        }

        public override (bool, bool) IsValidGridPosition(GridPosition targetPosition, out float cost)
        {
            (bool validRange, bool validTarget) = base.IsValidGridPosition(targetPosition, out cost);
            if (!validRange || !validTarget) return (false, false);

            if (!Pathfinding.Instance.HasPath(unit.GetGridPosition(), targetPosition, out int pathLength)) return (false, false);
            //cost = (float)pathLength / (float)Pathfinding.Instance.GetMoveCost();

            if (!LevelGrid.Instance.TryGetUnitAtGridPosition(targetPosition, out Unit testUnit)
                || (unit.IsEnemy() == testUnit.IsEnemy())
                || testUnit.IsDead()
            ) return (true, false);

            return (true, true);
        }

        public override bool TryStartAction(List<GridPosition> targetGridPositions)
        {
            stateTimer = beforeHitTime;
            state = State.BeforeHit;
            return base.TryStartAction(targetGridPositions);
        }

        public override bool UpdateAction()
        {
            stateTimer -= Time.deltaTime;

            switch (state)
            {
                case State.BeforeHit:
                    Vector3 moveDirection = (CurrentTargetVectorPosition() - transform.position).normalized;
                    transform.forward = Vector3.Lerp(transform.forward, moveDirection, rotateAimingSpeed * Time.deltaTime);
                    break;
                case State.AfterHit:
                    return true;
            }

            if (stateTimer <= 0f)
            {
                return NextState();
            }

            return false;
        }

        private bool NextState()
        {
            switch (state)
            {
                case State.BeforeHit:
                    state = State.AfterHit;
                    stateTimer = afterHitTime;
                    if(TryGetNextTargetUnit(out Unit currentTargetUnit))
                    {
                        currentTargetUnit.Damage(damageAmount, unit.GetWorldPositon());
                        OnAnySwordHit?.Invoke();
                    }
                    break;
                case State.AfterHit:
                    return true;
            }

            return false;
        }

        private bool TryGetNextTargetUnit(out Unit currentTargetUnit)
        {
            if(LevelGrid.Instance.TryGetUnitAtGridPosition(CurrentTargetPosition(), out currentTargetUnit) 
                && (unit.IsEnemy() != currentTargetUnit.IsEnemy())
                && !currentTargetUnit.IsDead())
            {
                return true;
            }

            return false;
        }

        private GridPosition? GetPositionWithShortestAdjacentPath(GridPosition startGridPosition, GridPosition endGridPosition)
        {
            var testPositions = GetAdjacentPositions(endGridPosition);

            GridPosition? endPosition = null;
            int pathLength = int.MaxValue;

            foreach (GridPosition testEndPosition in testPositions)
            {
                Pathfinding.Instance.FindPath(startGridPosition, testEndPosition, out int testEndLength);
                (var validRange, var validTarget) = moveAction.IsValidGridPosition(testEndPosition, out float cost);
                if (validTarget && testEndLength < pathLength)
                {
                    pathLength = testEndLength;
                    endPosition = testEndPosition;
                }
            }

            return endPosition;
        }

        private IEnumerable<GridPosition> GetAdjacentPositions(GridPosition position)
        {
            GridPosition left = position + new GridPosition(-1, 0);
            GridPosition right = position + new GridPosition(1, 0);
            GridPosition up = position + new GridPosition(0, 1);
            GridPosition down = position + new GridPosition(0, -1);
            return new List<GridPosition>() { left, right, up, down };
        }
    }
}
