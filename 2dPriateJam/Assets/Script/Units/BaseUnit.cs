using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public Tile curTile;
    public Sprite TurnIcon;
    public Side unitSide = Side.None;
    public int Hp = 10;
    public int AttackPower = 2;
    public int moveRange;
    public int attackRange;
    public MoveType moveType;
    public AttackType attackType;
    public int speed;
    public EnemyBehaviour enemyBehaviour;
    public AttackBehaviour attackBehaviour;
    public TextMeshPro curHp;
    public bool hasMoved { get; private set; } = false;


    public void EnemyMove() 
    {
        List<Tile> allMoveable = GridManager.instance.GetAllMoveableTiles(curTile, moveRange, moveType);
        switch (enemyBehaviour) 
        {
            case EnemyBehaviour.Chaser:
                ChaserMove(allMoveable);
                break;
            case EnemyBehaviour.Groupe:
                GroupeMove(allMoveable);
                break;
            case EnemyBehaviour.Coward:
                CowardMove(allMoveable);
                break;
            case EnemyBehaviour.Random:
                allMoveable[Random.Range(0, allMoveable.Count - 1)].SetUnit(this);
                break;
        }
        //enemy attack here
        List<Tile> allAttackable = GridManager.instance.GetAllAttackableTiles(curTile, attackRange, attackType);
        if (allAttackable.Count > 0) 
        {
            switch (attackBehaviour) 
            {
                case AttackBehaviour.Focus:
                    FocusAttack(allAttackable);
                    break;
                case AttackBehaviour.Spread:
                    SpreadAttack(allAttackable);
                    break;
                case AttackBehaviour.Random:
                    DealDamage(allAttackable[Random.Range(0, allAttackable.Count - 1)].curUnit, AttackPower);
                    break;
            }
        }
    }

    public void DealDamage(BaseUnit target, int amount) 
    {
        target.Hp -= amount;
        if (target.Hp <= 0)
        {
            target.Death();
        }
        else 
        {
            target.UpdateHp();
        }
    }

    public void UpdateHp() 
    {
        curHp.text = Hp.ToString();
    }

    public void Death() 
    {
        Destroy(gameObject);
    }

    public void Move() 
    {
        if (!hasMoved)
        {
            hasMoved = true;
        }
    }

    public void ResetMove()
    {
        hasMoved = false;
    }

    private void ChaserMove(List<Tile> tileList) 
    {
        Tile nearestTile = null;
        float shortestDistance = float.MaxValue;
        foreach (Tile tile in tileList)
        {
            float distance = Vector2.Distance(UnitManager.instance.GetRandomNotAllyUnit(unitSide).curTile.pos, tile.pos);

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestTile = tile;
            }
        }

        if (nearestTile != null) 
        {
            nearestTile.SetUnit(this);
        }
    }

    private void CowardMove(List<Tile> tileList) 
    {
        Tile farestTile = null;
        float longestDistance = float.MinValue;
        foreach (Tile tile in tileList)
        {
            float distance = Vector2.Distance(UnitManager.instance.GetRandomNotAllyUnit(unitSide).curTile.pos, tile.pos);

            if (distance >= longestDistance)
            {
                longestDistance = distance;
                farestTile = tile;
            }
        }

        if (farestTile != null)
        {
            farestTile.SetUnit(this);
        }
    }

    private void GroupeMove(List<Tile> tileList) 
    {
        GameObject[] units = GameObject.FindGameObjectsWithTag("Enemy");
        if (units.Length != 0) 
        {
            Tile nearestTile = null;
            float shortestDistance = float.MaxValue;
            foreach (Tile tile in tileList)
            {
                foreach (GameObject unit in units)
                {
                    // Calculate the Euclidean distance to the current unit
                    float distance = Vector2.Distance(UnitManager.instance.GetRandomNotAllyUnit(unitSide).curTile.pos, unit.GetComponent<BaseUnit>().curTile.pos);

                    // Update if this tile is closer to any unit
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        nearestTile = tile;
                    }
                }
            }
            if (nearestTile != null)
            {
                nearestTile.SetUnit(this);
            }
        } 
    }

    private void FocusAttack(List<Tile> tileList) 
    {
        BaseUnit target = null;
        int hp = int.MaxValue;
        foreach (Tile tile in tileList) 
        {
            if (tile.curUnit.Hp < hp) 
            {
                target = tile.curUnit;
                hp = tile.curUnit.Hp;
            }
        }

        if (target != null) 
        {
            DealDamage(target, AttackPower);
        }
    }

    private void SpreadAttack(List<Tile> tileList)
    {
        BaseUnit target = null;
        int hp = int.MinValue;
        foreach (Tile tile in tileList)
        {
            if (tile.curUnit.Hp > hp)
            {
                target = tile.curUnit;
                hp = tile.curUnit.Hp;
            }
        }

        if (target != null)
        {
            DealDamage(target, AttackPower);
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

public enum AttackBehaviour 
{
    Focus,
    Spread,
    Random
}

public enum Side 
{
    None,
    Player,
    Enemy
}