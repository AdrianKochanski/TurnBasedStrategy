using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core
{
    public class HealthSystem : MonoBehaviour
    {
        [SerializeField] private int health = 100;

        public event Action OnDead;

        public void Damage(int damageAmount)
        {
            health -= damageAmount;

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
    }
}
