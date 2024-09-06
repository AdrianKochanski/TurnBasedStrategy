using Game.Actions;
using Game.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Units
{
    public class UnitAnimator : MonoBehaviour
    {
        [SerializeField] private Animator unitAnimator;
        [SerializeField] private Transform bulletProjectilePrefab;
        [SerializeField] private Transform shootPointTransform;

        private void Awake()
        {
            if(TryGetComponent(out MoveAction moveAction))
            {
                moveAction.OnStartMoving += MoveAction_StartMoving;
                moveAction.OnStopMoving += MoveAction_StopMoving;
            }

            if (TryGetComponent(out ShootAction shootAction))
            {
                shootAction.OnShoot += ShootAction_Shoot;
            }
        }

        private void ShootAction_Shoot(Unit shootingUnit, Unit targetUnit)
        {
            unitAnimator.SetTrigger("Shoot");
            Transform bulletProjectileTransform = Instantiate(bulletProjectilePrefab, shootPointTransform.position, Quaternion.identity);
            BulletProjectile bulletProjectile = bulletProjectileTransform.GetComponent<BulletProjectile>();
            bulletProjectile.Setup(shootingUnit, targetUnit);
        }

        private void MoveAction_StartMoving()
        {
            unitAnimator.SetBool("IsWalking", true);
        }

        private void MoveAction_StopMoving()
        {
            unitAnimator.SetBool("IsWalking", false);
        }
    }
}
