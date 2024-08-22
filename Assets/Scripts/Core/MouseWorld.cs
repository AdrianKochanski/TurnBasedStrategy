using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core
{
    public class MouseWorld : MonoBehaviour
    {
        private static MouseWorld Instance;
        [SerializeField] private LayerMask mousePlaneLayerMask;

        private void Awake()
        {
            Instance = this;
        }

        //private void Update()
        //{
        //    Vector3? hitPoint = GetPosition();
        //    if (hitPoint.HasValue)
        //    {
        //        transform.position = hitPoint.Value;
        //    }
        //}

        public static bool TryGetPosition(out Vector3 position)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool wasHit = Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, Instance.mousePlaneLayerMask);
            if (wasHit)
            {
                position = hit.point;
                return true;
            }

            position = Vector3.zero;
            return false;
        }
    }
}
