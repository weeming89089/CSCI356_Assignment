using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public GameObject gameLoseUI;
    public GameObject gameWinUI;
    bool gameIsOver;

    void Start()
    {
        GuardController.OnGuardHasSpottedPlayer += showGameLoseUI;
        FindObjectOfType<PlayerController>().OnReachEndOfLevel += showGameWinUI;
    }

    void Update()
    {
        if (gameIsOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    void showGameWinUI()
    {
        OnGameOver(gameWinUI);
    }

    void showGameLoseUI()
    {
        OnGameOver(gameLoseUI);
    }

    void OnGameOver(GameObject gameOverUI)
    {
        gameOverUI.SetActive(true);
        gameIsOver = true;
        GuardController.OnGuardHasSpottedPlayer -= showGameLoseUI;
        FindObjectOfType<PlayerController>().OnReachEndOfLevel -= showGameWinUI;
    }
}
