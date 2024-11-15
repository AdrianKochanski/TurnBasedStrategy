using System;
using UnityEngine;

namespace Game.Core
{
    public class HealthSystem : Damageable
    {
        [SerializeField] private int health = 100;

        public event Action OnDead;
        public event Action OnDamaged;
        private int healthMax;

        private void Awake()
        {
            healthMax = health;
        }

        public override void Damage(int damageAmount, Vector3 source)
        {
            health -= damageAmount;
            OnDamaged?.Invoke();

            if (health <= 0)
            {
                health = 0;
                Die();
            }
        }

        private void Die()
        {
            OnDead?.Invoke();
        }

        public bool IsDead()
        {
            return health <= 0; 
        }

        public float GetHealthNormalized()
        {
            return (float)health / healthMax;
        }
    }
}
