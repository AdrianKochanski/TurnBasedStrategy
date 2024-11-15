using UnityEngine;

namespace Game.Core
{
    public class DestructiblePart : Damageable
    {
        private float destructForceMultiplier;
        private float destructRadius;

        public void ApplyExplosionToPart(Vector3 explosionPosition, int damageAmount, float destructForceMultiplier, float destructRadius)
        {
            this.destructForceMultiplier = destructForceMultiplier;
            this.destructRadius = destructRadius;

            if (TryGetComponent(out Rigidbody rigidBody))
            {
                rigidBody.AddExplosionForce(
                    GetDestructForce(damageAmount),
                    explosionPosition,
                    destructRadius
                );
            }
        }

        public override void Damage(int damageAmount, Vector3 source)
        {
            ApplyExplosionToPart(source, damageAmount, destructForceMultiplier, destructRadius);
        }

        public float GetDestructForce(float damage)
        {
            return destructForceMultiplier * damage;
        }
    }
}