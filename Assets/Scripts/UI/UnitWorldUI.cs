using Game.Units;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class UnitWorldUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI actionPointsText;
        [SerializeField] protected Unit unit;

        private void Start()
        {
            SetupUnitAction();
            UpdateActionPointsText();
        }

        private void SetupUnitAction()
        {
            foreach (var action in unit.GetBaseActions())
            {
                action.onActionBegin += Unit_onActionBegin;
                action.onRestorePoints += Unit_onRestorePoints;
            }
        }

        private void Unit_onActionBegin()
        {
            Debug.Log("Unit_onActionBegin");
            UpdateActionPointsText();
        }

        private void Unit_onRestorePoints()
        {
            Debug.Log("Unit_onRestorePoints");
            UpdateActionPointsText();
        }

        private void UpdateActionPointsText()
        {
            Debug.Log(unit.GetActionPoints());
            actionPointsText.text = unit.GetActionPoints().ToString();
        }
    }
}
