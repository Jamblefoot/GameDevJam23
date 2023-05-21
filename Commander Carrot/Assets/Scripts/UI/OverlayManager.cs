using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OverlayManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { print("Show Pause Menu"); }
    }
    public void UpdateScoreText(int score)
    {
        scoreText.text = $"Score: {score}";
    }
}
