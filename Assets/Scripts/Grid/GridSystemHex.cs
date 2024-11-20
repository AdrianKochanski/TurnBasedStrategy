using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Grid
{
    public class GridSystemHex<TGridObject>
    {
        private int width;
        private int height;
        private float cellSize;
        private TGridObject[,] gridObjectMap;
        private const float HEX_VERTICAL_OFFSET_MULTIPLIER = 0.125f;
        private const float HEX_HORIZONTAL_ODD_OFFSET = 0.5f;


        public GridSystemHex(int width, int height, float cellSize, Func<GridSystemHex<TGridObject>, GridPosition, TGridObject> createGridObject)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            gridObjectMap = new TGridObject[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    gridObjectMap[x, z] = createGridObject(this, new GridPosition(x, z));
                }
            }
        }

        public float GetZHexWorldPosition(GridPosition gridPosition)
        {
            return gridPosition.z * (1 - (HEX_VERTICAL_OFFSET_MULTIPLIER * cellSize));
        }

        public float GetXHexWorldPosition(int x, int z)
        {
            return z % 2 == 0 ? x : x + HEX_HORIZONTAL_ODD_OFFSET;
        }

        public Vector3 GetWorldPositon(GridPosition p)
        {
            return new Vector3(GetXHexWorldPosition(p.x, p.z), 0, GetZHexWorldPosition(p)) * cellSize;
        }

        public int GetZHexGridPosition(Vector3 worldPosition)
        {
            return Mathf.RoundToInt(worldPosition.z / (1 - (HEX_VERTICAL_OFFSET_MULTIPLIER * cellSize)));
        }

        public int GetXHexGridPosition(float x, float z)
        {
            return Mathf.RoundToInt(z % 2 == 0 ? x : x - HEX_HORIZONTAL_ODD_OFFSET);
        }

        public GridPosition GetGridPosition(Vector3 worldPosition)
        {
            Vector3 p = worldPosition / cellSize;
            int zHexGrid = GetZHexGridPosition(p);
            return new GridPosition(GetXHexGridPosition(p.x, zHexGrid), zHexGrid);
        }

        public void CreateDebugObjects(Transform debugPrefab, Transform? parent)
        {
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    GridPosition gridPosition = new GridPosition(x, z);
                    Transform gridObjectInstance = GameObject.Instantiate(debugPrefab, GetWorldPositon(gridPosition), Quaternion.identity, parent);
                    
                    if(gridObjectInstance.TryGetComponent(out GridDebugObject gridDebugObject) && TryGetGridObject(gridPosition, out TGridObject gridObject))
                    {
                        gridDebugObject.SetGridObject(gridObject);
                    }
                }
            }
        }

        public bool TryGetGridObject(GridPosition gridPosition, out TGridObject gridObject)
        {
            gridObject = default;

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

        public bool RaycastHorizontal(GridPosition from, GridPosition to, LayerMask layerMask, float? height = 1.7f)
        {
            Vector3 fromV = GetWorldPositon(from);
            Vector3 toV = GetWorldPositon(to);
            Vector3 direction = toV - fromV;
            return Physics.Raycast(
                fromV + Vector3.up * height.Value,
                direction.normalized,
                direction.magnitude,
                layerMask
            );
        }

        public bool RaycastVertical(GridPosition from, LayerMask layerMask, float? height = 1f, float checkPointOffset = 0.4f)
        {
            float verticalOffset = height.Value / 5;
            float horizontalOffset = cellSize * checkPointOffset;
            Vector3 fromV = GetWorldPositon(from);
            Vector3 origin = fromV + Vector3.down * verticalOffset;

            Vector3 rightOrigin = origin + Vector3.right * horizontalOffset;
            Vector3 leftOrigin = origin - Vector3.right * horizontalOffset;
            Vector3 forwardOrigin = origin + Vector3.forward * horizontalOffset;
            Vector3 backwardOrigin = origin - Vector3.forward * horizontalOffset;

            List<Vector3> positions = new List<Vector3>() { origin, rightOrigin, leftOrigin, forwardOrigin, backwardOrigin };

            foreach (Vector3 originPositon in positions)
            {
                bool result = Physics.Raycast(
                    originPositon,
                    Vector3.up,
                    height.Value + verticalOffset,
                    layerMask
                );
                if (result)
                {
                    return true;
                }
            }
            return false;
        }

        public float Distance(GridPosition from, GridPosition to)
        {
            Vector3 fromWorldPosition = GetWorldPositon(from);
            Vector3 toWorldPosition = GetWorldPositon(to);
            float distance = Vector3.Distance(fromWorldPosition, toWorldPosition) / cellSize;
            return distance;
        }
    }
}