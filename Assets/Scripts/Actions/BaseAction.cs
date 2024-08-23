using System;
using UnityEngine;
using Game.Units;
using Game.Grid;
using System.Collections.Generic;
using System.Linq;
using Game.Core;

namespace Game.Actions
{
    public abstract class BaseAction : MonoBehaviour
    {
        [SerializeField] protected float points = 1;
        [SerializeField, Min(1f)] protected float costPointRate = 1;
        [SerializeField] protected float restorePointTurnRate = 1;
        [SerializeField] protected float maxPointLimit = 3;

        protected Unit unit;
        private bool isActive;

        public event Action onActionBegin;
        public event Action onActionComplete;
        public event Action onRestorePoints;

        protected virtual void Awake()
        {
            unit = GetComponent<Unit>();
        }

        protected virtual void Start()
        {
            TurnSystem.Instance.onTurnChange += BaseAction_OnTurnChange;
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

        private void BaseAction_OnTurnChange(int turnNumber)
        {
            points += restorePointTurnRate;
            points = MathF.Min(points, maxPointLimit);
            onRestorePoints?.Invoke();
        }

        public abstract bool UpdateAction();

        public class BaseActionParameters { 
            internal GridPosition gridPosition;
        }

        public virtual bool StartAction(BaseActionParameters args)
        {
            if (!CanSpendActionPoints(args)) return false;
            
            onActionBegin?.Invoke();
            isActive = true;
            SpendActionPoints(args);
            return true;
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

        public virtual bool IsValidActionGridPositon(BaseActionParameters args)
        {
            IEnumerable<GridPosition> validGridPositions = GetValidActionGridPositions();
            return validGridPositions.Contains(args.gridPosition);
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

        public int GetPossibleActionsCount()
        {
            return Mathf.FloorToInt(points / costPointRate);
        }

        public bool CanSpendActionPoints(BaseActionParameters args)
        {
            return points >= GetActionPointCost(args);
        }

        public virtual float GetActionPointCost(BaseActionParameters args)
        {
            return costPointRate;
        }

        protected void SpendActionPoints(BaseActionParameters args)
        {
            points -= GetActionPointCost(args);
        }
    }
}