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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            if (!pausMenuShowing) 
            {
                pausMenu.SetActive(true);
                pausMenuShowing = true;
            }
            else
            {
                pausMenu.SetActive(false);
                pausMenuShowing = false;
            }
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
