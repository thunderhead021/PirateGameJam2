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
        var player = Instantiate(playerStartUnit.unitPrefab);
        player.isCurControl = true;
        startTile.SetUnit(player);
        GameManager.instance.ChangeState(GameState.SpawnEnemyUnits);
    }

    public void SpawnEnemyUnits()
    {
        for (int i = 0; i < enemyCountForLevel; i++) 
        {
            var startTile = GridManager.instance.GetStartTile(false);
            var unit = Instantiate(GetRandomUnit<BaseUnit>());
            unit.isCurControl = false;
            startTile.SetUnit(unit);
        }
        GameManager.instance.ChangeState(GameState.PlayerTurn);
    }

    public void SetSelectUnit(BaseUnit unit) 
    {
        SelectedUnit = unit;
    }

    private T GetRandomUnit<T>() where T : BaseUnit 
    {
        return (T)units.OrderBy(o => Random.value).First().unitPrefab;
    }

}
