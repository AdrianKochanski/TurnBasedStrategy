using Game.Core;
using Game.Grid;
using Game.Units;
using System;
using UnityEngine;

namespace Game.Projectiles
{
    public class GrenadeProjectile : MonoBehaviour
    {
        [SerializeField] private float reachedTargetDistance = .2f;
        [SerializeField] private float bulletSpeed = 15f;
        [SerializeField] private int damageAmount = 15;
        [SerializeField] private float damageRadius = 4.0f;
        [SerializeField] private GameObject hitEffect = null;
        [SerializeField] private TrailRenderer trailRenderer = null;
        [SerializeField] private AnimationCurve arcYAnimationCurve;

        public Action OnGrenadeExplode;
        public static Action OnAnyGrenadeExplode;
        private float startingFromY = 1f;
        private HealthSystem unitHealth;
        private Vector3 targetPosition;
        private float totalDistance;

        void Update()
        {
            Vector3 targetCurvePosition = CurvePosition(targetPosition);
            Vector3 moveDir = Tangent(targetCurvePosition);
            float distanceToTravel = bulletSpeed * Time.deltaTime;
            Vector3 newPosition = CurvePosition(transform.position + (moveDir * distanceToTravel));
            moveDir = (newPosition - transform.position).normalized;

            // In case of hit something before reach the target position
            //Debug.Log($"target: {targetCurvePosition}, current: {transform.position}, new: {newPosition}, dir: {moveDir}");
            Debug.DrawLine(transform.position, newPosition, Color.red, 120f);

            if (Physics.Raycast(transform.position, moveDir, out RaycastHit hit, distanceToTravel) && WasTargetHit(hit))
            {
                OnTargetReached(hit.point);
            }
            else if (Vector3.Distance(transform.position, targetPosition) < reachedTargetDistance)
            {
                OnTargetReached(targetPosition);
            }

            transform.position = newPosition;
        }

        private Vector3 Tangent(Vector3 toPosition)
        {
            return (toPosition - transform.position).normalized;
        }

        private Vector3 FlatPosition(Vector3 fromPosition)
        {
            return new Vector3(fromPosition.x, 0f, fromPosition.z);
        }

        private Vector3 CurvePosition(Vector3 fromPosition)
        {
            var fromFlatPos = FlatPosition(fromPosition);
            float distance = Vector3.Distance(fromFlatPos, FlatPosition(targetPosition));
            float distanceNormalized = Math.Clamp(1 - distance / totalDistance, 0f, 1f);
            float positionY = arcYAnimationCurve.Evaluate(distanceNormalized) * startingFromY;
            //Debug.Log($"{positionY}, {arcYAnimationCurve.Evaluate(distanceNormalized)}");
            return new Vector3(fromFlatPos.x, positionY, fromFlatPos.z);
        }

        private bool WasTargetHit(RaycastHit hit)
        {
            if(hit.collider.TryGetComponent(out Damageable objectHit))
            {
                if (hit.collider.TryGetComponent(out HealthSystem objectHealth) && (objectHealth.IsDead() || objectHealth == unitHealth))
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        private void OnTargetReached(Vector3 hitPoint)
        {
            bulletSpeed = 0;
            transform.position = hitPoint;

            if (trailRenderer)
            {
                trailRenderer.transform.parent = null;
            }
            if (hitEffect != null)
            {
                Instantiate(hitEffect, hitPoint + Vector3.up * 1f, Quaternion.identity);
            }
            //trailRenderer.transform.parent = null;

            Collider[] colliderArray = Physics.OverlapSphere(hitPoint, damageRadius);

            foreach (Collider collider in colliderArray)
            {
                if (collider.TryGetComponent(out Damageable targetObject))
                {
                    GridPosition unitPosition = targetObject.GetGridPosition();
                    GridPosition hitGrid = LevelGrid.Instance.GetGridPosition(hitPoint);
                    float distance = LevelGrid.Instance.Distance(hitGrid, unitPosition);
                    float distanceDamage = damageAmount * (damageRadius - distance) / damageRadius;

                    if (distanceDamage > 0)
                    {
                        targetObject.Damage(Mathf.CeilToInt(distanceDamage), hitPoint);
                    }
                }
            }

            OnGrenadeExplode?.Invoke();
            OnAnyGrenadeExplode?.Invoke();
            Destroy(gameObject);
        }

        public void Setup(Unit throwingUnit, GridPosition targetGridPositiont, Action callback)
        {
            startingFromY = transform.position.y;
            this.unitHealth = throwingUnit.GetComponent<HealthSystem>();
            targetPosition = LevelGrid.Instance.GetWorldPositon(targetGridPositiont);
            OnGrenadeExplode += callback;
            totalDistance = Vector3.Distance(FlatPosition(transform.position), FlatPosition(targetPosition));
        }
    }
}
