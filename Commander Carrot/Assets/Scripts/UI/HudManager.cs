using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HudManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] GameObject pausMenu;

    bool pausMenuShowing;

    private void Awake()
    {
        pausMenuShowing = false;
        pausMenu.SetActive(pausMenuShowing);
    }
    void OnPause(InputValue value)
    {
        print("escape pressed");
        if (pausMenuShowing == false)
        {
            pausMenu.SetActive(true);
            pausMenuShowing = true;
            Time.timeScale = 0f;
        }
        else if (pausMenuShowing == true)
        {
            pausMenu.SetActive(false);
            pausMenuShowing = false;
            Time.timeScale = 1f;
        }
    }
    public void UpdateScoreText(int score)
    {
        scoreText.text = $"Score: {score}";
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
