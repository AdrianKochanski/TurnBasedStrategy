using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.Grid
{
    public class GridDebugObject : MonoBehaviour
    {
        [SerializeField] TextMeshPro textMeshProUGUI;

        private GridObject gridObject;

        public void SetGridObject(GridObject gridObject)
        {
            this.gridObject = gridObject;
        }

        private void Update()
        {
            textMeshProUGUI.text = gridObject.ToString();
        }
    }
}
