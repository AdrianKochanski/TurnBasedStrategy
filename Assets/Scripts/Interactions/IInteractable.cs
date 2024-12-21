using Game.Units;
using System;

namespace Game.Interactions
{
    public interface IInteractable
    {
        void Interact();
        bool FinishedInteraction();
        bool CanInteract(Unit unit);
        void SetInteractableAtGrid();
    }
}
