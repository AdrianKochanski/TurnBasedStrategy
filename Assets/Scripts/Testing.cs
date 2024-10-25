using Game.Core;
using Game.Grid;
using Game.Units;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Testing : MonoBehaviour
{

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (MouseWorld.TryGetPosition(out Vector3 position))
            {
                GridPosition endGridPosition = LevelGrid.Instance.GetGridPosition(position);
                GridPosition startGridPosition = new GridPosition(0, 0);
                List<GridPosition> path = Pathfinding.Instance.FindPath(startGridPosition, endGridPosition, out int pathLength).ToList();

                for (int i = 0; i < path.Count() - 1; i++)
                {
                    Debug.DrawLine(
                        LevelGrid.Instance.GetWorldPositon(path[i]),// + new Vector3(0, 0.2f, 0),
                        LevelGrid.Instance.GetWorldPositon(path[i + 1]),// + new Vector3(0, 0.2f, 0),
                        Color.white,
                        10f
                    );
                }
            }
        }
    }
}
