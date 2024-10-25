using Game.Actions;
using Game.Core;
using Game.Grid;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Game.Actions.BaseAction;

namespace Game.Units
{
    public class EnemyAI : MonoBehaviour
    {
        private enum State
        {
            WaitingForEnemyTurn,
            TakingTurn,
            Busy
        };

        private float timer;
        private State state;

        private void Awake()
        {
            state = State.WaitingForEnemyTurn;
        }

        private void Start()
        {
            TurnSystem.Instance.onPlayerChange += TurnSystem_OnPlayerChange;
            //Unit.onAnyActionBegin += SetStateTakingTurn;
            BaseAction.OnAnyActionComplete += Unit_OnAnyActionComplete;
        }

        private void Update()
        {
            switch(state)
            {
                case State.WaitingForEnemyTurn:
                    break;
                case State.TakingTurn:
                    timer -= Time.deltaTime;
                    if (timer < 0)
                    {
                        state = State.Busy;
                        if(!TryTakeEnemyAIAction())
                        {
                            state = State.WaitingForEnemyTurn;
                            TurnSystem.Instance.NextTurn();
                        }
                    }
                    break;
                case State.Busy:
                    break;
            }
        }

        private bool TryTakeEnemyAIAction()
        {
            foreach(Unit enemyUnit in UnitManager.Instance.GetEnemyUnitList())
            {
                if(TryTakeEnemyAIAction(enemyUnit))
                {
                    return true;
                }
            }
            return false;
        }

        private bool TryTakeEnemyAIAction(Unit enemyUnit)
        {
            EnemyAIAction bestEnemyAIAction = null;

            foreach (BaseAction enemyAction in enemyUnit.GetBaseActions())
            {
                EnemyAIAction enemyAIAction = enemyAction.GetBestEnemyAIAction();
                if (enemyAIAction == null)
                {
                    continue;
                }

                if(bestEnemyAIAction == null || enemyAIAction.actionValue > bestEnemyAIAction.actionValue)
                {
                    bestEnemyAIAction = enemyAIAction;
                }
            }

            if (bestEnemyAIAction != null && bestEnemyAIAction.action.TryStartAction(new List<GridPosition> { bestEnemyAIAction.gridPosition }))
            {
                return true;
            }

            return false;
        }

        private void Unit_OnAnyActionComplete(BaseAction action)
        {
            if(!TurnSystem.Instance.IsPlayerTurn())
            {
                timer = 0.5f;
                state = State.TakingTurn;
            }
        }

        private void TurnSystem_OnPlayerChange(bool isPlayerTurn)
        {
            if (!isPlayerTurn)
            {
                timer = 2f;
                state = State.TakingTurn;
            }
        }
    }
}
