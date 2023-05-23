using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HudManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] Image pausMenu;

    bool pausMenuShowing;

    private void Awake()
    {
        pausMenuShowing = false;
        pausMenu.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            if (!pausMenu.enabled) 
            {
                pausMenu.enabled = true;
            }
            else if (pausMenu)
            {
                pausMenu.enabled = false;
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
