using Game.Grid;
using UnityEngine;

public class PathFindingLinkUpdater : MonoBehaviour
{
    [SerializeField] public Vector3 linkPositionA;
    [SerializeField] public Vector3 linkPositionB;

    public bool TryGetPathfindingLink(out PathfindingLink link)
    {
        link = null;
        if(LevelGrid.Instance.TryGetGridPosition(linkPositionA, out GridPosition linkA) 
            && LevelGrid.Instance.TryGetGridPosition(linkPositionB, out GridPosition linkB))
        {
            link = new PathfindingLink()
            {
                gridPositionA = linkA,
                gridPositionB = linkB
            };
            return true;
        }
        return false;
    }
}
