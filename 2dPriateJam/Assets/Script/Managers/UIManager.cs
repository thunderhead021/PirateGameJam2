using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public RectTransform playerTurn;
    public RectTransform enemyTurn;
    public GameObject gameOverScreen;
    public InfoDisplay infoDisplay;
    private BaseUnit SelectedUnit;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void DisplayInfo(BaseUnit unit, bool active) 
    {
        infoDisplay.gameObject.SetActive(active);
        if(active)
            infoDisplay.Setup(unit.TurnIcon, unit.AttackPower);
    }

    public void SetSelectUnit(BaseUnit unit = null)
    {
        if (SelectedUnit != null)
        {
            SelectedUnit.Select(false);
            GridManager.instance.SetTilesMoveable(SelectedUnit.curTile, SelectedUnit.moveRange, SelectedUnit.moveType, false, true);
            DisplayInfo(SelectedUnit, false);
        }
        SelectedUnit = unit;
        if (unit != null)
        {
            unit.Select(true);
            DisplayInfo(unit, true);
        }
    }

    public void GameOver()
    {
        gameOverScreen.SetActive(true);
    }
    public void NewGame() 
    {
        gameOverScreen.SetActive(false);
    }

    public void SwitchTurn() 
    {
        if (GameManager.instance.GameState == GameState.PlayerTurn)
        {
            enemyTurn.localScale = new(0.5f, 0.5f);
            playerTurn.localScale = Vector3.one;
        }
        else if (GameManager.instance.GameState == GameState.EnemyTurn) 
        {
            playerTurn.localScale = new(0.5f, 0.5f);
            enemyTurn.localScale = Vector3.one;
        }
    }

}
