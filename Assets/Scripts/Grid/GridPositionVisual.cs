using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Grid
{
    public class GridPositionVisual : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;

        public void Show()
        {
            meshRenderer.enabled = true;
        }

        public void Hide()
        {
            meshRenderer.enabled = false;
        }
    }
}
