namespace Game.Grid
{
    public class PathNode
    {
        private GridPosition _gridPosition;
        private int gCost;
        private int hCost;
        private int fCost;
        private PathNode cameFromPathNode;
        private bool isWalkable = true;

        public PathNode(GridPosition gridPosition)
        {
            _gridPosition = gridPosition;
        }

        public override string ToString()
        {
            return _gridPosition.ToString();
        }

        internal int GetGCost()
        {
            return gCost;
        }

        internal int GetHCost()
        {
            return hCost;
        }

        internal int GetFCost()
        {
            return fCost;
        }

        internal void SetGCost(int gCost)
        {
            this.gCost = gCost;
        }

        internal void SetHCost(int hCost)
        {
            this.hCost = hCost;
        }

        internal void CalculateFCost()
        {
            fCost = gCost + hCost;
        }

        internal void SetCameFromPathNode(PathNode pathNode)
        {
            cameFromPathNode = pathNode;
        }

        internal void ResetCameFromPathNode()
        {
            cameFromPathNode = null;
        }

        internal PathNode GetCameFromPathNode()
        {
            return cameFromPathNode;
        }

        internal GridPosition GetGridPosition()
        {
            return _gridPosition;
        }

        internal bool IsWalkable()
        {
            return isWalkable;
        }

        internal void SetWalkable(bool isWalkable)
        {
            this.isWalkable = isWalkable;
        }
    }
}
