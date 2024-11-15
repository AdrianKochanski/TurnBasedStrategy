using Game.Core;
using Game.Grid;
using UnityEngine;

public class PathfindingUpdater : MonoBehaviour
{
    void Start()
    {
        DestructibleCrate.OnAnyDestroyed += DestructibleCrate_OnAnyDestroyed;
    }

    private void DestructibleCrate_OnAnyDestroyed(DestructibleCrate crate)
    {
        Pathfinding.Instance.SetIsWalkableGridPosition(crate.GetGridPosition(), true);
    }
}
