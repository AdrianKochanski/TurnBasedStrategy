using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathFindingLinkUpdater))]
public class PathfindingLinkEditor : Editor
{
    private void OnSceneGUI()
    {
        PathFindingLinkUpdater pathFindingLinkUpdater = (PathFindingLinkUpdater)target;
        EditorGUI.BeginChangeCheck();
        Vector3 newLinkPositionA = Handles.PositionHandle(pathFindingLinkUpdater.linkPositionA, Quaternion.identity);
        Vector3 newLinkPositionB = Handles.PositionHandle(pathFindingLinkUpdater.linkPositionB, Quaternion.identity);
        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(pathFindingLinkUpdater, "Change Link Position");
            pathFindingLinkUpdater.linkPositionA = newLinkPositionA;
            pathFindingLinkUpdater .linkPositionB = newLinkPositionB;
        }
    }
}
