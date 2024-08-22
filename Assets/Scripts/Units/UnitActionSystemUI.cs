using Game.Actions;
using Game.Units;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Units
{
    public class UnitActionSystemUI : MonoBehaviour
    {
        [SerializeField] private Transform actionButtonUIPrefab;
        [SerializeField] private Transform actionButtonContainer;

        private List<UnitActionButtonUI> unitActionButtonUIList;

        private void Awake()
        {
            unitActionButtonUIList = new List<UnitActionButtonUI>();
        }

        private void Start()
        {
            UnitActionSystem.Instance.OnSelectedUnitChange += UnitActionSystem_OnSelectedUnitChange;
            UnitActionSystem.Instance.OnSelectedActionChange += UnitActionSystem_OnSelectedActionChange;
            UnitActionSystem.Instance.OnBusyChange += UnitActionSystem_OnBusyChange;
            CreateUnitActionButtons(UnitActionSystem.Instance.GetSelectedUnit());
        }

        private void UnitActionSystem_OnSelectedUnitChange(Unit selectedUnit)
        {
            CreateUnitActionButtons(selectedUnit);
        }

        private void UnitActionSystem_OnSelectedActionChange(BaseAction selectedAction)
        {
            UpdateSelectedVisual(selectedAction);
        }

        private void UnitActionSystem_OnBusyChange(bool isBusy)
        {
            UpdateButtonInteractable(!isBusy);
        }

        private void CreateUnitActionButtons(Unit selectedUnit)
        {
            foreach (Transform child in actionButtonContainer)
            {
                Destroy(child.gameObject);
            }

            unitActionButtonUIList.Clear();
            foreach (var baseAction in selectedUnit.GetBaseActions())
            {
                Transform actionButtonTransform = Instantiate(actionButtonUIPrefab, actionButtonContainer);
                UnitActionButtonUI actionButtonUI = actionButtonTransform.GetComponent<UnitActionButtonUI>();
                unitActionButtonUIList.Add(actionButtonUI);
                actionButtonUI.SetBaseAction(baseAction);
            }

            UpdateSelectedVisual(UnitActionSystem.Instance.GetSelectedAction());
        }

        private void UpdateSelectedVisual(BaseAction selectedAction)
        {
            foreach (UnitActionButtonUI button in unitActionButtonUIList)
            {
                button.UpdateSelectedVisual(selectedAction);
            }
        }

        private void UpdateButtonInteractable(bool interactable)
        {
            foreach (UnitActionButtonUI button in unitActionButtonUIList)
            {
                button.UpdateButtonInteractable(interactable);
            }
        }
    }
}
