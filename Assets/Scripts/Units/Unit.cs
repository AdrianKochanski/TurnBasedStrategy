using Game.Actions;
using Game.Core;
using Game.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Units
{
    public class Unit : MonoBehaviour
    {
        [SerializeField] private float rotateSpeed = 10f;
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float stoppingDistance = .1f;
        [SerializeField] private bool isEnemy = false;

        private GridPosition gridPosition;
        private BaseAction[] baseActions;
        private HealthSystem healthSystem;

        public static event Action<Unit> OnAnyUnitSpawned;
        public static event Action<Unit> OnAnyUnitDead;

        private void Awake()
        {
            baseActions = GetComponents<BaseAction>();
            healthSystem = GetComponent<HealthSystem>();
        }

        private void Start()
        {
            gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
            healthSystem.OnDead += HealthSystem_OnDead;
            OnAnyUnitSpawned?.Invoke(this);
        }

        private void Update()
        {

            GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            if (newGridPosition != gridPosition)
            {
                GridPosition oldGridPosition = gridPosition;
                gridPosition = newGridPosition;
                LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newGridPosition);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Unit other = (Unit)obj;
            return GetInstanceID() == other.GetInstanceID();
        }

        private void HealthSystem_OnDead()
        {
            LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
            Destroy(gameObject);
            OnAnyUnitDead?.Invoke(this);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(GetInstanceID());
        }

        public GridPosition GetGridPosition()
        {
            return gridPosition;
        }

        public T GetAction<T>() where T : BaseAction
        {
            return baseActions.OfType<T>().FirstOrDefault();
        }

        public Vector3 GetWorldPositon() => LevelGrid.Instance.GetWorldPositon(gridPosition);

        public IEnumerable<BaseAction> GetBaseActions()
        {
            return baseActions;
        }

        public int GetActionPoints()
        {
            int points = 0;

            foreach (BaseAction action in baseActions) 
            { 
                if(action.GetPossibleActionsCount() > 0) { points++; };
            }

            return points;
        }

        public bool IsEnemy()
        {
            return isEnemy; 
        }

        public void Damage(int damageAmount) => healthSystem.Damage(damageAmount);
        public bool IsDead() => healthSystem.IsDead();
        public float GetHealthNormalized() => healthSystem.GetHealthNormalized();
    }
}
