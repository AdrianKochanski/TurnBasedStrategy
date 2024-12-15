using Game.Grid;
using UnityEngine;

namespace Game.Core
{
    public abstract class Damageable : MonoBehaviour
    {
        private GridPosition gridPosition;
        public abstract void Damage(int damageAmount, Vector3 source);

        private void Start()
        {
            LevelGrid.Instance.TryGetGridPosition(transform.position, out gridPosition);
        }

        public GridPosition GetGridPosition()
        {
            return gridPosition;
        }
    }
}
