using Game.Units;
using System;

namespace Game.Interactions
{
    public interface IInteractable
    {
        float Interact();
        bool CanInteract();
        void SetInteractableAtGrid();
    }
}
