using Game.Actions;
using Game.Units;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Grid
{
    public class GridSystemVisual : MonoBehaviour
    {
        [SerializeField] GridPositionVisual GridPositionVisualPrefab;

        private GridPositionVisual[,] gridPositionVisuals;

        private void Start()
        {
            InitalizeGridVisual();
        }

        private void Update()
        {
            UpdateGridVisual();
        }

        public void HideAllGridPosition()
        {
            foreach (var gridPosition in gridPositionVisuals) gridPosition.Hide();
        }

        public void ShowGridPositionList(IEnumerable<GridPosition> gridPositionList)
        {
            foreach (GridPosition gridPosition in gridPositionList) 
            {
                if(LevelGrid.Instance.IsValidGridPosition(gridPosition))
                {
                    gridPositionVisuals[gridPosition.x, gridPosition.z].Show();
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

        private void UpdateGridVisual()
        {
            HideAllGridPosition();
            BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
            ShowGridPositionList(selectedAction.GetValidActionGridPositions());
        }
    }
}
