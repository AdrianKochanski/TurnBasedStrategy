using Game.Actions;
using Game.Core;
using Game.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Grid
{
    public class GridSystemVisual : MonoBehaviour
    {
        [SerializeField] GridPositionVisual GridPositionVisualPrefab;
        [SerializeField] List<GridVisualTypeMaterial> gridVisualTypeMaterials;

        [Serializable]
        public struct GridVisualTypeMaterial
        {
            public GridVisualType gridVisualType;
            public Material material;
        }

        [Serializable]
        public enum GridVisualType
        {
            White,
            Blue,
            Red,
            Yellow,
            Green,
            RedSoft
        }

        private GridPositionVisual[,,] gridPositionVisuals;


        private void Start()
        {
            InitalizeGridVisual();
            UnitActionSystem.Instance.OnSelectedActionChange += UnitActionSystem_OnSelectedActionChange;
            TurnSystem.Instance.OnTurnChange += TurnSystem_OnTurnChange;
            BaseAction.OnAnyActionGridUpdate += BaseAction_OnAnyActionGridUpdate;
            UpdateActionGrid();
            //ShowAllPositons();
        }

        //private void Update()
        //{
        //    HighligtPositionSelected();
        //}

        private void UnitActionSystem_OnSelectedActionChange(BaseAction action)
        {
            UpdateActionGrid();
        }

        private void TurnSystem_OnTurnChange(int turnNumber)
        {
            UpdateActionGrid();
        }

        private void BaseAction_OnAnyActionGridUpdate(BaseAction action)
        {
            UpdateGridPositionVisuals(action.GetActionGridPositions());
        }

        public void HideAllGridPosition()
        {
            foreach (var gridPosition in gridPositionVisuals) gridPosition.Hide();
        }

        public void ShowGridPositionList(IEnumerable<GridPosition> gridPositionList, GridVisualType gridVisualType)
        {
            foreach (GridPosition gridPosition in gridPositionList) 
            {
                if(LevelGrid.Instance.IsValidGridPosition(gridPosition))
                {
                    gridPositionVisuals[gridPosition.x, gridPosition.z, gridPosition.floor].Show(GetGridVisualTypeMaterial(gridVisualType));
                }
            }
        }

        private void InitalizeGridVisual()
        {
            int width = LevelGrid.Instance.GetWidth();
            int height = LevelGrid.Instance.GetHeight();
            int floors = LevelGrid.Instance.GetFloorAmount();

            gridPositionVisuals = new GridPositionVisual[width, height, floors];
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    for (int floor = 0; floor < floors; floor++)
                    {
                        GridPosition gridPosition = new GridPosition(x, z, floor);
                        if(LevelGrid.Instance.TryGetWorldPositon(gridPosition, out Vector3 worldPosition))
                        {
                            GridPositionVisual gridPositionVisualInstance = Instantiate(
                                GridPositionVisualPrefab,
                                worldPosition,
                                Quaternion.identity,
                                transform
                            );
                            gridPositionVisuals[x, z, floor] = gridPositionVisualInstance;
                        }
                    }
                }
            }
        }

        private void UpdateActionGrid()
        {
            BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
            selectedAction.UpdateActionGridPositions();
        }

        private void UpdateGridPositionVisuals(Dictionary<GridVisualType, List<(GridPosition, float)>> actionGridPositions)
        {
            HideAllGridPosition();
            foreach (var gridPositionsList in actionGridPositions) { 
                ShowGridPositionList(gridPositionsList.Value.Select(i => i.Item1), gridPositionsList.Key);
            }
        }

        private void ShowAllPositons()
        {
            int width = LevelGrid.Instance.GetWidth();
            int height = LevelGrid.Instance.GetHeight();
            int floors = LevelGrid.Instance.GetFloorAmount();

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    for (int floor = 0; floor < floors; floor++)
                    {
                        gridPositionVisuals[x, z, floor].Show(GetGridVisualTypeMaterial(GridVisualType.White));
                    }
                }
            }
        }

        private GridPositionVisual lastSelectedPosition;
        private void HighligtPositionSelected()
        {
            if(MouseWorld.TryGetPosition(out Vector3 mouseWorldPosition) 
                && LevelGrid.Instance.TryGetGridPosition(mouseWorldPosition, out GridPosition gridPosition) 
                && LevelGrid.Instance.IsValidGridPosition(gridPosition))
            {
                if(lastSelectedPosition != null)
                {
                    lastSelectedPosition.Show(GetGridVisualTypeMaterial(GridVisualType.White));
                }
                lastSelectedPosition = gridPositionVisuals[gridPosition.x, gridPosition.z, gridPosition.floor];
                lastSelectedPosition.Show(GetGridVisualTypeMaterial(GridVisualType.Green));
            }
        }

        private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
        {
            Material gridVisualMaterial = gridVisualTypeMaterials.Where(g => g.gridVisualType == gridVisualType).Select(g => g.material).FirstOrDefault();
            if (gridVisualMaterial == null)
            {
                Debug.LogError($"Could not find GridVisualTypeMaterial for GridVisualType {gridVisualType}");
            }
            return gridVisualMaterial;
        }
    }
}
