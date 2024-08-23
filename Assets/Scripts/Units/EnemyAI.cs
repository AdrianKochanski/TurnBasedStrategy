using Game.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Units
{
    public class EnemyAI : MonoBehaviour
    {
        private float timer;

        private void Start()
        {
            TurnSystem.Instance.onPlayerChange += (bool isPlayer) => {
                timer = 2f;
            };
        }

        private void Update()
        {
            if (TurnSystem.Instance.IsPlayerTurn()) return;

            timer -= Time.deltaTime;
            if (timer < 0 )
            {
                TurnSystem.Instance.NextTurn();
            }
        }
    }
}
