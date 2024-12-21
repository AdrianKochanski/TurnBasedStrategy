using Game.Grid;
using Game.Units;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Interactions
{
    public class Door : MonoBehaviour, IInteractable
    {
        [SerializeField] private bool isOpen;
        private Animator animator;
        private List<GridPosition> positions;
        [SerializeField] private float afterInteractiontTime = .5f;

        private float timer;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            SetInteractableAtGrid();

            if (isOpen)
            {
                OpenDoor();
            }
            else
            {
                CloseDoor();
            }
        }

        private void Update()
        {
            if(timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                timer = 0;
            }
        }

        public void Interact()
        {
            timer = afterInteractiontTime;
            if (isOpen)
            {
                CloseDoor();
            }
            else
            {
                OpenDoor();
            }
        }

        public bool CanInteract(Unit unit)
        {
            if (isOpen)
            {
                return CanBeClosed();
            }
            else
            {
                return true;
            }
        }

        private bool CanBeClosed()
        {
            return !positions.Any(position => LevelGrid.Instance.TryGetUnitAtGridPosition(position, out Unit unit));
        }

        public void OpenDoor()
        {
            isOpen = true;
            animator.SetBool("IsOpen", true);
            foreach (var position in positions)
            {
                Pathfinding.Instance.SetIsWalkableGridPosition(position, true);
            }
        }

        public void CloseDoor()
        {
            if (!CanBeClosed()) return;
            isOpen = false;
            animator.SetBool("IsOpen", false);
            foreach (var position in positions)
            {
                Pathfinding.Instance.SetIsWalkableGridPosition(position, false);
            }
        }

        public void SetInteractableAtGrid()
        {
            positions = LevelGrid.Instance.GetSurroundingGridsInLine(transform, Mathf.CeilToInt(transform.localScale.x));
            foreach (var position in positions)
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
