using Game.Grid;
using Game.Units;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Actions
{
    public class ShootAction : BaseAction
    {
        [SerializeField] private int maxShootDistance = 6;
        [SerializeField] private float aimingStateTime = .9f;
        [SerializeField] private float shootingStateTime = .1f;
        [SerializeField] private float cooloffStateTime = .5f;
        [SerializeField] private float rotateAimingSpeed = 10f;

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

        public override bool UpdateAction(BaseActionParameters args)
        {
            stateTimer -= Time.deltaTime;

            switch (state)
            {
                case State.Aiming:
                    Vector3 moveDirection = (targetPosition - transform.position).normalized;
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
            //targetUnit?.Damage();
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

        public override bool StartAction(BaseActionParameters args)
        {
            if (!LevelGrid.Instance.TryGetUnitAtGridPosition(args.targetGridPosition, out targetUnit)) return false;
            stateTimer = aimingStateTime;
            state = State.Aiming;
            canShootBullet = true;
            return base.StartAction(args);
        }

        public override IEnumerable<GridPosition> GetValidActionGridPositions()
        {
            List<GridPosition> validGridPositions = new List<GridPosition>();
            GridPosition unitGridPosition = unit.GetGridPosition();

            if(LevelGrid.Instance.IsUnitInsideTheGrid(unit))
            {
                for (int x = -maxShootDistance; x <= maxShootDistance; x++)
                {
                    for (int z = -maxShootDistance; z <= maxShootDistance; z++)
                    {
                        GridPosition offsetGridPosition = new GridPosition(x, z);
                        GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                        if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) continue;
                        if (!LevelGrid.Instance.TryGetUnitAtGridPosition(testGridPosition, out Unit unit) || !unit.IsEnemy() || unit.IsDead()) continue;
                        float distance = GridPosition.Distance(unit.GetGridPosition(), testGridPosition);
                        if (Mathf.RoundToInt(distance) > maxShootDistance) continue;

                        validGridPositions.Add(testGridPosition);
                    }
                }
            }

            return validGridPositions;
        }
    }
}
