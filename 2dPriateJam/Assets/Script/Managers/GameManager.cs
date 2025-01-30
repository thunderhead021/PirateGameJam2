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
        if(curLevel >= levels.Count)
            UIManager.instance.Victory();
        else
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
        ChangeState(GameState.GameStart);
    }

    public void EndPlayerTurn() 
    {
        ChangeState(GameState.EnemyTurn);
    }

    public void StartGame() 
    {
        ChangeState(GameState.GenerateGrid);
    }

    IEnumerator GenerateGridPhase()
    {
        GridManager.instance.ResetLevel();
        UnitManager.instance.ResetLevel();
        UIManager.instance.NewGame();
        GridManager.instance.GenerateGrid(levels[curLevel].percentageOfBlockingTileOutOf100, levels[curLevel].maxNumberOfSpecialTiles);
        SoundManager.instance.PlayBattleBGM();
        while(UIManager.instance.IsButtonStillPlaySound())
            yield return null;
        UIManager.instance.mainMenuScreen.SetActive(false);
        UIManager.instance.gameScreen.SetActive(true);
    }

    public void ChangeState(GameState state) 
    {
        GameState = state;
        switch (state) 
        {
            case GameState.GameStart:
                UIManager.instance.mainMenuScreen.SetActive(true);
                UIManager.instance.gameScreen.SetActive(false);
                break;
            case GameState.GenerateGrid:
                StartCoroutine(GenerateGridPhase());
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
                UnitManager.instance.SetSelectUnit();
                UIManager.instance.SwitchTurn();
                UnitManager.instance.ApplyStatus(true);
                UnitManager.instance.EnemyTurn();
                break;
            case GameState.GameEnd:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
}

public enum GameState 
{
    GameStart,
    GenerateGrid,
    SpawnPlayerUnit,
    SpawnEnemyUnits,
    PlayerTurn,
    EnemyTurn,
    GameEnd,
}
