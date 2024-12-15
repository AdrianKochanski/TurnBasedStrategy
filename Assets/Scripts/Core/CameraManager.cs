using Game.Actions;
using Game.Units;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private GameObject actionCameraGameObject;
        [SerializeField] private Vector3 shootOffsetAmount = new Vector3(0.5f, 1.7f, 1);

        private void Start()
        {
            BaseAction.OnAnyActionBegin += BaseAction_OnAnyActionBegin;
            BaseAction.OnAnyActionComplete += BaseAction_OnAnyActionComplete;
            HideActionCamera();
        }

        private void ShowActionCamera()
        {
            actionCameraGameObject.SetActive(true);
        }

        private void HideActionCamera()
        {
            actionCameraGameObject.SetActive(false);
        }

        private void BaseAction_OnAnyActionBegin(BaseAction action)
        {
            switch(action)
            {
                case ShootAction shootAction:
                    Unit shooterUnit = shootAction.GetUnit();
                    Unit targetUnit = shootAction.GetTargetUnit();
                    if(targetUnit.TryGetWorldPositon(out Vector3 worldTargetPosition) && shooterUnit.TryGetWorldPositon(out Vector3 worldShooterPosition))
                    {
                        Vector3 shootDir = (worldTargetPosition - worldShooterPosition).normalized;

                        Vector3 shoulderRightOffset = Quaternion.Euler(0, 90, 0) * shootDir * shootOffsetAmount.x;
                        Vector3 shoulderUpOffset = Vector3.up * shootOffsetAmount.y;
                        Vector3 shoulderBackOffset = -1 * shootDir * shootOffsetAmount.z;
                        Vector3 actionCameraPosition = worldShooterPosition + shoulderRightOffset + shoulderUpOffset + shoulderBackOffset;

                        actionCameraGameObject.transform.position = actionCameraPosition;
                        actionCameraGameObject.transform.LookAt(worldTargetPosition + new Vector3(0, shootOffsetAmount.y, 0));
                        ShowActionCamera();
                    }

                    break;
            }
        }

        private void BaseAction_OnAnyActionComplete(BaseAction action)
        {
            switch (action)
            {
                case ShootAction shootAction:
                    HideActionCamera();
                    break;
            }
        }
    }
}
