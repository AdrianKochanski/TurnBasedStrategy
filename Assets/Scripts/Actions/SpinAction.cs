using Game.Grid;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Actions
{
    public class SpinAction : BaseAction
    {
        [SerializeField] private float spinSpeed = 360f;
        [SerializeField] private float eulerAnglesDestination = 360f;

        private float totalSpinAmount = 0;

        public override bool StartAction(BaseActionParameters baseParams)
        {
            totalSpinAmount = 0;
            return base.StartAction(baseParams);
        }

        public override bool UpdateAction(BaseActionParameters args)
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

        public override IEnumerable<GridPosition> GetValidActionGridPositions()
        {
            List<GridPosition> validGridPositionList = new List<GridPosition>();

            if (LevelGrid.Instance.IsValidGridPosition(unit.GetGridPosition()))
            {
                validGridPositionList.Add(unit.GetGridPosition());
            }

            return validGridPositionList;
        }

        public override string GetActionName()
        {
            return "SPIN";
        }
    }
}