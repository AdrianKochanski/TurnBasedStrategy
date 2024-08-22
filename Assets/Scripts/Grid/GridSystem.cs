using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Grid
{
    public class GridSystem
    {
        private int width;
        private int height;
        private float cellSize;
        private GridObject[,] gridObjectMap;

        public GridSystem(int width, int height, float cellSize)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            gridObjectMap = new GridObject[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    gridObjectMap[x, z] = new GridObject(this, new GridPosition(x, z));
                }
            }
        }

        public Vector3 GetWorldPositon(GridPosition gridPosition)
        {
            return new Vector3(gridPosition.x, 0, gridPosition.z) * cellSize;
        }

        public GridPosition GetGridPosition(Vector3 worldPosition)
        {
            return new GridPosition(
                Mathf.RoundToInt(worldPosition.x / cellSize),
                Mathf.RoundToInt(worldPosition.z / cellSize)
            );
        }

        public void CreateDebugObjects(Transform debugPrefab, Transform? parent)
        {
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    GridPosition gridPosition = new GridPosition(x, z);
                    Transform gridObjectInstance = GameObject.Instantiate(debugPrefab, GetWorldPositon(gridPosition), Quaternion.identity, parent);
                    
                    if(gridObjectInstance.TryGetComponent(out GridDebugObject gridDebugObject) && TryGetGridObject(gridPosition, out GridObject gridObject))
                    {
                        gridDebugObject.SetGridObject(gridObject);
                    }
                }
            }
        }

        public bool TryGetGridObject(GridPosition gridPosition, out GridObject gridObject)
        {
            gridObject = null;

            if (gridPosition.x < 0 || gridPosition.x >= gridObjectMap.GetLength(0) ||
                gridPosition.z < 0 || gridPosition.z >= gridObjectMap.GetLength(1))
            {
                return false;
            }

            gridObject = gridObjectMap[gridPosition.x, gridPosition.z];
            return true;
        }

        public bool IsValidGridPosition(GridPosition gridPosition)
        {
            return gridPosition.x >= 0 
                && gridPosition.z >= 0 
                && gridPosition.x < width
                && gridPosition.z < height;
        }

        public bool IsGridBorder(GridPosition gridPosition)
        {
            return gridPosition.x == 0
                || gridPosition.z == 0
                || gridPosition.x == width - 1
                || gridPosition.z == height - 1;
        }

        public int GetWidth()
        {
            return width;
        }

        public int GetHeight()
        {
            return height;
        }
    }
}