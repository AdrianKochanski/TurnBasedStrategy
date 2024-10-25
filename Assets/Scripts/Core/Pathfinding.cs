using Game.Grid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Core
{
    public class Pathfinding : MonoBehaviour
    {
        public static Pathfinding Instance { get; private set; }

        private int width = 10;
        private int height = 10;
        private float cellSize = 2f;
        private const int MOVE_COST = 10;
        private GridSystem<PathNode> gridSystem;
        [SerializeField] private Transform gridObjectPrefab;
        [SerializeField] private LayerMask obstaclesLayer;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError($"There's more than one Pathfinding! {transform} - {Instance}");
                Destroy(gameObject);
                return;
            }
            Instance = this;

        }

        public void Setup(int width, int height, float cellSize)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;

            gridSystem = new GridSystem<PathNode>(width, height, cellSize, (gS, gP) => new PathNode(gP));
            //gridSystem.CreateDebugObjects(gridObjectPrefab, transform);

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    GridPosition gridPosition = new GridPosition(x, z);
                    float raycastOffsetDistance = 5f;
                    if(gridSystem.RaycastVertical(gridPosition, obstaclesLayer, raycastOffsetDistance))
                    {
                        GetNode(x, z).SetWalkable(false);
                    }
                }
            }
        }

        public IEnumerable<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength)
        {
            List<PathNode> openList = new List<PathNode>();
            List<PathNode> closedList = new List<PathNode>();

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    if (gridSystem.TryGetGridObject(new GridPosition(x, z), out PathNode pathNode))
                    {
                        pathNode.SetGCost(int.MaxValue);
                        pathNode.SetHCost(0);
                        pathNode.CalculateFCost();
                        pathNode.ResetCameFromPathNode();
                    }
                }
            }

            if (gridSystem.TryGetGridObject(startGridPosition, out PathNode startNode)
                && gridSystem.TryGetGridObject(endGridPosition, out PathNode endNode))
            {
                openList.Add(startNode);
                startNode.SetGCost(0);
                startNode.SetHCost(Distance(startGridPosition, endGridPosition));
                startNode.CalculateFCost();


                while (openList.Count > 0)
                {
                    PathNode currentNode = openList.OrderBy(n => n.GetFCost()).First();

                    if (currentNode == endNode)
                    {
                        pathLength = endNode.GetFCost();
                        return CalculatePath(endNode);
                    }

                    openList.Remove(currentNode);
                    closedList.Add(currentNode);

                    foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
                    {
                        if (closedList.Contains(neighbourNode)
                            || !neighbourNode.IsWalkable()) continue;
                        int tentativeGCost = currentNode.GetGCost() + Distance(currentNode.GetGridPosition(), neighbourNode.GetGridPosition());

                        if (tentativeGCost < neighbourNode.GetGCost())
                        {
                            neighbourNode.SetGCost(tentativeGCost);
                            neighbourNode.SetCameFromPathNode(currentNode);
                            neighbourNode.SetHCost(Distance(neighbourNode.GetGridPosition(), endGridPosition));
                            neighbourNode.CalculateFCost();

                            if(!openList.Contains(neighbourNode))
                            {
                                openList.Add(neighbourNode);
                            }
                        }
                    }
                }
            }

            pathLength = 0;
            return Enumerable.Empty<GridPosition>();
        }

        private IEnumerable<GridPosition> CalculatePath(PathNode endNode)
        {
            List<PathNode> path = new List<PathNode>() { endNode };
            PathNode currentNode = endNode.GetCameFromPathNode();
            while(currentNode != null)
            {
                path.Add(currentNode);
                currentNode = currentNode.GetCameFromPathNode();
            }
            path.Reverse();

            return path.Select(n => n.GetGridPosition());
        }

        public int Distance(GridPosition startGridPosition, GridPosition endGridPosition)
        {
            return Mathf.RoundToInt(GridPosition.Distance(startGridPosition, endGridPosition) * MOVE_COST);
        }

        public bool IsWalkableGridPosition(GridPosition gridPosition)
        {
            return gridSystem.TryGetGridObject(gridPosition, out PathNode pathNode) && pathNode.IsWalkable();
        }

        public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength)
        {
            return FindPath(startGridPosition, endGridPosition, out pathLength).Count() > 0;
        }

        private IEnumerable<PathNode> GetNeighbourList(PathNode currentNode)
        {
            GridPosition gridPosition = currentNode.GetGridPosition();
            List<PathNode> neighbourList = new List<PathNode>()
            {
                GetNode(gridPosition.x - 1, gridPosition.z),
                GetNode(gridPosition.x - 1, gridPosition.z + 1),
                GetNode(gridPosition.x - 1, gridPosition.z - 1),
                GetNode(gridPosition.x, gridPosition.z + 1),
                GetNode(gridPosition.x, gridPosition.z - 1),
                GetNode(gridPosition.x + 1, gridPosition.z),
                GetNode(gridPosition.x + 1, gridPosition.z + 1),
                GetNode(gridPosition.x + 1, gridPosition.z - 1)
            };

            return neighbourList.NotNull();
        }

        private PathNode GetNode(int x, int z)
        {
            if(gridSystem.TryGetGridObject(new GridPosition(x, z), out PathNode pathNode))
            {
                return pathNode;
            }
            return null;
        }

        public int GetMoveCost()
        {
            return MOVE_COST;
        }
    }
}
