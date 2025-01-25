using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public RectTransform playerTurn;
    public RectTransform enemyTurn;
    public GameObject gameOverScreen;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
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
