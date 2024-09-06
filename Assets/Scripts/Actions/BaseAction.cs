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
        protected Vector3 targetPosition;
        private BaseActionParameters args;

        public event Action onActionBegin;
        public event Action onActionComplete;
        public event Action onRestorePoints;

        protected virtual void Awake()
        {
            unit = GetComponent<Unit>();
        }

        protected virtual void Start()
        {
            TurnSystem.Instance.onTurnChange += TurnSystem_OnTurnChange;
        }

        protected void Update() 
        { 
            if (!isActive) return;

            if(UpdateAction(args))
            {
                isActive = false;
                onActionComplete?.Invoke();
            }
        }

        private void TurnSystem_OnTurnChange(int turnNumber)
        {
            points += restorePointTurnRate;
            points = MathF.Min(points, maxPointLimit);
            onRestorePoints?.Invoke();
        }

        public abstract bool UpdateAction(BaseActionParameters args);
        public abstract string GetActionName();
        public abstract IEnumerable<GridPosition> GetValidActionGridPositions();

        public class BaseActionParameters { 
            internal GridPosition targetGridPosition;
        }

        public virtual bool StartAction(BaseActionParameters args)
        {
            if (!CanSpendActionPoints(args)) return false;

            this.args = args;
            targetPosition = LevelGrid.Instance.GetWorldPositon(args.targetGridPosition);
            onActionBegin?.Invoke();
            isActive = true;
            SpendActionPoints(args);
            return true;
        }

        public virtual void StartAction()
        {
            StartAction(null);
        }

        public virtual bool IsValidActionGridPositon(BaseActionParameters args)
        {
            IEnumerable<GridPosition> validGridPositions = GetValidActionGridPositions();
            return validGridPositions.Contains(args.targetGridPosition);
        }

        public int GetPossibleActionsCount()
        {
            return Mathf.FloorToInt(points / costPointRate);
        }

        public int GetPossibleActionsCountLimit()
        {
            return Mathf.FloorToInt(maxPointLimit / costPointRate);
        }

        public bool CanSpendActionPoints(BaseActionParameters args)
        {
            return points >= GetActionPointCost(args);
        }

        public virtual float GetActionPointCost(BaseActionParameters args)
        {
            return costPointRate;
        }

        public virtual float GetRestoreActionTurnRate()
        {
            return restorePointTurnRate / costPointRate;
        }

        protected void SpendActionPoints(BaseActionParameters args)
        {
            points -= GetActionPointCost(args);
        }
    }
}