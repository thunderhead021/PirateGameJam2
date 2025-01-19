using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class BaseUnit : MonoBehaviour
{
    public Tile curTile;
    public Sprite TurnIcon;
    public bool isCurControl = false;
    public int moveRange;
    public int attackRange;
    public MoveType moveType;
    public MoveType attackType;
    public int speed;
    public EnemyBehaviour enemyBehaviour;

    public void EnemyMove() 
    {
        List<Tile> allMoveable = GridManager.instance.GetAllMoveableTiles(curTile, moveRange, moveType);
        switch (enemyBehaviour) 
        {
            case EnemyBehaviour.Chaser:
                ChaserMove(allMoveable);
                break;
            case EnemyBehaviour.Groupe:
                break;
            case EnemyBehaviour.Coward:
                CowardMove(allMoveable);
                break;
            case EnemyBehaviour.Random:
                allMoveable[Random.Range(0, allMoveable.Count - 1)].SetUnit(this);
                UIManager.instance.turnsDisplay.ChangeTurn();
                break;
        }
    }

    private void ChaserMove(List<Tile> tileList) 
    {
        Tile nearestTile = null;
        float shortestDistance = float.MaxValue;
        foreach (Tile tile in tileList)
        {
            float distance = Vector2.Distance(UnitManager.instance.playerUnit.curTile.pos, tile.pos);

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestTile = tile;
            }
        }

        if (nearestTile != null) 
        {
            nearestTile.SetUnit(this);
            UIManager.instance.turnsDisplay.ChangeTurn();
        }
    }

    private void CowardMove(List<Tile> tileList) 
    {
        Tile nearestTile = null;
        float shortestDistance = float.MaxValue;
        foreach (Tile tile in tileList)
        {
            float distance = Vector2.Distance(UnitManager.instance.playerUnit.curTile.pos, tile.pos);

            if (distance >= shortestDistance)
            {
                shortestDistance = distance;
                nearestTile = tile;
            }
        }

        if (nearestTile != null)
        {
            nearestTile.SetUnit(this);
            UIManager.instance.turnsDisplay.ChangeTurn();
        }
    }
}

public enum EnemyBehaviour 
{
    Chaser,
    Coward,
    Groupe,
    Random
}
