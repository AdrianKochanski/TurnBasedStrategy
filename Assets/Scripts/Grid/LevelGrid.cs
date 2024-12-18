using Game.Interactions;
using Game.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Grid
{
    public class LevelGrid : MonoBehaviour
    {
        public static LevelGrid Instance { get; private set; }

        public const float FLOOR_HEIGHT = 1.5f;
        [SerializeField] private Transform gridObjectPrefab;
        [SerializeField] private int width = 10;
        [SerializeField] private int height = 10;
        [SerializeField] private float cellSize = 2f;
        [SerializeField] private int floorAmount;

        private List<GridSystemHex<GridObject>> gridSystems;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError($"There's more than one LevelGrid! {transform} - {Instance}");
                Destroy(gameObject);
                return;
            }
            Instance = this;

            gridSystems = new List<GridSystemHex<GridObject>>();
            for (int floor = 0; floor < floorAmount; floor++)
            {
                var gridSystem = new GridSystemHex<GridObject>(width, height, cellSize, floor, FLOOR_HEIGHT, (gS, gP) => new GridObject(gS, gP));
                gridSystems.Add(gridSystem);
                //gridSystem.CreateDebugObjects(gridObjectPrefab, transform);
            }
        }

        private void Start()
        {
            Pathfinding.Instance.Setup(width, height, cellSize, floorAmount);
        }

        private bool TryGetGridSystem(int floor, out GridSystemHex<GridObject> gridSystem)
        {
            if (floor < 0 || floor >= floorAmount)
            {
                gridSystem = null;
                return false;
            }
            gridSystem = gridSystems[floor];
            return true;
        }

        public IEnumerable<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
        {
            if (TryGetGridSystem(gridPosition.floor, out GridSystemHex<GridObject> gridSystem) && gridSystem.TryGetGridObject(gridPosition, out GridObject gridObject))
            {
                return gridObject.GetUnitList();
            }

            return Enumerable.Empty<Unit>();
        }

        public bool TryGetUnitAtGridPosition(GridPosition gridPosition, out Unit unit)
        {
            unit = null;

            if (TryGetGridSystem(gridPosition.floor, out GridSystemHex<GridObject> gridSystem) && gridSystem.TryGetGridObject(gridPosition, out GridObject gridObject)
                && gridObject.TryGetUnit(out unit))
            {
                return true;
            }

            return false;
        }

        public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
        {
            if (TryGetGridSystem(gridPosition.floor, out GridSystemHex<GridObject> gridSystem) && gridSystem.TryGetGridObject(gridPosition, out GridObject gridObject))
            {
                gridObject.AddUnit(unit);
            }
        }

        public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
        {
            if (TryGetGridSystem(gridPosition.floor, out GridSystemHex<GridObject> gridSystem) && gridSystem.TryGetGridObject(gridPosition, out GridObject gridObject))
            {
                gridObject.RemoveUnit(unit);
            }
        }

        public void UnitMovedGridPosition(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition)
        {
            RemoveUnitAtGridPosition(fromGridPosition, unit);
            AddUnitAtGridPosition(toGridPosition, unit);
            //OnAnyUnitMovedGridPosition?.Invoke();
        }

        public int GetFloor(float floorHeight)
        {
            return Mathf.RoundToInt(floorHeight / (FLOOR_HEIGHT * cellSize));
        }

        public bool TryGetGridPosition(Vector3 worldPosition, out GridPosition gridPosition)
        {
            int floor = GetFloor(worldPosition.y);
            if(TryGetGridSystem(floor, out GridSystemHex<GridObject> gridSystem))
            {
                gridPosition = gridSystem.GetGridPosition(worldPosition);
                return true;
            }
            gridPosition = new GridPosition();
            return false;
        }

        public bool IsValidGridPosition(GridPosition gridPosition) => TryGetGridSystem(gridPosition.floor, out GridSystemHex<GridObject> gridSystem) && gridSystem.IsValidGridPosition(gridPosition);

        public bool TryGetWorldPositon(GridPosition gridPosition, out Vector3 worldPosition)
        {
            if(TryGetGridSystem(gridPosition.floor, out GridSystemHex<GridObject> gridSystem))
            {
                worldPosition = gridSystem.GetWorldPositon(gridPosition);
                return true;
            }
            worldPosition = new Vector3();
            return false;
        }
        // TO CONSIDER: Multifloor distance?
        public float Distance(GridPosition from, GridPosition to)
        {
            if (TryGetGridSystem(from.floor, out GridSystemHex<GridObject> gridSystem))
            {
                return gridSystem.Distance(from, to);
            }
            return float.MaxValue;
        }
        public int GetWidth() => width;
        public int GetHeight() => height;
        public float GetCellSize() => cellSize;
        public bool IsUnitInsideTheGrid(Unit unit) => TryGetGridSystem(unit.GetGridPosition().floor, out GridSystemHex<GridObject> gridSystem) && gridSystem.IsValidGridPosition(unit.GetGridPosition());
        public bool RaycastHorizontal(GridPosition from, GridPosition to, LayerMask layerMask, float? offset = 1.7f) => TryGetGridSystem(from.floor, out GridSystemHex<GridObject> gridSystem) && gridSystem.RaycastHorizontal(from, to, layerMask, offset);
        public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
        {
            if (TryGetGridSystem(gridPosition.floor, out GridSystemHex<GridObject> gridSystem) && gridSystem.TryGetGridObject(gridPosition, out GridObject gridObject))
            {
                return gridObject.HasAnyUnit();
            }
            return false;
        }

        public bool TryGetInteractableAtGrid(GridPosition gridPosition, out IInteractable interactable)
        {
            interactable = null;

            if(TryGetGridSystem(gridPosition.floor, out GridSystemHex<GridObject> gridSystem) && gridSystem.TryGetGridObject(gridPosition, out GridObject gridObject))
            {
                interactable = gridObject.GetInteractable();
                if(interactable == null)
                {
                    return false;
                }
                return true;
            }

            return false;
        }

        public void SetInteractableAtGrid(GridPosition gridPosition, IInteractable interactable)
        {
            if (TryGetGridSystem(gridPosition.floor, out GridSystemHex<GridObject> gridSystem) && gridSystem.TryGetGridObject(gridPosition, out GridObject gridObject))
            {
                gridObject.SetInteractable(interactable);
            }
        }

        internal List<GridPosition> GetSurroundingGridsInLine(Transform transform, int positionsCount)
        {
            List<GridPosition> positions = new List<GridPosition>();
            Vector3 startingPosition = transform.position + transform.right * cellSize * ((float)(positionsCount - 1) / 2) * (-1);
            float positionOffset = 0.20f;
            Vector3 offsetVector = transform.forward * cellSize * positionOffset;

            Action<Vector3> AddToList = (Vector3 position) => {
                if(TryGetGridPosition(position, out GridPosition calculatedPosition) && !positions.Contains(calculatedPosition))
                {
                    positions.Add(calculatedPosition);
                }
            };

            Vector3 vectorPosition = transform.position;
            Vector3 forwardPositionOffset = vectorPosition + offsetVector;
            Vector3 backwardPositionOffset = vectorPosition - offsetVector;
            AddToList(forwardPositionOffset);
            AddToList(backwardPositionOffset);

            for (int i = 0; i < positionsCount; i++)
            {
                vectorPosition = startingPosition + transform.right * cellSize * i;
                forwardPositionOffset = vectorPosition + offsetVector;
                backwardPositionOffset = vectorPosition - offsetVector;

                //Debug.DrawLine(vectorPosition, forwardPositionOffset, Color.red, 300f);
                //Debug.DrawLine(vectorPosition, backwardPositionOffset, Color.red, 300f);
                AddToList(forwardPositionOffset);
                AddToList(backwardPositionOffset);
            }

            return positions;
        }

        internal int GetFloorAmount()
        {
            return floorAmount;
        }

#if UNITY_EDITOR
        public void DrawLine(List<GridPosition> positions)
        {
            for (int i = 0; i < positions.Count - 1; i++)
            {
                DrawLine(positions[i], positions[i + 1]);
            }
        }

        public void DrawLine(GridPosition fromGrid, GridPosition toGrid)
        {
            if (TryGetWorldPositon(fromGrid, out Vector3 from)
                && TryGetWorldPositon(toGrid, out Vector3 to))
            {
                Debug.DrawLine(from, to, Color.red, 3600f);
            }
        }
#endif
    }
}