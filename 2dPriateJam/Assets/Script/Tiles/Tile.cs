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
    [SerializeField]
    private bool isWalkable;
    private bool isSelected = false;
    [HideInInspector]
    public Vector2 pos;

    public BaseUnit curUnit;
    public bool WalkAble()
    { 
        return isWalkable && curUnit == null; 
    }

    public bool AttackAble()
    {
        return isWalkable && curUnit != null && curUnit != UnitManager.instance.SelectedUnit;
    }

    public virtual void Init(int x, int y) 
    {
        pos = new Vector2(x, y);
    }

    public void SetSelectable(bool isSelect, bool forattack = false) 
    {
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
            if (curUnit.isCurControl)
            {
                UnitManager.instance.SetSelectUnit(curUnit);
                //show move range
                GridManager.instance.SetTilesMoveable(this, curUnit.moveRange, curUnit.moveType, true);
            }
            else
            {
                //do attack here
                UnitManager.instance.SelectedUnit.DealDamage(curUnit, UnitManager.instance.SelectedUnit.AttackPower);
                //disable attack range
                GridManager.instance.SetTilesAttackable(UnitManager.instance.SelectedUnit.curTile, UnitManager.instance.SelectedUnit.attackRange, UnitManager.instance.SelectedUnit.attackType, false);
                //end turn
                UIManager.instance.turnsDisplay.ChangeTurn();
            }
        }
        else if (UnitManager.instance.SelectedUnit != null && isSelected) 
        {
            GridManager.instance.SetTilesMoveable(UnitManager.instance.SelectedUnit.curTile, UnitManager.instance.SelectedUnit.moveRange, UnitManager.instance.SelectedUnit.moveType, false);
            SetUnit(UnitManager.instance.SelectedUnit);
            //show attack range
            GridManager.instance.SetTilesAttackable(UnitManager.instance.SelectedUnit.curTile, UnitManager.instance.SelectedUnit.attackRange, UnitManager.instance.SelectedUnit.attackType, true);
        }
    }

    private void Update()
    {
        if (GameManager.instance.GameState != GameState.PlayerTurn && isSelected)
            SetSelectable(false);
            
    }

    public void SetUnit(BaseUnit unit) 
    {
        if(unit.curTile != null)
            unit.curTile.curUnit = null;
        unit.transform.position = transform.position;
        curUnit = unit;
        unit.curTile = this;
    }

}
