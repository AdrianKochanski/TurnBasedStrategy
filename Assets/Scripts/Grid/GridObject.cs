using Game.Units;
using System.Collections.Generic;
using System.Linq;

namespace Game.Grid
{
    public class GridObject
    {
        private GridSystem gridSystem;
        private GridPosition gridPosition;
        private HashSet<Unit> units;

        public GridObject(GridSystem gridSystem, GridPosition gridPosition)
        {
            this.gridSystem = gridSystem;
            this.gridPosition = gridPosition;
            this.units = new HashSet<Unit>();
        }

        public override string ToString()
        {
            if (!units.Any()) return gridPosition.ToString();
            return $"{gridPosition.ToString()}\n{units.Select(u => u.ToString()).Aggregate((a, b) => $"{a}\n{b}")}";
        }

        public IEnumerable<Unit> GetUnitList()
        {
            return units;
        }

        public bool TryGetUnit(out Unit unit)
        {
            unit = units.FirstOrDefault();
            if (unit == null) return false;
            return true;
        }

        public void AddUnit(Unit unit)
        {
            units.Add(unit);
        }

        public void RemoveUnit(Unit unit)
        {
            units.Remove(unit);
        }

        public bool HasAnyUnit()
        {
            return units.Any();
        }
    }
}

