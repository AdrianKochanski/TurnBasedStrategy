using System;
using UnityEngine;
using Game.Units;
using Game.Grid;
using System.Collections.Generic;
using System.Linq;
using Game.Core;
using static Game.Grid.GridSystemVisual;

namespace Game.Actions
{
    public abstract class BaseAction : MonoBehaviour
    {
        [SerializeField] protected float points = 1;
        [SerializeField, Min(1f)] protected float costPointRate = 1;
        [SerializeField] protected float restorePointTurnRate = 1;
        [SerializeField] protected float maxPointLimit = 3;
        [SerializeField] protected GridVisualType targetVisualType = GridVisualType.White;
        [SerializeField] protected int range = 6;
        [SerializeField] protected GridVisualType rangeVisualType = GridVisualType.Yellow;
        [SerializeField] protected LayerMask obstaclesLayerMask;

        public enum UpdateActionResult
        {
            NextStep,
            Break,
            Continue
        }

        protected Unit unit;
        private bool isActive;
        protected List<(GridPosition, float)> targetPositions;
        private int currentTargetPositionIdx;
        private float pointsSnapshot = 0;

        public static event Action<BaseAction> OnAnyActionBegin;
        public static event Action<BaseAction> OnAnyActionGridUpdate;
        public static event Action<BaseAction> OnAnyActionComplete;
        public event Action onActionBegin;
        public event Action onActionComplete;
        public event Action OnRestorePoints;

        private Dictionary<GridVisualType, List<(GridPosition, float)>> actionGridPositions = new Dictionary<GridVisualType, List<(GridPosition, float)>>();

        protected virtual void Awake()
        {
            unit = GetComponent<Unit>();
        }

        protected virtual void Start()
        {
            TurnSystem.Instance.OnTurnChange += TurnSystem_OnTurnChange;
        }

        protected void Update() 
        { 
            if (!isActive) return;
            var updateResult = UpdateAction();

            switch(updateResult)
            {
                case UpdateActionResult.NextStep:
                    UpdateActionPoints();
                    currentTargetPositionIdx++;
                    UpdateActionGridPositions();

                    if (currentTargetPositionIdx >= targetPositions.Count)
                    {
                        OnActionComplete();
                    }
                    break;
                case UpdateActionResult.Break:
                        OnActionComplete();
                    break;
                case UpdateActionResult.Continue:
                    break;
            }
        }

        private void OnActionComplete()
        {
            isActive = false;
            onActionComplete?.Invoke();
            OnAnyActionComplete?.Invoke(this);
        }

        private void TurnSystem_OnTurnChange(int turnNumber)
        {
            points += restorePointTurnRate;
            points = MathF.Min(points, maxPointLimit);
            OnRestorePoints?.Invoke();
        }

        protected abstract UpdateActionResult UpdateAction();
        public abstract string GetActionName();
        public virtual bool TryStartAction(List<GridPosition> targetGridPositions)
        {
            targetPositions = FilterValidGridPositions(targetGridPositions).ToList();

            if (targetPositions.Count() <= 0) return false;
            currentTargetPositionIdx = 0;
            pointsSnapshot = points;
            onActionBegin?.Invoke();
            OnAnyActionBegin?.Invoke(this);
            isActive = true;
            return true;
        }

        protected virtual IEnumerable<(GridPosition, float)> FilterValidGridPositions(IEnumerable<GridPosition> targetPositions)
        {
            var validPositions = GetActionGridPositions(targetVisualType);

            return targetPositions.Where(p => validPositions.Any(t => t.Item1 == p)).Select(p => (
                p,
                validPositions.Where(t => t.Item1 == p).FirstOrDefault().Item2
            ));
        }

        public IEnumerable<(GridPosition, float)> GetActionGridPositions(GridVisualType visualType)
        {
            return actionGridPositions.ContainsKey(visualType) ? actionGridPositions[visualType] : Enumerable.Empty<(GridPosition, float)>();
        }

        public Dictionary<GridVisualType, List<(GridPosition, float)>> GetActionGridPositions()
        {
            return actionGridPositions;
        }

        public Dictionary<GridVisualType, List<(GridPosition, float)>> UpdateActionGridPositions()
        {
            UpdateActionGridPositions(unit.GetGridPosition());
            return actionGridPositions;
        }

        public void UpdateActionGridPositions(GridPosition unitPosition)
        {
            actionGridPositions = new Dictionary<GridVisualType, List<(GridPosition, float)>>() {
                { rangeVisualType, new List<(GridPosition, float)>() }
            };
            bool twoDimension = rangeVisualType != targetVisualType;

            if (twoDimension) {
                actionGridPositions.Add(targetVisualType, new List<(GridPosition, float)>());
            }

            for (int x = -range; x <= range; x++)
            {
                for (int z = -range; z <= range; z++)
                {
                    for (int floor = -range; floor <= range; floor++)
                    {
                        GridPosition offsetGridPosition = new GridPosition(x, z, floor);
                        GridPosition testGridPosition = unitPosition + offsetGridPosition;

                        float distance = LevelGrid.Instance.Distance(unitPosition, testGridPosition);
                        if (Mathf.RoundToInt(distance) > range) continue;
                        (bool validRange, bool validTarget) = IsValidGridPosition(testGridPosition, out float cost);

                        var actualCost = GetActionPointCost(cost);

                        if (!validRange || !CanSpendMaxActionPoints(actualCost)) continue;
                        if (twoDimension)
                        {
                            actionGridPositions[rangeVisualType].Add((testGridPosition, actualCost));
                        }

                        if (!validTarget || !CanSpendActionPoints(actualCost)) continue;
                        actionGridPositions[targetVisualType].Add((testGridPosition, actualCost));
                    }
                }
            }

            OnAnyActionGridUpdate?.Invoke(this);
        }

        public virtual (bool, bool) IsValidGridPosition(GridPosition targetPosition, out float cost)
        {
            cost = 1;
            bool isValid = LevelGrid.Instance.IsValidGridPosition(targetPosition);
            bool isWalkable = isValid && Pathfinding.Instance.IsValidGridPosition(targetPosition);
            return (isValid && isWalkable, isValid && isWalkable);
        }

        public int GetTargetGridPositonCount()
        {
            return GetActionGridPositions(targetVisualType).Count();
        }

        public int GetPossibleActionsCount()
        {
            return Mathf.FloorToInt(points / costPointRate);
        }

        public int GetPossibleActionsCountLimit()
        {
            return Mathf.FloorToInt(maxPointLimit / costPointRate);
        }

        public virtual float GetRestoreActionTurnRate()
        {
            return restorePointTurnRate / costPointRate;
        }

        protected void UpdateActionPoints()
        {
            points = pointsSnapshot - GetActionPointCost(CurrentTargetPosition());
        }

        public virtual float GetActionPointCost(float cost)
        {
            return costPointRate * cost;
        }

        protected bool CanSpendActionPoints(float cost)
        {
            return points >= cost;
        }

        protected bool CanSpendMaxActionPoints(float cost)
        {
            return maxPointLimit >= cost;
        }

        public Unit GetUnit()
        {
            return unit;
        }

        public abstract EnemyAIAction GetEnemyAIAction(GridPosition gridPosition);

        protected GridPosition CurrentTargetPosition()
        {
            return targetPositions[currentTargetPositionIdx].Item1;
        }

        protected bool TryGetCurrentTargetWorldPosition(out Vector3 worldPosition) => LevelGrid.Instance.TryGetWorldPositon(CurrentTargetPosition(), out worldPosition);

        protected float GetActionPointCost(GridPosition targetGridPosition)
        {
            var targetVisual = targetPositions.Where(p => p.Item1 == targetGridPosition).FirstOrDefault();
            return targetVisual.Item2;
        }

        public EnemyAIAction GetBestEnemyAIAction()
        {
            List<EnemyAIAction> enemyAIActions = new List<EnemyAIAction>();

            UpdateActionGridPositions();
            foreach (var gridPosition in GetActionGridPositions(targetVisualType))
            {
                EnemyAIAction enemyAIAction = GetEnemyAIAction(gridPosition.Item1);
                enemyAIActions.Add(enemyAIAction);
            }

            return enemyAIActions.OrderByDescending(a => a.actionValue).FirstOrDefault();
        }
    }
}