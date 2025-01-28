using System.Collections.Generic;
using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    [SerializeField]
    protected SpriteRenderer spriteRenderer;
    [SerializeField]
    private GameObject highlight;
    [SerializeField]
    private GameObject moveableTile;
    [SerializeField]
    private GameObject attackableTile;
    public bool isWalkable;
    private bool isSelected = false;
    [HideInInspector]
    public Vector2 pos;
    private TileEffect curEffect;
    [SerializeField]
    private List<TileEffect> effects;

    public BaseUnit curUnit;
    public bool WalkAble()
    { 
        return isWalkable && curUnit == null; 
    }

    public bool AttackAble()
    {
        return isWalkable && UnitManager.instance.SelectedUnit != null && curUnit != null && curUnit != UnitManager.instance.SelectedUnit && curUnit.unitSide != UnitManager.instance.SelectedUnit.unitSide;
    }

    public virtual void Init(int x, int y, bool haveEffect = false) 
    {
        pos = new Vector2(x, y);
        if (haveEffect)
            SetEffect();
    }

    public void UpdateTile(Sprite sprite) 
    {
        spriteRenderer.sprite = sprite;
    }

    public void SetSelectable(bool isSelect, bool forattack = false, bool forDisplay = false) 
    {
        if(!forDisplay)
            isSelected = isSelect;
        if (GameManager.instance.GameState != GameState.PlayerTurn)
        {
            moveableTile.SetActive(false);
            attackableTile.SetActive(false);
        }
        else
        {
            if(forattack)
                attackableTile.SetActive(isSelect); 
            else
                moveableTile.SetActive(isSelect);
        }
    }

    private void OnMouseEnter()
    {
        if (isWalkable) 
            highlight.SetActive(true);
    }

    private void OnMouseExit()
    {
        if (isWalkable)
            highlight.SetActive(false);
    }

    private void OnMouseDown()
    {
        if(GameManager.instance.GameState != GameState.PlayerTurn)
            return;

        if (curUnit != null)
        {
            //show info

            if (!curUnit.hasMoved || curUnit != UnitManager.instance.SelectedUnit) 
            {
                if (curUnit.unitSide == Side.Player)
                {
                    if (!curUnit.hasMoved) 
                    {
                        UIManager.instance.SetSelectUnit();
                        UnitManager.instance.SetSelectUnit(curUnit);
                        //show move range
                        GridManager.instance.SetTilesMoveable(this, curUnit.moveRange, curUnit.moveType, true);
                    }
                }
                else if (curUnit.unitSide == Side.Enemy )
                {
                    if (isSelected)
                    {
                        //do attack here
                        UnitManager.instance.SelectedUnit.DealDamage(curUnit, UnitManager.instance.SelectedUnit.AttackPower, UnitManager.instance.SelectedUnit.AttackEffect);
                        //disable attack range
                        GridManager.instance.SetTilesAttackable(UnitManager.instance.SelectedUnit.curTile, UnitManager.instance.SelectedUnit.attackRange, UnitManager.instance.SelectedUnit.attackType, false);
                    }
                    else 
                    {
                        UnitManager.instance.SetSelectUnit();
                        UIManager.instance.SetSelectUnit(curUnit);
                        GridManager.instance.SetTilesMoveable(this, curUnit.moveRange, curUnit.moveType, true, true);
                    }
                }
            }
        }
        else if (UnitManager.instance.SelectedUnit != null && isSelected) 
        {
            GridManager.instance.SetTilesMoveable(UnitManager.instance.SelectedUnit.curTile, UnitManager.instance.SelectedUnit.moveRange, UnitManager.instance.SelectedUnit.moveType, false);
            SetUnit(UnitManager.instance.SelectedUnit);
            UnitManager.instance.SelectedUnit.Move();
            GridManager.instance.SetTilesAttackable(UnitManager.instance.SelectedUnit.curTile, UnitManager.instance.SelectedUnit.attackRange, UnitManager.instance.SelectedUnit.attackType, true);
        }

        //check for end turn
        if (UnitManager.instance.IsAutoEndTurn())
            GameManager.instance.ChangeState(GameState.EnemyTurn);
    }

    private void Update()
    {
        if (GameManager.instance.GameState != GameState.PlayerTurn && isSelected)
            SetSelectable(false);
            
    }

    public void SetUnit(BaseUnit unit) 
    {
        if (unit.curTile != null)
        {
            if (unit.curTile.curEffect != null) 
            {
                unit.curTile.curEffect.RemoveEffect(unit);
            }
            unit.curTile.curUnit = null;
        }
        unit.transform.position = transform.position;
        curUnit = unit;
        unit.curTile = this;
        //apply effect
        if (curEffect != null) 
        {
            curEffect.ApplyEffect(unit);
        }
    }

    private void SetEffect() 
    {
        curEffect = effects[Random.Range(0, effects.Count - 1)];
        curEffect.gameObject.SetActive(true);
    }

}
