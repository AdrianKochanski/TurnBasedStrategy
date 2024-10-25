using Game.Core;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.Grid
{
    public class PathfindingGridDebugObject : GridDebugObject
    {
        [SerializeField] private TextMeshPro gCostText;
        [SerializeField] private TextMeshPro hCostText;
        [SerializeField] private TextMeshPro fCostText;
        [SerializeField] private SpriteRenderer isWalkableSprite;

        protected override void Update()
        {
            base.Update();
            gCostText.text = (gridObject as PathNode).GetGCost().ToString();
            hCostText.text = (gridObject as PathNode).GetHCost().ToString();
            fCostText.text = (gridObject as PathNode).GetFCost().ToString();
            isWalkableSprite.color = (gridObject as PathNode).IsWalkable() ? Color.green : Color.red;
        }
    }
}
