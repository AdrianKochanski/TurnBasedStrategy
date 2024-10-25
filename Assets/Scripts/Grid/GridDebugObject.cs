using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.Grid
{
    public class GridDebugObject : MonoBehaviour
    {
        [SerializeField] TextMeshPro textMeshProUGUI;

        protected object gridObject;

        public virtual void SetGridObject(object gridObject)
        {
            this.gridObject = gridObject;
        }

        protected virtual void Update()
        {
            textMeshProUGUI.text = gridObject.ToString();
        }
    }
}
