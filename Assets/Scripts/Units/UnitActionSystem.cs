using Game.Actions;
using Game.Core;
using Game.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static Game.Actions.BaseAction;

namespace Game.Units
{
    public class UnitActionSystem : MonoBehaviour
    {
        public static UnitActionSystem Instance { get; private set; }
        public event Action<Unit> OnSelectedUnitChange;
        public event Action<BaseAction> OnSelectedActionChange;
        public event Action<bool> OnBusyChange;

        [SerializeField] private Unit selectedUnit;
        [SerializeField] private LayerMask unitLayerMask;
        private BaseAction selectedAction;

        private bool isBusy;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError($"There's more than one UnitActionSystem! {transform} - {Instance}");
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            SetSelectedUnit(selectedUnit);
            BaseAction.OnAnyActionBegin += BaseAction_OnAnyActionBegin;
            BaseAction.OnAnyActionComplete += BaseAction_OnAnyActionComplete;
        }

        private void Update()
        {
            if (isBusy) return;
            if (!TurnSystem.Instance.IsPlayerTurn()) return;
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (InputManager.Instance.IsSelectMouseButtonDownThisFrame())
            {
                if (TryHandleUnitSelection()) return;

                HandleSelectedAction();
            };
        }

        public Unit GetSelectedUnit()
        {
            return selectedUnit;
        }

        public void SetSelectedAction(BaseAction newAction)
        {
            selectedAction = newAction;
            OnSelectedActionChange?.Invoke(newAction);
        }

        public BaseAction GetSelectedAction() 
        {
            return selectedAction;
        }

        private void BaseAction_OnAnyActionBegin(BaseAction action)
        {
            isBusy = true;
            OnBusyChange?.Invoke(isBusy);
        }

        private void BaseAction_OnAnyActionComplete(BaseAction action)
        {
            isBusy = false;
            OnBusyChange?.Invoke(isBusy);
        }

        private bool TryHandleUnitSelection()
        {
            Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());

            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, unitLayerMask))
            {
                if (hit.transform.TryGetComponent<Unit>(out Unit newSelectedUnit) && selectedUnit != newSelectedUnit && !newSelectedUnit.IsEnemy())
                {
                    SetSelectedUnit(newSelectedUnit);
                    return true;
                }
            }

            return false;
        }

        private void SetSelectedUnit(Unit unit)
        {
            selectedUnit = unit;
            SetSelectedAction(unit.GetAction<MoveAction>());
            OnSelectedUnitChange?.Invoke(unit);
        }

        private void HandleSelectedAction()
        {
            GridPosition gridPosition = new GridPosition(0, 0);
            bool newPositionFound = MouseWorld.TryGetPosition(out Vector3 mousePosition);
            if (newPositionFound) gridPosition = LevelGrid.Instance.GetGridPosition(mousePosition);

            switch (selectedAction)
            {
                case MoveAction moveAction:
                    moveAction.TryStartAction(new List<GridPosition> { gridPosition });
                    break;
                default:
                    selectedAction.TryStartAction(new List<GridPosition> { gridPosition });
                    break;
            }
        }
    }
}