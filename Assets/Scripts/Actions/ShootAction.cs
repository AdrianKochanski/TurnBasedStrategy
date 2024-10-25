using Game.Core;
using Game.Grid;
using Game.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Actions
{
    public class ShootAction : BaseAction
    {
        [SerializeField] private float aimingStateTime = .9f;
        [SerializeField] private float shootingStateTime = .1f;
        [SerializeField] private float cooloffStateTime = .5f;
        [SerializeField] private float rotateAimingSpeed = 10f;
        [SerializeField] private LayerMask obstaclesLayerMask;

        public event Action<Unit, Unit> OnShoot;

        private State state;
        private float stateTimer;
        private Unit targetUnit;
        private bool canShootBullet;

        private enum State
        {
            Aiming,
            Shooting,
            Cooloff
        }

        public override string GetActionName()
        {
            return "SHOOT";
        }

        public override bool UpdateAction()
        {
            stateTimer -= Time.deltaTime;

            switch (state)
            {
                case State.Aiming:
                    Vector3 moveDirection = (CurrentTargetVectorPosition() - transform.position).normalized;
                    transform.forward = Vector3.Lerp(transform.forward, moveDirection, rotateAimingSpeed * Time.deltaTime);
                    break;
                case State.Shooting:
                    if (canShootBullet)
                    {
                        Shoot();
                        canShootBullet = false;
                    }
                    break;
                case State.Cooloff:
                    return true;
            }

            if (stateTimer <= 0f)
            {
                return NextState();
            }

            return false;
        }

        private void Shoot()
        {
            OnShoot?.Invoke(unit, targetUnit);
        }

        private bool NextState()
        {
            switch (state)
            {
                case State.Aiming:
                    state = State.Shooting;
                    stateTimer = shootingStateTime;
                    break;
                case State.Shooting:
                    state = State.Cooloff;
                    stateTimer = cooloffStateTime;
                    break;
                case State.Cooloff:
                    return true;
            }

            return false;
        }

        public override bool TryStartAction(List<GridPosition> targetGridPositions)
        {
            if (!LevelGrid.Instance.TryGetUnitAtGridPosition(targetGridPositions.First(), out targetUnit)) return false;
            stateTimer = aimingStateTime;
            state = State.Aiming;
            canShootBullet = true;
            return base.TryStartAction(targetGridPositions);
        }

        public override (bool, bool) IsValidGridPosition(GridPosition targetPosition, out float cost)
        {
            (bool validRange, bool validTarget) = base.IsValidGridPosition(targetPosition, out cost);
            if (!validRange || !validTarget) return (false, false);
            if (!LevelGrid.Instance.IsUnitInsideTheGrid(unit)) return (false, false);
            if (LevelGrid.Instance.RaycastHorizontal(unit.GetGridPosition(), targetPosition, obstaclesLayerMask)) return (false, false);
            if (!LevelGrid.Instance.TryGetUnitAtGridPosition(targetPosition, out Unit testUnit)
                || (unit.IsEnemy() == testUnit.IsEnemy())
                || testUnit.IsDead()
            ) return (true, false);

            return (true, true);
        }

        public Unit GetTargetUnit()
        {
            return targetUnit;
        }

        public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
        {
            int actionPoints = 100;

            if(LevelGrid.Instance.TryGetUnitAtGridPosition(gridPosition, out Unit unit))
            {
                actionPoints += Convert.ToInt32((1 - unit.GetHealthNormalized()) * 100f);
            }

            return new EnemyAIAction()
            {
                action = this,
                gridPosition = gridPosition,
                actionValue = actionPoints
            };
        }
    }
}
