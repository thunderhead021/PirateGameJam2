using System.Collections.Generic;
using UnityEngine;

public class TurnsDisplay : MonoBehaviour
{
    public List<TurnIndicator> turnIndicators;

    public void Setup(List<BaseUnit> entities)
    {
        int entitiesIndex = 0;
        for (int i = 0; i < turnIndicators.Count; i++)
        {
            turnIndicators[i].Setup(entities[entitiesIndex]);
            if (entitiesIndex == entities.Count - 1)
                entitiesIndex = 0;
            else
                entitiesIndex++;
        }
        NextTurn();
    }

    private void NextTurn() 
    {
        if (transform.GetChild(0).TryGetComponent<TurnIndicator>(out var nextTurn)) 
        {
            bool isPlayer = nextTurn.Entity.isCurControl;
            GameManager.instance.ChangeState(isPlayer ? GameState.PlayerTurn : GameState.EnemyTurn);
            if (!isPlayer) 
            {
                nextTurn.Entity.EnemyMove();
            }
        }
    }

    public void ChangeTurn() 
    {
        if (turnIndicators.Count > 0)
        {
            Transform lastTurn = transform.GetChild(0);
            lastTurn.SetAsLastSibling();
            NextTurn();
        }
    }

}
