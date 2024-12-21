using Game.Grid;
using Game.Units;
using UnityEngine;

namespace Game.Interactions
{
    public class InteractSphere : MonoBehaviour, IInteractable
    {
        [SerializeField] private Material greenMaterial;
        [SerializeField] private Material redMaterial;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private float afterInteractiontTime = .5f;

        private float timer;
        private bool isGreen;

        private void Start()
        {
            SetInteractableAtGrid();
            SetColorGreen();
        }

        private void Update()
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                timer = 0;
            }
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

        public void Interact()
        {
            if(isGreen)
            {
                SetColorRed();
            }
            else
            {
                SetColorGreen();
            }
        }

        public bool CanInteract(Unit unit)
        {
            return true;
        }

        public void SetInteractableAtGrid()
        {
            if(LevelGrid.Instance.TryGetGridPosition(transform.position, out GridPosition position))
            {
                LevelGrid.Instance.SetInteractableAtGrid(position, this);
            }
        }

        public bool FinishedInteraction()
        {
            return timer == 0;
        }
    }
}
