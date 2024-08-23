using Game.Actions;
using Game.Core;
using Game.Units;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UnitActionButtonUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textMeshPro;
        [SerializeField] private Button button;
        [SerializeField] private Transform selectedVisual;
        [SerializeField] private TextMeshProUGUI actionCountText;
        [SerializeField] private Color activeColor = Color.white;
        [SerializeField] private Color disabledColor = Color.white;

        private BaseAction baseAction;
        private Image selectedVisualImage;

        private void Awake()
        {
            selectedVisualImage = selectedVisual.GetComponent<Image>();
        }

        public void SetBaseAction(BaseAction baseAction)
        {
            this.baseAction = baseAction;
            this.textMeshPro.text = baseAction.GetActionName();
            button.onClick.AddListener(() => { 
                UnitActionSystem.Instance.SetSelectedAction(baseAction);
            });
            UpdateActionCountText();
            baseAction.onRestorePoints += BaseAction_OnRestorePoints;
        }

        private void BaseAction_OnRestorePoints()
        {
            UpdateActionCountText();
            UpdateSelectedVisual(UnitActionSystem.Instance.GetSelectedAction());
        }

        private void OnDestroy()
        {
            if (baseAction != null)
            {
                baseAction.onRestorePoints -= BaseAction_OnRestorePoints;
            }
        }

        public void UpdateSelectedVisual(BaseAction selectedAction)
        {
            selectedVisual.gameObject.SetActive(baseAction == selectedAction);
            int count = baseAction.GetPossibleActionsCount();

            if (count > 0)
            {
                selectedVisualImage.color = activeColor;
            }
            else
            {
                selectedVisualImage.color = disabledColor;
            }
        }

        public void UpdateButtonInteractable(bool interactable)
        {
            button.interactable = interactable;
        }

        public void UpdateActionCountText()
        {
            int count = baseAction.GetPossibleActionsCount();

            if (count > 0)
            {
                actionCountText.color = activeColor;
            }
            else
            {
                actionCountText.color = disabledColor;
            }

            actionCountText.text = $"{count}";
        }
    }
}
