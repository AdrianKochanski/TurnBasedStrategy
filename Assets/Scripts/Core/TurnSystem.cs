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
        private bool isPlayerTurn = true;
        public event Action<int> onTurnChange;
        public event Action<bool> onPlayerChange;

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
            // if last player
            if (!isPlayerTurn)
            {
                turnNumber++;
                onTurnChange?.Invoke(turnNumber);
            }

            // change Turn for the new player
            isPlayerTurn = !isPlayerTurn;
            onPlayerChange?.Invoke(isPlayerTurn);
        }

        public int GetTurn()
        {
            return turnNumber;
        }

        public bool IsPlayerTurn()
        {
            return isPlayerTurn;
        }
    }
}
