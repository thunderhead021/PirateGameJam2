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
    [HideInInspector]
    public BaseUnit playerUnit;

    private List<ScriptableUnit> units;
    public List<BaseUnit> order = new();

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
        player.gameObject.tag = "Player";
        player.UpdateHp();
        startTile.SetUnit(player);
        playerUnit = player;
        order.Add(player);
        GameManager.instance.ChangeState(GameState.SpawnEnemyUnits);
    }

    public void SpawnEnemyUnits()
    {
        for (int i = 0; i < enemyCountForLevel; i++) 
        {
            var startTile = GridManager.instance.GetStartTile(false);
            var unit = Instantiate(GetRandomUnit<BaseUnit>());
            unit.isCurControl = false;
            unit.gameObject.tag = "Enemy";
            unit.UpdateHp();
            startTile.SetUnit(unit);
            order.Add(unit);
        }
        order.Sort((a, b) => b.speed.CompareTo(a.speed));
        UIManager.instance.turnsDisplay.Setup(order);
    }

    public void SpawnUnit(bool isEnemy) 
    {
        var startTile = GridManager.instance.GetRandomTile();
        var unit = Instantiate(GetRandomUnit<BaseUnit>());
        unit.isCurControl = false;
        if (isEnemy)
            unit.gameObject.tag = "Enemy";
        else
        {
            unit.isCurControl = true;
        }
        unit.UpdateHp();
        startTile.SetUnit(unit);
        order.Add(unit);
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
