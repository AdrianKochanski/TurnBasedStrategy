using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Units
{
    public class UnitRagdoll : MonoBehaviour
    {
        [SerializeField] private Transform ragdollRootBone;
        [SerializeField] private float explosionForce = 300f;
        [SerializeField] private float explosionRange = 10f;

        public void Setup(Transform originalRootBone)
        {
            SetupChildsTransform(originalRootBone, ragdollRootBone);
            AddExplosionToRagdoll(ragdollRootBone, explosionForce, transform.position, explosionRange);
        }

        private void SetupChildsTransform(Transform root, Transform clone)
        {
            foreach (Transform child in root)
            {
                Transform cloneChild = clone.Find(child.name);
                if (cloneChild != null)
                {
                    cloneChild.position = child.position;
                    cloneChild.rotation = child.rotation;
                    SetupChildsTransform (child, cloneChild);
                }
            }
        }

        private void AddExplosionToRagdoll(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange)
        {
            foreach(Transform child in root)
            {
                if(child.TryGetComponent(out Rigidbody childRigidBody))
                {
                    childRigidBody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
                }

                AddExplosionToRagdoll(child, explosionForce, explosionPosition, explosionRange);
            }
        }
    }
}
