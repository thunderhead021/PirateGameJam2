using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameState GameState;
    private int curLevel = 0;
    private List<ScriptableLevel> levels = new();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            levels = Resources.LoadAll<ScriptableLevel>("Levels").ToList();
        }
        else
            Destroy(gameObject);
    }

    public void NextLevel() 
    {
        curLevel++;
        ChangeState(GameState.GenerateGrid);
    }

    public void NewGame() 
    {
        curLevel = 0;
        ChangeState(GameState.GenerateGrid);
    }

    // Start is called before the first frame update 
    void Start()
    {
        ChangeState(GameState.GenerateGrid);
    }

    public void EndPlayerTurn() 
    {
        ChangeState(GameState.EnemyTurn);
    }

    public void ChangeState(GameState state) 
    {
        GameState = state;
        switch (state) 
        {
            case GameState.GenerateGrid:
                GridManager.instance.ResetLevel();
                UnitManager.instance.ResetLevel();
                UIManager.instance.NewGame();
                GridManager.instance.GenerateGrid(levels[curLevel].percentageOfBlockingTileOutOf100, levels[curLevel].maxNumberOfSpecialTiles);
                break;
            case GameState.SpawnPlayerUnit:
                UnitManager.instance.SpawnPlayerUnits(levels[curLevel].numberOfPlayerUnits, levels[curLevel].playerUnitsPool);
                break;
            case GameState.SpawnEnemyUnits:
                UnitManager.instance.SpawnEnemyUnits(levels[curLevel].numberOfEnemies, levels[curLevel].enemyUnitsPool);
                break;
            case GameState.PlayerTurn:
                UIManager.instance.SwitchTurn();
                UnitManager.instance.ApplyStatus(false);
                UnitManager.instance.ResetMove();
                break;
            case GameState.EnemyTurn:
                UIManager.instance.SwitchTurn();
                UnitManager.instance.ApplyStatus(true);
                UnitManager.instance.EnemyTurn();
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
