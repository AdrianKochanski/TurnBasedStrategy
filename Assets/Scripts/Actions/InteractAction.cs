using Game.Actions;
using Game.Grid;
using Game.Interactions;
using Game.Units;
using System.Collections.Generic;
using UnityEngine;

public class InteractAction : BaseAction
{
    [SerializeField] private float beforeInteractiontTime = .7f;
    [SerializeField] private float rotateAimingSpeed = 10f;

    private State state;
    private float stateTimer;
    private bool interactionFinished = false;
    private enum State
    {
        BeforeInteraction,
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
        if (!validRange || !validTarget) return (false, false);

        if (!LevelGrid.Instance.TryGetInteractableAtGrid(targetPosition, out IInteractable door) || !door.CanInteract())
        {
            return (true, false);
        }

        return (true, true);
    }

    public override bool TryStartAction(List<GridPosition> targetGridPositions)
    {
        stateTimer = beforeInteractiontTime;
        state = State.BeforeInteraction;
        interactionFinished = false;
        return base.TryStartAction(targetGridPositions);
    }

    public override bool UpdateAction()
    {
        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.BeforeInteraction:
                Vector3 moveDirection = (CurrentTargetVectorPosition() - transform.position).normalized;
                transform.forward = Vector3.Lerp(transform.forward, moveDirection, rotateAimingSpeed * Time.deltaTime);
                break;
            case State.AfterInteraction:
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
            case State.BeforeInteraction:
                state = State.AfterInteraction;
                if (TryGetNextInteractable(out IInteractable interactable))
                {
                    stateTimer = interactable.Interact();
                }
                break;
            case State.AfterInteraction:
                return true;
        }

        return false;
    }

    private bool TryGetNextInteractable(out IInteractable interactable)
    {
        return LevelGrid.Instance.TryGetInteractableAtGrid(CurrentTargetPosition(), out interactable);
    }
}
