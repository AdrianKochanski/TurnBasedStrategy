using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core
{
    public class LookAtCamera : MonoBehaviour
    {
        [SerializeField] private bool invert;

        private Transform cameraTransform;

        private void Awake()
        {
            cameraTransform = Camera.main.transform;
        }

        private void LateUpdate()
        {
            if (invert)
            {
                Vector3 revDirToCamera = (transform.position - cameraTransform.position).normalized;
                transform.LookAt(transform.position + revDirToCamera);
            }
            else
            {
                transform.LookAt(cameraTransform);
            }
        }
    }
}
