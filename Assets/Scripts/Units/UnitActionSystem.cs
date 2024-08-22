using Game.Actions;
using Game.Core;
using Game.Grid;
using System;
using System.Linq;
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
        }

        private void Update()
        {
            if (isBusy) return;
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (Input.GetMouseButtonDown(0))
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

        private void SetBusy()
        {
            isBusy = true;
            OnBusyChange?.Invoke(isBusy);
        }

        private void ClearBusy()
        {
            isBusy = false;
            OnBusyChange?.Invoke(isBusy);
        }

        private bool TryHandleUnitSelection()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, unitLayerMask))
            {
                if (hit.transform.TryGetComponent<Unit>(out Unit newSelectedUnit) && selectedUnit != newSelectedUnit)
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
            SetSelectedAction(unit.GetBaseActions().First());
            SetupUnitAction();
            OnSelectedUnitChange?.Invoke(unit);
        }

        private void SetupUnitAction()
        {
            foreach (var action in selectedUnit.GetBaseActions())
            {
                if(!action.IsActionSetup())
                {
                    action.onActionBegin += SetBusy;
                    action.onActionComplete += ClearBusy;
                }
            }
        }

        private void HandleSelectedAction()
        {
            GridPosition gridPosition = new GridPosition(0, 0);
            bool newPositionFound = MouseWorld.TryGetPosition(out Vector3 mousePosition);
            if (newPositionFound) gridPosition = LevelGrid.Instance.GetGridPosition(mousePosition);

            if(selectedAction.IsValidActionGridPositon(gridPosition))
            {
                switch (selectedAction)
                {
                    case MoveAction moveAction:
                        moveAction.StartAction(new BaseActionParameters() { gridPosition = gridPosition });
                        break;
                    case SpinAction spinAction:
                        spinAction.StartAction(new BaseActionParameters() { gridPosition = gridPosition });
                        break;
                    default:
                        selectedAction.StartAction(new BaseActionParameters() { gridPosition = gridPosition });
                        break;
                }
            }
        }
    }

}