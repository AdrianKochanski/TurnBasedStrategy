using Game.Actions;
using Game.Grid;
using Game.Projectiles;
using UnityEngine;

namespace Game.Units
{
    public class UnitAnimator : MonoBehaviour
    {
        [SerializeField] private Animator unitAnimator;
        [SerializeField] private Transform bulletProjectilePrefab;
        [SerializeField] private Transform grenadeProjectilePrefab;
        [SerializeField] private Transform shootPointTransform;
        [SerializeField] private Transform throwPointTransform;
        [SerializeField] private Transform rifleTransform;
        [SerializeField] private Transform swordTransform;

        private void Awake()
        {
            if(TryGetComponent(out MoveAction moveAction))
            {
                moveAction.onActionBegin += MoveAction_StartMoving;
                moveAction.onActionComplete += MoveAction_StopMoving;
            }

            if (TryGetComponent(out ShootAction shootAction))
            {
                shootAction.OnShoot += ShootAction_Shoot;
            }

            if (TryGetComponent(out GrenadeAction grenadeAction))
            {
                grenadeAction.OnThrow += (Unit shootingUnit, GridPosition targetGrid) =>
                {
                    GrenadeAction_OnThrow(shootingUnit, targetGrid, grenadeAction);
                };
            }

            if (TryGetComponent(out SwordAction swordAction))
            {
                swordAction.onActionBegin += SwordAction_OnActionBegin;
                swordAction.onActionComplete += SwordAction_OnActionComplete;
            }
        }

        private void ShootAction_Shoot(Unit shootingUnit, Unit targetUnit)
        {
            unitAnimator.SetTrigger("Shoot");
            Transform bulletProjectileTransform = Instantiate(bulletProjectilePrefab, shootPointTransform.position, Quaternion.identity);
            BulletProjectile bulletProjectile = bulletProjectileTransform.GetComponent<BulletProjectile>();
            bulletProjectile.Setup(shootingUnit, targetUnit);
        }

        private void GrenadeAction_OnThrow(Unit shootingUnit, GridPosition targetGrid, GrenadeAction grenadeAction)
        {
            //unitAnimator.SetTrigger("Throw");
            Transform projectileTransform = Instantiate(grenadeProjectilePrefab, throwPointTransform.position, Quaternion.identity);
            GrenadeProjectile grenadeProjectile = projectileTransform.GetComponent<GrenadeProjectile>();
            grenadeProjectile.Setup(shootingUnit, targetGrid, grenadeAction.TargetReached);
        }

        private void SwordAction_OnActionBegin()
        {
            EquipSword();
            unitAnimator.SetTrigger("SwordSlash");
        }

        private void SwordAction_OnActionComplete()
        {
            EquipRifle();
        }

        private void MoveAction_StartMoving()
        {
            unitAnimator.SetBool("IsWalking", true);
        }

        private void MoveAction_StopMoving()
        {
            unitAnimator.SetBool("IsWalking", false);
        }

        private void EquipSword()
        {
            rifleTransform.gameObject.SetActive(false);
            swordTransform.gameObject.SetActive(true);
        }

        private void EquipRifle()
        {
            rifleTransform.gameObject.SetActive(true);
            swordTransform.gameObject.SetActive(false);
        }
    }
}
