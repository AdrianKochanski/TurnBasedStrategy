using Game.Core;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class TurnSystemUI : MonoBehaviour
    {
        [SerializeField] Button turnButton;
        [SerializeField] TextMeshProUGUI turnButtonText;
        [SerializeField] TextMeshProUGUI turnNumberText;

        private void Start()
        {
            turnButton.onClick.AddListener(() => TurnSystem.Instance.NextTurn());
            TurnSystem.Instance.onTurnChange += TurnSystem_OnTurnChange;
            TurnSystem.Instance.onPlayerChange += TurnSystem_OnPlayerChange;
        }

        private void TurnSystem_OnTurnChange(int turnNumber)
        {
            turnNumberText.text = $"TURN: {turnNumber}";
        }

        private void TurnSystem_OnPlayerChange(bool isPlayer)
        {
            turnButton.enabled = isPlayer;
            turnButtonText.text = isPlayer ? "END TURN" : "ENEMY TURN";
        }
    }
}
