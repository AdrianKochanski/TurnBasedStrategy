using Game.Grid;
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

        public override string GetActionName()
        {
            return "SPIN";
        }
    }
}