using Game.Actions;
using Game.Grid;
using Game.Interactions;
using Game.Units;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractAction : BaseAction
{
    [SerializeField] private float rotateAimingSpeed = 10f;
    [SerializeField] private float rotationTolerance = 1f;
    [SerializeField] private float rotateSpeed = 10f;

    private State state;
    private enum State
    {
        BeforeInteraction,
        Interaction,
        AfterInteraction
    }

    public override string GetActionName()
    {
        return "Interact";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction()
        {
            action = this,
            gridPosition = gridPosition,
            actionValue = 50
        };
    }

    public override (bool, bool) IsValidGridPosition(GridPosition targetPosition, out float cost)
    {
        (bool validRange, bool validTarget) = base.IsValidGridPosition(targetPosition, out cost);

        if (LevelGrid.Instance.TryGetInteractableAtGrid(targetPosition, out IInteractable interactable) && interactable.CanInteract(unit))
        {
            return (true, true);
        }

        return (validRange, false);
    }

    public override bool TryStartAction(List<GridPosition> targetGridPositions)
    {
        state = State.BeforeInteraction;
        if(LevelGrid.Instance.TryGetInteractableAtGrid(targetGridPositions.First(), out IInteractable interactable) && interactable.FinishedInteraction() && interactable.CanInteract(unit))
        {
            return base.TryStartAction(targetGridPositions);
        }
        return false;
    }

    protected override UpdateActionResult UpdateAction()
    {
        if (TryGetNextInteractable(out IInteractable interactable) && TryGetCurrentTargetWorldPosition(out Vector3 targetPosition))
        {
            switch (state)
            {
                case State.BeforeInteraction:
                    Vector3 moveDirection = (targetPosition - transform.position).normalized;
                    float angleDifference = Vector3.Angle(transform.forward, moveDirection);
                    transform.forward = Vector3.Slerp(transform.forward, moveDirection, rotateSpeed * Time.deltaTime);
                    if (GridPosition.IsParallel(CurrentTargetPosition(), unit.GetGridPosition()) || angleDifference <= rotationTolerance)
                    {
                        state = State.Interaction;
                    }
                    break;
                case State.Interaction:
                    interactable.Interact();
                    state = State.AfterInteraction;
                    break;
                case State.AfterInteraction:
                    if(interactable.FinishedInteraction())
                    {
                        return UpdateActionResult.NextStep;
                    }
                    break;
            }
        }

        return UpdateActionResult.Continue;
    }

    private bool TryGetNextInteractable(out IInteractable interactable)
    {
        return LevelGrid.Instance.TryGetInteractableAtGrid(CurrentTargetPosition(), out interactable);
    }
}
