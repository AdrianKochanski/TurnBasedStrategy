using Game.Interactions;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Grid
{
    public class Pathfinding : MonoBehaviour
    {
        public static Pathfinding Instance { get; private set; }
        [SerializeField] private Transform gridObjectPrefab;
        [SerializeField] private LayerMask obstaclesLayer;
        [SerializeField] private LayerMask mousePlaneLayerMask;
        [Range(0, 0.5f)]
        [SerializeField] private float obstaclesCheckOffset = 0.4f;
        [Range(0, 5f)]
        [SerializeField] private float obstaclesCheckHeight = 1f;
        //[SerializeField] private Transform pathFindingLinkContainer;

        private int width = 10;
        private int height = 10;
        private float cellSize = 2f;
        private int floorAmount;
        private const int MOVE_COST = 10;
        private List<GridSystemHex<PathNode>> gridSystems = new List<GridSystemHex<PathNode>>();
        //private List<PathfindingLink> pathFindingLinkList = new List<PathfindingLink>();

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

        public void Setup(int width, int height, float cellSize, int floorAmount)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            this.floorAmount = floorAmount;

            //foreach(Transform pathfindingLinkTransform in pathFindingLinkContainer)
            //{
            //    if(pathfindingLinkTransform.TryGetComponent(out PathFindingLinkUpdater linkUpdater)
            //        && linkUpdater.TryGetPathfindingLink(out PathfindingLink link))
            //    {
            //        pathFindingLinkList.Add(link);
            //    }
            //}

            for (int floor = 0; floor < floorAmount; floor++)
            {
                gridSystems.Add(new GridSystemHex<PathNode>(width, height, cellSize, floor, LevelGrid.FLOOR_HEIGHT, (gS, gP) => new PathNode(gP)));
                //if (TryGetGridSystem(floor, out GridSystemHex<PathNode> gridSystem))
                //{
                //    gridSystem.CreateDebugObjects(gridObjectPrefab, transform);
                //}
            }

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    for (int floor = 0; floor < floorAmount; floor++)
                    {
                        GridPosition gridPosition = new GridPosition(x, z, floor);
                        if(TryGetGridSystem(floor, out GridSystemHex<PathNode> gridSystem))
                        {
                            if (!gridSystem.RaycastVertical(gridPosition, mousePlaneLayerMask, -obstaclesCheckHeight))
                            {
                                GetNode(x, z, floor).SetWalkable(false);
                            }
                            else if (gridSystem.RaycastVertical(gridPosition, obstaclesLayer, obstaclesCheckHeight, obstaclesCheckOffset))
                            {
                                GetNode(x, z, floor).SetWalkable(false);
                            }
                        }
                    }
                }
            }
        }

        private bool TryGetGridSystem(int floor, out GridSystemHex<PathNode> gridSystem)
        {
            if (floor < 0 || floor >= floorAmount)
            {
                gridSystem = null;
                return false;
            }
            gridSystem = gridSystems[floor];
            return true;
        }

        public IEnumerable<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, int? range, out int pathLength)
        {
            PriorityQueue<PathNode> openList = new();
            List<PathNode> closedList = new List<PathNode>();

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    for (int floor = 0; floor < floorAmount; floor++)
                    {
                        if (TryGetGridSystem(floor, out GridSystemHex<PathNode> gridSystem) && gridSystem.TryGetGridObject(new GridPosition(x, z, floor), out PathNode pathNode))
                        {
                            pathNode.SetGCost(int.MaxValue);
                            pathNode.SetHCost(0);
                            pathNode.CalculateFCost();
                            pathNode.ResetCameFromPathNode();
                        }
                    }
                }
            }

            if (TryGetGridSystem(startGridPosition.floor, out GridSystemHex<PathNode> startGridSystem) && startGridSystem.TryGetGridObject(startGridPosition, out PathNode startNode)
                && TryGetGridSystem(endGridPosition.floor, out GridSystemHex<PathNode> endGridSystem) && endGridSystem.TryGetGridObject(endGridPosition, out PathNode endNode))
            {
                openList.Enqueue(startNode);
                startNode.SetGCost(0);
                startNode.SetHCost(Distance(startGridPosition, endGridPosition));
                startNode.CalculateFCost();

                while (openList.Count > 0)
                {
                    PathNode currentNode = openList.Dequeue();

                    if (currentNode == endNode)
                    {
                        pathLength = endNode.GetFCost();
                        return CalculatePath(endNode);
                    }

                    closedList.Add(currentNode);
                    var neighbours = GetNeighbourLinkedList(currentNode.GetGridPosition());

                    foreach (PathNode neighbourNode in neighbours)
                    {
                        if (closedList.Contains(neighbourNode)) continue;
                        int tentativeGCost = currentNode.GetGCost() + Distance(currentNode.GetGridPosition(), neighbourNode.GetGridPosition());

                        if (tentativeGCost < neighbourNode.GetGCost())
                        {
                            neighbourNode.SetGCost(tentativeGCost);
                            neighbourNode.SetCameFromPathNode(currentNode);
                            neighbourNode.SetHCost(Distance(neighbourNode.GetGridPosition(), endGridPosition));
                            neighbourNode.CalculateFCost();

                            if (!openList.Contains(neighbourNode))
                            {
                                if (range == null || (range != null) && (tentativeGCost <= range * MOVE_COST))
                                {
                                    openList.Enqueue(neighbourNode);
                                }
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
            return Mathf.RoundToInt(LevelGrid.Instance.Distance(startGridPosition, endGridPosition) * MOVE_COST);
        }

        public void SetIsWalkableGridPosition(GridPosition gridPosition, bool isWalkable)
        {
            if(TryGetGridSystem(gridPosition.floor, out GridSystemHex<PathNode> gridSystem) && gridSystem.TryGetGridObject(gridPosition, out PathNode pathNode))
            {
                pathNode.SetWalkable(isWalkable); 
            }
        }

        public bool IsWalkableGridPosition(GridPosition gridPosition)
        {
            return TryGetGridSystem(gridPosition.floor, out GridSystemHex<PathNode> gridSystem) && gridSystem.TryGetGridObject(gridPosition, out PathNode pathNode) && pathNode.IsWalkable();
        }

        public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition, int range, out int pathLength)
        {
            return FindPath(startGridPosition, endGridPosition, range, out pathLength).Count() > 0;
        }

        public IEnumerable<PathNode> GetNeighbourList(GridPosition gridPosition)
        {
            return (new List<PathNode>()
            {
                GetNode(gridPosition.x - 1, gridPosition.z, gridPosition.floor),
                GetNode(gridPosition.x + 1, gridPosition.z, gridPosition.floor),
                GetNode(gridPosition.x, gridPosition.z - 1, gridPosition.floor),
                GetNode(gridPosition.x, gridPosition.z + 1, gridPosition.floor),
                GetNode(gridPosition.z % 2 == 0 ? gridPosition.x - 1 : gridPosition.x + 1, gridPosition.z + 1, gridPosition.floor),
                GetNode(gridPosition.z % 2 == 0 ? gridPosition.x - 1 : gridPosition.x + 1, gridPosition.z - 1, gridPosition.floor),
            }).NotNull().Where(n => n.IsWalkable());
        }

        private IEnumerable<PathNode> GetNeighbourLinkedList(GridPosition gridPosition)
        {
            List<PathNode> neighbourList = GetNeighbourList(gridPosition).ToList();
            var linkNodes = GetConntectedPathNodes(gridPosition);
            if (linkNodes.Count() > 0)
            {
                neighbourList.AddRange(linkNodes);
            }

            return neighbourList;
        }

        private IEnumerable<PathNode> GetConntectedPathNodes(GridPosition gridPosition)
        {
            if (LevelGrid.Instance.TryGetInteractableAtGrid(gridPosition, out IInteractable interactable)
                && interactable is Lift lift && lift.TryGetConnectedLinks(gridPosition, out IEnumerable<GridPosition> connectedGrids))
            {
                return connectedGrids.Select(g => GetNode(g.x, g.z, g.floor)).NotNull().Where(n => n.IsWalkable());
            }

            return Enumerable.Empty<PathNode>();
        }

        private PathNode GetNode(int x, int z, int floor)
        {
            if(TryGetGridSystem(floor, out GridSystemHex<PathNode> gridSystem) && gridSystem.TryGetGridObject(new GridPosition(x, z, floor), out PathNode pathNode))
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
