using System;
using UnityEngine;

namespace Game.Core
{
    public class DestructibleCrate : Damageable
    {
        [SerializeField]
        private Transform crateDestroyPrefab;

        [SerializeField]
        private float destructForceMultiplier = 4.0f;

        [SerializeField]
        private float destructRadius = 10.0f;

        public static Action<DestructibleCrate> OnAnyDestroyed;

        public override void Damage(int damageAmount, Vector3 source)
        {
            Transform crateDestroyedTransform = Instantiate(crateDestroyPrefab, transform.position, transform.rotation);
            ApplyExplosionToParts(crateDestroyedTransform, source, damageAmount);
            Destroy(gameObject);
            OnAnyDestroyed?.Invoke(this);
        }

        private void ApplyExplosionToParts(Transform root, Vector3 explosionPosition, int damageAmount)
        {
            foreach (Transform child in root)
            {
                if (child.TryGetComponent(out DestructiblePart part))
                {
                    part.ApplyExplosionToPart(explosionPosition, damageAmount, destructForceMultiplier, destructRadius);
                }

                ApplyExplosionToParts(child, explosionPosition, damageAmount);
            }
        }

        public float GetDestructRadius()
        {
            return destructRadius;
        }
    }
}
