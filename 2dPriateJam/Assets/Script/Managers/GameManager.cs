using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameState GameState;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        ChangeState(GameState.GenerateGrid);
    }

    public void ChangeState(GameState state) 
    {
        GameState = state;
        switch (state) 
        {
            case GameState.GenerateGrid:
                GridManager.instance.GenerateGrid();
                break;
            case GameState.SpawnPlayerUnit:
                UnitManager.instance.SpawnPlayerUnits();
                break;
            case GameState.SpawnEnemyUnits:
                UnitManager.instance.SpawnEnemyUnits();
                break;
            case GameState.PlayerTurn:
                break;
            case GameState.EnemyTurn:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
}

public enum GameState 
{
    GenerateGrid,
    SpawnPlayerUnit,
    SpawnEnemyUnits,
    PlayerTurn,
    EnemyTurn,
}
