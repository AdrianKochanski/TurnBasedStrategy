using Game.Grid;
using Game.Units;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Actions
{
    public class SpinAction : BaseAction
    {
        [SerializeField] private float spinSpeed = 360f;
        [SerializeField] private float eulerAnglesDestination = 360f;

        private float totalSpinAmount = 0;

        public override bool TryStartAction(List<GridPosition> targetGridPositions)
        {
            totalSpinAmount = 0;
            return base.TryStartAction(targetGridPositions);
        }

        public override bool UpdateAction()
        {
            float spinToAdd = spinSpeed * Time.deltaTime;
            transform.eulerAngles += new Vector3(0, spinToAdd, 0);
            totalSpinAmount += spinToAdd;

            if (totalSpinAmount >= eulerAnglesDestination)
            {
                return true;
            }

            return false;
        }

        public override bool IsValidGridPosition(GridPosition targetPosition, out float cost)
        {
            return base.IsValidGridPosition(targetPosition, out cost);
        }

        public override string GetActionName()
        {
            return "SPIN";
        }

        public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
        {
            return new EnemyAIAction()
            {
                action = this,
                gridPosition = gridPosition,
                actionValue = 0
            };
        }
    }
}