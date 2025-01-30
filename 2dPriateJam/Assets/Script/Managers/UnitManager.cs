using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager instance;
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

    public void ResetLevel()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    public void SpawnPlayerUnits(int playerCountForLevel, List<BaseUnit> playerUnitsPool) 
    {
        for (int i = 0; i < playerCountForLevel; i++) 
        {
            var startTile = GridManager.instance.GetStartTile(true);
            BaseUnit unit = Instantiate(playerUnitsPool.OrderBy(o => Random.value).First(), transform);
            unit.unitSide = Side.Player;
            unit.gameObject.tag = "Player";
            unit.UpdateHp();
            startTile.SetUnit(unit);
            playerUnits.Add(unit);
        }
        GameManager.instance.ChangeState(GameState.SpawnEnemyUnits);
    }

    public void SpawnEnemyUnits(int enemyCountForLevel, List<BaseUnit> enemyUnitsPool)
    {
        for (int i = 0; i < enemyCountForLevel; i++) 
        {
            var startTile = GridManager.instance.GetStartTile(false);
            BaseUnit unit = Instantiate(enemyUnitsPool.OrderBy(o => Random.value).First(), transform);
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

    public void SetSelectUnit(BaseUnit unit = null) 
    { 
        if (SelectedUnit != null)
        {
            SelectedUnit.Select(false);
            GridManager.instance.SetTilesMoveable(SelectedUnit.curTile, SelectedUnit.moveRange, SelectedUnit.moveType, false);
            GridManager.instance.SetTilesAttackable(SelectedUnit.curTile, SelectedUnit.attackRange, SelectedUnit.attackType, false);
            UIManager.instance.DisplayInfo(SelectedUnit, false);
        }
        SelectedUnit = unit;
        if (unit != null)
        {
            unit.Select(true);
            UIManager.instance.DisplayInfo(unit, true);
        }
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
        StartCoroutine(EnemyTurnHelper());
    }

    IEnumerator EnemyTurnHelper() 
    {
        foreach (BaseUnit unit in enemyUnits)
        {
            unit.EnemyMove();
            yield return new WaitForSeconds(0.5f);
        }

        while (!IsAllEnemyMoved())
        {
            yield return null;
        }
        GameManager.instance.ChangeState(GameState.PlayerTurn);
    }

    private bool IsAllEnemyMoved() 
    {
        foreach (BaseUnit unit in enemyUnits)
        {
            if(!unit.hasMoved)
                return false;
        }
        return true;
    }

    public void ResetMove() 
    {
        foreach (BaseUnit checkUnit in playerUnits)
        {
            checkUnit.ResetMove();
        }
        foreach (BaseUnit checkUnit in enemyUnits)
        {
            checkUnit.ResetMove();
        }
    }

    public void ApplyStatus(bool forEnemy) 
    {
        var unitsList = forEnemy ? enemyUnits : playerUnits;
        for (int i = 0; i < unitsList.Count; i++) 
        {
            var unit = forEnemy ? enemyUnits[i] : playerUnits[i];
            unit.CheckStatus();
        }
    }

    public void RemoveUnit(BaseUnit unit) 
    {
        LevelState levelState = LevelState.Playing;
        bool changeSide = false;
        if (unit.unitSide == Side.Player) 
        {
            foreach (BaseUnit checkUnit in playerUnits) 
            {
                if (checkUnit == unit) 
                {
                    playerUnits.Remove(checkUnit);
                    break;
                }
            }
            if(playerUnits.Count == 0)
                levelState = LevelState.Lose;

            if (playerUnits.Count <= 1)
                SoundManager.instance.PlayCritBGM();
        }
        else 
        {
            foreach (BaseUnit checkUnit in enemyUnits)
            {
                if (checkUnit == unit)
                {
                    enemyUnits.Remove(checkUnit);
                    playerUnits.Add(checkUnit);
                    checkUnit.unitSide = Side.Player;
                    changeSide = true;
                    checkUnit.ReviveUnit();
                    break;
                }
            }
            if (enemyUnits.Count == 0)
                levelState = LevelState.Win;
        }

        switch (levelState) 
        {
            case LevelState.Playing:
                if (!changeSide)
                {
                    unit.RemoveGameObject();
                }
                break;
            case LevelState.Lose:
                UIManager.instance.GameOver();
                GameManager.instance.ChangeState(GameState.GameEnd);
                break;
            case LevelState.Win:
                UIManager.instance.NextLevel();
                GameManager.instance.ChangeState(GameState.GameEnd);
                break;
        }
        
    }

    public bool IsAutoEndTurn() 
    {
        foreach (BaseUnit unit in playerUnits) 
        {
            if(!unit.hasMoved)
                return false;
        }
        return true;
    }

    private T GetRandomUnit<T>() where T : BaseUnit 
    {
        return (T)units.OrderBy(o => Random.value).First().unitPrefab;
    }

}

public enum LevelState 
{
    Playing,
    Win,
    Lose
}