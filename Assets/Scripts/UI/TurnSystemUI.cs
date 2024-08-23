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
        [SerializeField] TextMeshProUGUI turnNumberText;

        private void Awake()
        {
            TurnSystem.Instance.onTurnChange += UpdateTurnText;
        }

        private void Start()
        {
            turnButton.onClick.AddListener(() => TurnSystem.Instance.NextTurn());
        }

        public void UpdateTurnText(int turnNumber)
        {
            turnNumberText.text = $"TURN: {turnNumber}";
        }

    }
}
