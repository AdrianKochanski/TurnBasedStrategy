using Game.Actions;
using Game.Grid;
using Game.Units;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAction : BaseAction
{
    [SerializeField] private float rotateAimingSpeed = 10f;
    [SerializeField] private float rotationTolerance = 1f;

    public event Action<Unit, GridPosition> OnThrow;
    public static event Action<Unit, GridPosition> OnAnyThrow;

    private State state = State.TargetReached;

    private enum State
    {
        Rotating,
        Throwing,
        Waiting,
        TargetReached
    }

    public override string GetActionName()
    {
        return "Grenade";
    }

    protected override UpdateActionResult UpdateAction()
    {
        if(TryGetCurrentTargetWorldPosition(out Vector3 targetPosition))
        {
            Debug.Log(state);
            switch (state)
            {
                case State.Rotating:

                    Vector3 moveDirection = (targetPosition - transform.position).normalized;
                    transform.forward = Vector3.Slerp(transform.forward, moveDirection, rotateAimingSpeed * Time.deltaTime);

                    float angleDifference = Vector3.Angle(transform.forward, moveDirection);
                    if (angleDifference <= rotationTolerance)
                    {
                        NextState();
                    }
                    break;
                case State.Throwing:
                    Shoot();
                    NextState();
                    break;
                case State.TargetReached:
                    return UpdateActionResult.NextStep;
            }
        }

        return UpdateActionResult.Continue;
    }

    private bool NextState()
    {
        switch (state)
        {
            case State.Rotating:
                state = State.Throwing;
                break;
            case State.Throwing:
                state = State.Waiting;
                break;
            case State.TargetReached:
                return true;
        }

        return false;
    }

    public override bool TryStartAction(List<GridPosition> targetGridPositions)
    {
        if (!(state == State.TargetReached)) return false;
        state = State.Rotating;
        return base.TryStartAction(targetGridPositions);
    }

    public override (bool, bool) IsValidGridPosition(GridPosition targetPosition, out float cost)
    {
        (bool validRange, bool validTarget) = base.IsValidGridPosition(targetPosition, out cost);
        if (!validRange || !validTarget) return (false, false);
        if (!LevelGrid.Instance.IsUnitInsideTheGrid(unit)) return (false, false);
        if (LevelGrid.Instance.RaycastHorizontal(unit.GetGridPosition(), targetPosition, obstaclesLayerMask)) return (false, false);

        return (true, true);
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction()
        {
            action = this,
            actionValue = 0,
            gridPosition = gridPosition
        };
    }

    public void TargetReached()
    {
        Debug.Log("TargetReached");
        state = State.TargetReached;
    }

    private void Shoot()
    {
        var currentTargetPos = CurrentTargetPosition();
        OnThrow?.Invoke(unit, currentTargetPos);
        OnAnyThrow?.Invoke(unit, currentTargetPos);
    }   
}
