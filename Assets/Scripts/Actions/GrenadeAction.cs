using Game.Actions;
using Game.Grid;
using Game.Units;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAction : BaseAction
{
    public event Action<Unit, GridPosition> OnThrow;
    public static event Action<Unit, GridPosition> OnAnyThrow;

    private bool wasThrown = false;
    private bool targetReached = true;
    public override string GetActionName()
    {
        return "Grenade";
    }

    public override bool UpdateAction()
    {
        if(!wasThrown)
        {
            Shoot();
        }

        return targetReached;
    }

    public override bool TryStartAction(List<GridPosition> targetGridPositions)
    {
        if (!targetReached) return false;
        targetReached = false;
        wasThrown = false;
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
        targetReached = true;
    }

    private void Shoot()
    {
        wasThrown = true;
        OnThrow?.Invoke(unit, CurrentTargetPosition());
        OnAnyThrow?.Invoke(unit, CurrentTargetPosition());
    }   
}
