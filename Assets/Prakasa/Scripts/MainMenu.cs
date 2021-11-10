using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    private InputHandler inputHandler;

    public void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
    }

    public void Update()
    {
        if (inputHandler.isOpening)
        {
            PlayGame();
        }

        if (inputHandler.isAttackedPressed)
        {
            QuitGame();
        }

    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
