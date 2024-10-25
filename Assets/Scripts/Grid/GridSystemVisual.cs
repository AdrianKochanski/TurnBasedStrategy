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

        private GridPositionVisual[,] gridPositionVisuals;


        private void Start()
        {
            InitalizeGridVisual();
            UnitActionSystem.Instance.OnSelectedActionChange += UnitActionSystem_OnSelectedActionChange;
            TurnSystem.Instance.OnTurnChange += TurnSystem_OnTurnChange;
            BaseAction.OnAnyActionGridUpdate += BaseAction_OnAnyActionGridUpdate;
            UpdateActionGrid();
        }

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
                    gridPositionVisuals[gridPosition.x, gridPosition.z].Show(GetGridVisualTypeMaterial(gridVisualType));
                }
            }
        }

        private void InitalizeGridVisual()
        {
            int width = LevelGrid.Instance.GetWidth();
            int height = LevelGrid.Instance.GetHeight();

            gridPositionVisuals = new GridPositionVisual[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    GridPosition gridPosition = new GridPosition(x, z);
                    GridPositionVisual gridPositionVisualInstance = Instantiate(
                        GridPositionVisualPrefab,
                        LevelGrid.Instance.GetWorldPositon(gridPosition),
                        Quaternion.identity,
                        transform
                    );

                    gridPositionVisuals[x, z] = gridPositionVisualInstance;
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
