using Game.Grid;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static Game.Actions.MoveAction;

namespace Game.Actions
{
    public class MoveAction : BaseAction
    {
        [SerializeField] private Animator unitAnimator;
        [SerializeField] private float rotateSpeed = 10f;
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float stoppingDistance = .1f;
        [SerializeField] private int maxMoveDistance = 2;

        private Vector3 targetPosition;

        protected override void Awake()
        {
            base.Awake();
            targetPosition = transform.position;
        }

        public override bool UpdateAction()
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;

            if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
            {
                transform.position += moveDirection * moveSpeed * Time.deltaTime;
                unitAnimator.SetBool("IsWalking", true);
            }
            else
            {
                unitAnimator.SetBool("IsWalking", false);
                return true;
            }

            transform.forward = Vector3.Lerp(transform.forward, moveDirection, rotateSpeed * Time.deltaTime);
            return false;
        }

        public override void StartAction(BaseActionParameters baseParams)
        {
            base.StartAction(baseParams);
            targetPosition = LevelGrid.Instance.GetWorldPositon(baseParams.gridPosition);
        }

        public override bool IsValidActionGridPositon(GridPosition gridPosition)
        {
            if (LevelGrid.Instance.IsUnitInsideTheGrid(unit))
            {
                return base.IsValidActionGridPositon(gridPosition);
            }
            else
            {
                return !LevelGrid.Instance.IsValidGridPosition(gridPosition) || LevelGrid.Instance.IsGridBorder(gridPosition);
            }
        }

        public override IEnumerable<GridPosition> GetValidActionGridPositions()
        {
            List<GridPosition> validGridPositions = new List<GridPosition>();
            GridPosition unitGridPosition = unit.GetGridPosition();

            for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
            {
                for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
                {
                    GridPosition offsetGridPosition = new GridPosition(x, z);
                    GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                    if (unitGridPosition == testGridPosition) continue;
                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) continue;
                    if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) continue;
                    if (!LevelGrid.Instance.IsUnitInsideTheGrid(unit) && !LevelGrid.Instance.IsGridBorder(testGridPosition)) continue;

                    validGridPositions.Add(testGridPosition);
                }
            }

            return validGridPositions;
        }

        public override string GetActionName()
        {
            return "MOVE";
        }
    }
}
