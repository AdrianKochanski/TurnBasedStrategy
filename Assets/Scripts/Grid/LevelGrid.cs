using Game.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Grid
{
    public class LevelGrid : MonoBehaviour
    {
        public static LevelGrid Instance { get; private set; }

        [SerializeField] private int width = 10;
        [SerializeField] private int height = 10;
        [SerializeField] private float cellSize = 2f;
        [SerializeField] private Transform gridObjectPrefab;

        private GridSystem gridSystem;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError($"There's more than one LevelGrid! {transform} - {Instance}");
                Destroy(gameObject);
                return;
            }
            Instance = this;

            gridSystem = new GridSystem(width, height, cellSize);
            gridSystem.CreateDebugObjects(gridObjectPrefab, transform);
        }

        public IEnumerable<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
        {
            if (gridSystem.TryGetGridObject(gridPosition, out GridObject gridObject))
            {
                return gridObject.GetUnitList();
            }

            return Enumerable.Empty<Unit>();
        }

        public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
        {
            if (gridSystem.TryGetGridObject(gridPosition, out GridObject gridObject))
            {
                gridObject.AddUnit(unit);
            }
        }

        public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
        {
            if (gridSystem.TryGetGridObject(gridPosition, out GridObject gridObject))
            {
                gridObject.RemoveUnit(unit);
            }
        }

        public void UnitMovedGridPosition(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition)
        {
            RemoveUnitAtGridPosition(fromGridPosition, unit);
            AddUnitAtGridPosition(toGridPosition, unit);
        }

        public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);
        public bool IsValidGridPosition(GridPosition gridPosition) => gridSystem.IsValidGridPosition(gridPosition);
        public bool IsGridBorder(GridPosition gridPosition) => gridSystem.IsGridBorder(gridPosition);
        public Vector3 GetWorldPositon(GridPosition gridPosition) => gridSystem.GetWorldPositon(gridPosition);
        public int GetWidth() => gridSystem.GetWidth();
        public int GetHeight() => gridSystem.GetHeight();
        public bool IsUnitInsideTheGrid(Unit unit) => gridSystem.IsValidGridPosition(unit.GetGridPosition());

        public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
        {
            if (gridSystem.TryGetGridObject(gridPosition, out GridObject gridObject))
            {
                return gridObject.HasAnyUnit();
            }
            return false;
        }
    }
}