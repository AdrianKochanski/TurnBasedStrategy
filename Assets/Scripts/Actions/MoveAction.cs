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

        public override bool StartAction(BaseActionParameters baseParams)
        {
            targetPosition = LevelGrid.Instance.GetWorldPositon(baseParams.gridPosition);
            return base.StartAction(baseParams);
        }

        public override bool IsValidActionGridPositon(BaseActionParameters args)
        {
            if (LevelGrid.Instance.IsUnitInsideTheGrid(unit))
            {
                return base.IsValidActionGridPositon(args);
            }
            else
            {
                return !LevelGrid.Instance.IsValidGridPosition(args.gridPosition) || LevelGrid.Instance.IsGridBorder(args.gridPosition);
            }
        }

        public override IEnumerable<GridPosition> GetValidActionGridPositions()
        {
            List<GridPosition> validGridPositions = new List<GridPosition>();
            GridPosition unitGridPosition = unit.GetGridPosition();
            int availablePoints = GetPossibleActionsCount();

            for (int x = -availablePoints; x <= availablePoints; x++)
            {
                for (int z = -availablePoints; z <= availablePoints; z++)
                {
                    GridPosition offsetGridPosition = new GridPosition(x, z);
                    GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                    if (unitGridPosition == testGridPosition) continue;
                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) continue;
                    if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) continue;
                    if (!LevelGrid.Instance.IsUnitInsideTheGrid(unit) && !LevelGrid.Instance.IsGridBorder(testGridPosition)) continue;
                    if (!CanSpendActionPoints(new BaseActionParameters() { gridPosition = testGridPosition })) continue;

                    validGridPositions.Add(testGridPosition);
                }
            }

            return validGridPositions;
        }

        public override float GetActionPointCost(BaseActionParameters args)
        {
            return base.GetActionPointCost(args) * GridPosition.Distance(unit.GetGridPosition(), args.gridPosition);
        }

        public override string GetActionName()
        {
            return "MOVE";
        }
    }
}
