using Game.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core
{
    public class BulletProjectile : MonoBehaviour
    {
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private float bulletSpeed = 20f;
        [SerializeField] private int damageAmount = 20;
        [SerializeField] bool isHoming = false;
        [SerializeField] GameObject hitEffect = null;
        private Unit shootingUnit;
        private Unit targetUnit;

        public event Action<RaycastHit> onHit;

        private void Update()
        {
            if (targetUnit == null) return;
            if (isHoming)// && !target.IsDead())
            {
                transform.LookAt(GetAimLocation());
            }

            // Better version of OnTriggerEnter for fast moving objects
            float distanceToTravel = bulletSpeed * Time.deltaTime;
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, distanceToTravel)) OnRaycastHit(hit);
            transform.Translate(Vector3.forward * distanceToTravel);
        }

        public void Setup(Unit shootingUnit, Unit targetUnit)
        {
            this.shootingUnit = shootingUnit;
            this.targetUnit = targetUnit;
            transform.LookAt(GetAimLocation());
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = targetUnit.GetComponent<CapsuleCollider>();
            if (targetCapsule == null)
            {
                Vector3 targetPositon = targetUnit.GetWorldPositon();
                targetPositon.y = transform.position.y;
                return targetPositon;
            }
            return targetUnit.transform.position + Vector3.up * targetCapsule.height * 2 / 3;
        }

        private void OnRaycastHit(RaycastHit hit)
        {
            if (hit.collider.TryGetComponent(out Unit unitHit) && !unitHit.IsDead())
            {
                onHit?.Invoke(hit);
                unitHit.Damage(damageAmount);
                bulletSpeed = 0;

                if (hitEffect != null)
                {
                    Instantiate(hitEffect, hit.point, transform.rotation);
                }

                transform.position = hit.point;
                trailRenderer.transform.parent = null;
                Destroy(gameObject);
            }
        }
    }
}
