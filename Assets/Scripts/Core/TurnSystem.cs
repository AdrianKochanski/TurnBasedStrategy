using Game.Grid;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core
{
    public class TurnSystem : MonoBehaviour
    {
        public static TurnSystem Instance { get; private set; }

        private int turnNumber = 1;

        public event Action<int> onTurnChange;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError($"There's more than one TurnSystem! {transform} - {Instance}");
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void NextTurn()
        {
            turnNumber++;
            onTurnChange?.Invoke(turnNumber);
        }

        public int GetTurn()
        {
            return turnNumber;
        }
    }
}
