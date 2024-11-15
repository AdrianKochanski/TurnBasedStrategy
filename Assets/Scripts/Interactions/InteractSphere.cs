using Game.Grid;
using System;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Interactions
{
    public class InteractSphere : MonoBehaviour, IInteractable
    {
        [SerializeField] private Material greenMaterial;
        [SerializeField] private Material redMaterial;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private float afterInteractiontTime = .5f;

        private bool isGreen;

        private void Start()
        {
            SetInteractableAtGrid();
            SetColorGreen();
        }

        private void SetColorGreen()
        {
            isGreen = true;
            meshRenderer.material = greenMaterial;
        }

        private void SetColorRed() 
        { 
            isGreen = false;
            meshRenderer.material = redMaterial;
        }

        public float Interact()
        {
            if(isGreen)
            {
                SetColorRed();
            }
            else
            {
                SetColorGreen();
            }

            return afterInteractiontTime;
        }

        public bool CanInteract()
        {
            return true;
        }

        public void SetInteractableAtGrid()
        {
            var position = LevelGrid.Instance.GetGridPosition(transform.position);
            LevelGrid.Instance.SetInteractableAtGrid(position, this);
        }
    }
}
