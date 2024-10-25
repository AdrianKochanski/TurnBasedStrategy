using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Units
{
    public class UnitManager : MonoBehaviour
    {
        private List<Unit> unitList = new List<Unit>();
        private List<Unit> friendlyUnitList = new List<Unit>();
        private List<Unit> enemyUnitList = new List<Unit>();
        public static UnitManager Instance { get; private set; }

        private void Start()
        {
            if (Instance != null)
            {
                Debug.LogError($"There's more than one UnitManager! {transform} - {Instance}");
                Destroy(gameObject);
                return;
            }
            Instance = this;

            Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;
            Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
        }

        private void Unit_OnAnyUnitSpawned(Unit unit)
        {
            unitList.Add(unit);
            if (unit.IsEnemy())
            {
                enemyUnitList.Add(unit);
            }
            else
            {
                friendlyUnitList.Add(unit);
            }
        }

        private void Unit_OnAnyUnitDead(Unit unit)
        {
            unitList.Remove(unit);
            if (unit.IsEnemy())
            {
                enemyUnitList.Remove(unit);
            }
            else
            {
                friendlyUnitList.Remove(unit);
            }
        }

        public IEnumerable<Unit> GetUnitList() { return unitList; }
        public IEnumerable<Unit> GetFirendlyUnitList() { return friendlyUnitList; }
        public IEnumerable<Unit> GetEnemyUnitList() { return enemyUnitList; }
    }
}