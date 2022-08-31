using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public GameObject gameLoseUI;
    public GameObject gameWinUI;
    bool gameIsOver;
    bool win = false;
    bool lose = false;
    public static int guardsCount;
    public Text text;

    void Start()
    {
        Guard.OnGuardHasSpottedPlayer += showGameLoseUI;
        FindObjectOfType<PlayerController>().OnReachEndOfLevel += showGameWinUI;
        guardsCount = GameObject.FindGameObjectsWithTag("Guard").Length;
    }

    void Update()
    {
        text.text = "Guards Left: " + guardsCount.ToString();

        if (gameIsOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (lose)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

                }else if (win)
                {   
                    if (SceneManager.GetActiveScene().buildIndex + 1 > 2)
                    {
                        SceneManager.LoadScene(0);
                    }
                    else
                    {
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                    }
                }

            }
        }
    }

    void showGameWinUI()
    {
        OnGameOver(gameWinUI);
        win = true;
    }

    void showGameLoseUI()
    {
        OnGameOver(gameLoseUI);
        lose = true;
    }

    void OnGameOver(GameObject gameOverUI)
    {
        gameOverUI.SetActive(true);
        gameIsOver = true;
        Guard.OnGuardHasSpottedPlayer -= showGameLoseUI;
        FindObjectOfType<PlayerController>().OnReachEndOfLevel -= showGameWinUI;
    }
}
