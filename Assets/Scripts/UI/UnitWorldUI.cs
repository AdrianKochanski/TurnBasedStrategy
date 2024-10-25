using Game.Core;
using Game.Units;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UnitWorldUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI actionPointsText;
        [SerializeField] private Image healthBarImage;
        [SerializeField] protected Unit unit;
        [SerializeField] private HealthSystem healthSystem;

        private void Start()
        {
            SetupUnitAction();
            UpdateActionPointsText();
            UpdateHealthBar();
            healthSystem.OnDamaged += HealthSystem_OnDamaged;
        }

        private void SetupUnitAction()
        {
            foreach (var action in unit.GetBaseActions())
            {
                action.onActionComplete += Unit_onActionComplete;
                action.OnRestorePoints += Unit_onRestorePoints;
            }
        }

        private void Unit_onActionComplete()
        {
            UpdateActionPointsText();
        }

        private void Unit_onRestorePoints()
        {
            UpdateActionPointsText();
        }

        private void UpdateActionPointsText()
        {
            actionPointsText.text = unit.GetActionPoints().ToString();
        }

        private void HealthSystem_OnDamaged()
        {
            UpdateHealthBar();
        }

        private void UpdateHealthBar()
        {
            healthBarImage.fillAmount = healthSystem.GetHealthNormalized();
        }
    }
}
