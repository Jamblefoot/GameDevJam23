using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class HudManager : MonoBehaviour
{
    public static HudManager singleton;

    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] UnityEngine.UI.Slider playerHealthBar;
    [SerializeField] GameObject pausMenu;

    bool pausMenuShowing;

    private void Awake()
    {
        if (HudManager.singleton != null && HudManager.singleton != this) 
        { 
            DestroyImmediate(this); 
        }
        else
        {
            HudManager.singleton = this;
        }

        pausMenuShowing = false;
        pausMenu.SetActive(pausMenuShowing);
    }
    public void UpdateScoreText(int score)
    {
        scoreText.text = $"Score: {score}";
    }
    public void UpdatePlayerHealth(int playerHealth)
    {
        playerHealthBar.value = playerHealth;
    }
    //public void UpdateShipHealth(int shipHealth)
    //{
    //    shipHealthBar.fillAmount = shipHealth / 100;
    //}
    public void PausMenuTogle()
    {
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
    public void QuitGame()
    {
        Application.Quit();
    }
}
