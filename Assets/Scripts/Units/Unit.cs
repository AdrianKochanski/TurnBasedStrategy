using Game.Actions;
using Game.Grid;
using System;
using System.Collections.Generic;
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

        private void Awake()
        {
            baseActions = GetComponents<BaseAction>();
        }

        private void Start()
        {
            gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
        }

        private void Update()
        {

            GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            if (newGridPosition != gridPosition)
            {
                LevelGrid.Instance.UnitMovedGridPosition(this, gridPosition, newGridPosition);
                gridPosition = newGridPosition;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Unit other = (Unit)obj;
            return GetInstanceID() == other.GetInstanceID();
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(GetInstanceID());
        }

        public GridPosition GetGridPosition()
        {
            return gridPosition;
        }

        public IEnumerable<BaseAction> GetBaseActions()
        {
            return baseActions;
        }

        public bool IsEnemy()
        {
            return isEnemy; 
        }

        public void Damage()
        {
            Debug.Log(transform + " damaged!");
        }
    }
}
