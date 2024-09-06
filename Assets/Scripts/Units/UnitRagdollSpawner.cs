using Game.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Units
{
    public class UnitRagdollSpawner : MonoBehaviour
    {
        [SerializeField] private Transform ragdollPrefab;
        [SerializeField] private Transform ragdollRootBone;

        private HealthSystem healthSystem;

        private void Awake()
        {
            healthSystem = GetComponent<HealthSystem>();
            healthSystem.OnDead += HealthSystem_OnDead;
        }

        private void HealthSystem_OnDead()
        {
            Transform ragdollTransform = Instantiate(ragdollPrefab, transform.position, transform.rotation);
            UnitRagdoll unitRagdoll = ragdollTransform.GetComponent<UnitRagdoll>();
            unitRagdoll.Setup(ragdollRootBone);
        }
    }
}
