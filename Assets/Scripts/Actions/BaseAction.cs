using System;
using UnityEngine;
using Game.Units;
using Game.Grid;
using System.Collections.Generic;
using System.Linq;

namespace Game.Actions
{
    public abstract class BaseAction : MonoBehaviour
    {
        protected Unit unit;
        private bool isActive;

        public event Action onActionBegin;
        public event Action onActionComplete;

        protected virtual void Awake()
        {
            unit = GetComponent<Unit>();
        }

        protected void Update() 
        { 
            if (!isActive) return;

            if(UpdateAction())
            {
                isActive = false;
                onActionComplete?.Invoke();
            }
        }

        public abstract bool UpdateAction();

        public class BaseActionParameters { 
            internal GridPosition gridPosition;
        }

        public virtual void StartAction(BaseActionParameters args)
        {
            onActionBegin?.Invoke();
            isActive = true;
        }

        public virtual void StartAction()
        {
            StartAction(null);
        }

        public abstract string GetActionName();

        public bool IsActionSetup()
        {
            return onActionBegin != null && onActionComplete != null;
        }

        public virtual bool IsValidActionGridPositon(GridPosition gridPosition)
        {
            IEnumerable<GridPosition> validGridPositions = GetValidActionGridPositions();
            return validGridPositions.Contains(gridPosition);
        }

        public virtual IEnumerable<GridPosition> GetValidActionGridPositions()
        {
            List<GridPosition> validGridPositionList = new List<GridPosition>();

            if(LevelGrid.Instance.IsValidGridPosition(unit.GetGridPosition()))
            {
                validGridPositionList.Add(unit.GetGridPosition());
            }

            return validGridPositionList;
        }
    }
}