using Game.Actions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Units
{
    public class UnitActionButtonUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textMeshPro;
        [SerializeField] private Button button;
        [SerializeField] private Transform selectedVisual;

        private BaseAction baseAction;

        public void SetBaseAction(BaseAction baseAction)
        {
            this.baseAction = baseAction;
            this.textMeshPro.text = baseAction.GetActionName();
            button.onClick.AddListener(() => { 
                UnitActionSystem.Instance.SetSelectedAction(baseAction);
            });
        }

        public void UpdateSelectedVisual(BaseAction selectedAction)
        {
            selectedVisual.gameObject.SetActive(baseAction == selectedAction);
        }

        public void UpdateButtonInteractable(bool interactable)
        {
            button.interactable = interactable;
        }
    }
}
