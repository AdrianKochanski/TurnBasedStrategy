using Game.Grid;
using Game.Interactions;
using Game.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Lift : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform liftPanel;
    [SerializeField] private Transform upPosition;
    [SerializeField] private Transform downPosition;
    [SerializeField] private State state;
    [SerializeField] private float afterInteractiontTime = .5f;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float stoppingDistance = .1f;

    private GridPosition upGridPosition;
    private GridPosition downGridPosition;
    private List<GridPosition> upGridLinkedPositions = new List<GridPosition>();

    [Serializable]
    public enum State
    {
        PositionUp,
        PositionDown,
        MoveUp,
        MoveDown
    }

    private void Start()
    {
        SetInteractableAtGrid();
    }

    private void Update()
    {
        if(LevelGrid.Instance.TryGetWorldPositon(upGridPosition, out Vector3 upTargetVector3)
            && LevelGrid.Instance.TryGetWorldPositon(downGridPosition, out Vector3 downTargetVector3))
        {
            if (state == State.MoveUp)
            {
                UpdateLift(upTargetVector3);
            }
            else if (state == State.MoveDown)
            {
                UpdateLift(downTargetVector3);
            }
        }
    }

    private void UpdateLift(Vector3 targetVector)
    {
        if (Mathf.Abs(targetVector.y - liftPanel.transform.position.y) > stoppingDistance)
        {
            Vector3 climbingDirection = (targetVector.y - liftPanel.transform.position.y) * Vector3.up;
            liftPanel.transform.position += climbingDirection * moveSpeed * Time.deltaTime;
        }
        else
        {
            liftPanel.transform.position = new Vector3(liftPanel.transform.position.x, targetVector.y, liftPanel.transform.position.z);
            NextState();
        }

        if (TryGetUnitInLift(out Unit unitInLift)) unitInLift.transform.position = liftPanel.transform.position;
    }

    private bool TryGetUnitInLift(out Unit unit)
    {
        return LevelGrid.Instance.TryGetUnitAtGridPosition(downGridPosition, out unit)
            || LevelGrid.Instance.TryGetUnitAtGridPosition(upGridPosition, out unit);
    }

    public bool CanInteract()
    {
        if(state == State.MoveUp || state == State.MoveDown) return false;
        else if (state == State.PositionUp && LevelGrid.Instance.TryGetUnitAtGridPosition(downGridPosition, out Unit unitUnderLift)) return false;
        return true;
    }

    public float Interact()
    {
        NextState();
        return afterInteractiontTime;
    }

    private void NextState()
    {
        //animator.SetBool("IsOpen", false);
        switch(state)
        {
            case State.PositionDown:
                state = State.MoveUp;
                break;
            case State.MoveUp:
                Pathfinding.Instance.SetIsWalkableGridPosition(upGridPosition, true);
                state = State.PositionUp;
                break;
            case State.PositionUp:
                Pathfinding.Instance.SetIsWalkableGridPosition(upGridPosition, false);
                state = State.MoveDown;
                break;
            case State.MoveDown:
                state = State.PositionDown;
                break;
        }
    }

    public void SetInteractableAtGrid()
    {
        if (LevelGrid.Instance.TryGetGridPosition(downPosition.position, out downGridPosition))
        {
            LevelGrid.Instance.SetInteractableAtGrid(downGridPosition, this);
        }

        if (LevelGrid.Instance.TryGetGridPosition(upPosition.position, out upGridPosition))
        {
            LevelGrid.Instance.SetInteractableAtGrid(upGridPosition, this);
            upGridLinkedPositions.Add(upGridPosition);
            foreach(var neighbour in Pathfinding.Instance.GetNeighbourList(upGridPosition))
            {
                LevelGrid.Instance.SetInteractableAtGrid(neighbour.GetGridPosition(), this);
                upGridLinkedPositions.Add(neighbour.GetGridPosition());
            }
        }
    }

    internal bool TryGetConnectedLinks(GridPosition gridPosition, out IEnumerable<GridPosition> connectedGrids)
    {
        if (upGridLinkedPositions.Any(p => p == gridPosition))
        {
            connectedGrids = new List<GridPosition>() { downGridPosition };
            return true;
        }
        else if (gridPosition == downGridPosition)
        {
            connectedGrids = upGridLinkedPositions;
            return true;
        }

        connectedGrids = new List<GridPosition>();
        return false;
    }

    public Vector3 GetLiftPlatformPosition()
    {
        return liftPanel.position;
    }

    public State GetState()
    {
        return state;
    }
}
