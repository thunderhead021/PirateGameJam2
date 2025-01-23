using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager instance;
    public int enemyCountForLevel = 3;
    public ScriptableUnit playerStartUnit;
    public BaseUnit SelectedUnit;

    private List<ScriptableUnit> units;
    private List<BaseUnit> playerUnits = new();
    private List<BaseUnit> enemyUnits = new();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            units = Resources.LoadAll<ScriptableUnit>("Units").ToList();
        }
        else
            Destroy(gameObject);
    }

    public void SpawnPlayerUnits() 
    {
        var startTile = GridManager.instance.GetStartTile(true);
        var player = Instantiate(playerStartUnit.unitPrefab, transform);
        player.unitSide = Side.Player;
        player.gameObject.tag = "Player";
        player.UpdateHp();
        startTile.SetUnit(player);
        playerUnits.Add(player);
        GameManager.instance.ChangeState(GameState.SpawnEnemyUnits);
    }

    public void SpawnEnemyUnits()
    {
        for (int i = 0; i < enemyCountForLevel; i++) 
        {
            var startTile = GridManager.instance.GetStartTile(false);
            var unit = Instantiate(GetRandomUnit<BaseUnit>(), transform);
            unit.unitSide = Side.Enemy;
            unit.gameObject.tag = "Enemy";
            unit.UpdateHp();
            startTile.SetUnit(unit);
            enemyUnits.Add(unit);
        }
        GameManager.instance.ChangeState(GameState.PlayerTurn);
    }

    public void SpawnUnit(bool isEnemy) 
    {
        var startTile = GridManager.instance.GetRandomTile();
        var unit = Instantiate(GetRandomUnit<BaseUnit>(), transform);    
        if (isEnemy)
        {
            unit.unitSide = Side.Enemy;
            unit.gameObject.tag = "Enemy";
            enemyUnits.Add(unit);
        }
        else
        {
            unit.unitSide = Side.Player;
            unit.gameObject.tag = "Player";
            playerUnits.Add(unit);
        }
        unit.UpdateHp();
        startTile.SetUnit(unit);
    }

    public void SetSelectUnit(BaseUnit unit) 
    {
        if(SelectedUnit != null)
            GridManager.instance.SetTilesMoveable(SelectedUnit.curTile, SelectedUnit.moveRange, SelectedUnit.moveType, false);
        SelectedUnit = unit;
    }

    public BaseUnit GetRandomNotAllyUnit(Side side) 
    {
        if (side == Side.Player)
            return enemyUnits.OrderBy(o => Random.value).First();
        else
            return playerUnits.OrderBy(o => Random.value).First();
    }

    public void EnemyTurn() 
    {
        foreach (BaseUnit unit in enemyUnits) 
        {
            unit.EnemyMove();
        }
        GameManager.instance.ChangeState(GameState.PlayerTurn);
    }

    public void ResetMove() 
    {

    }

    private T GetRandomUnit<T>() where T : BaseUnit 
    {
        return (T)units.OrderBy(o => Random.value).First().unitPrefab;
    }

}
