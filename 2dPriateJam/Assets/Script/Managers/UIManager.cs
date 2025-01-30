using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public RectTransform playerTurn;
    public RectTransform enemyTurn;
    public GameObject mainMenuScreen;
    public GameObject gameScreen;
    public GameObject gameOverScreen;
    public GameObject nextLevelScreen;
    public GameObject victoryScreen;
    public InfoDisplay infoDisplay;
    private BaseUnit SelectedUnit;

    public Image Intro;
    public List<Sprite> intros;
    public List<AudioSource> buttonAudioSources;

    private int introIndex = 0;

    public void PlayIntro() 
    {
        if (intros.Count > 0)
        {
            Intro.sprite = intros[introIndex];
            introIndex++;
            if (introIndex > intros.Count)
            {
                introIndex = 0;
                GameManager.instance.ChangeState(GameState.GenerateGrid);
            }
        }
        else 
        {
            GameManager.instance.ChangeState(GameState.GenerateGrid);
        }
    }

    public bool IsButtonStillPlaySound() 
    {
        foreach (AudioSource source in buttonAudioSources) 
        {
            if(source.isPlaying)    
            { 
                return true; 
            }
        }
        return false;
    }

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
        if (active)
        {
            unit.ClickOnSoundPlay();
            infoDisplay.Setup(unit.TurnIcon, unit.AttackPower);
        }
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

    public void NextLevel() 
    {
        nextLevelScreen.SetActive(true);
    }

    public void Victory()
    {
        victoryScreen.SetActive(true);
    }

    public void NewGame() 
    {
        gameOverScreen.SetActive(false);
        nextLevelScreen.SetActive(false);
        victoryScreen.SetActive(false);
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
