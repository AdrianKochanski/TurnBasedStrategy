using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Units
{
    public class UnitSelectedVisual : MonoBehaviour
    {
        [SerializeField] private Unit unit;

        private MeshRenderer meshRenderer;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Start()
        {
            UnitActionSystem.Instance.OnSelectedUnitChange += UnitActionSystem_OnSelectedUnitChange;
            UpdateVisual(UnitActionSystem.Instance.GetSelectedUnit());
        }

        private void OnDestroy()
        {
            UnitActionSystem.Instance.OnSelectedUnitChange -= UnitActionSystem_OnSelectedUnitChange;
        }

        private void UnitActionSystem_OnSelectedUnitChange(Unit selectedUnit)
        {
            UpdateVisual(selectedUnit);
        }

        private void UpdateVisual(Unit selectedUnit)
        {
            meshRenderer.enabled = unit == selectedUnit;
        }
    }
}
