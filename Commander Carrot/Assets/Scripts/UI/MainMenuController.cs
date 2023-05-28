using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    //[SerializeField] GameObject settingsCanvas;

    public void StartNewGame() { SceneManager.LoadScene(1); }
    public void QuitGame() { Application.Quit(); }

    //  Settings menu canvas buttons
    //public void OpenSettings() { settingsCanvas.SetActive(true); }
    //public void CloseSettings() { settingsCanvas.SetActive(false); }
    public void OpenSettings() { SettingsControl.singleton.settingsCanvas.gameObject.SetActive(true); }
    public void CloseSettings() { SettingsControl.singleton.settingsCanvas.gameObject.SetActive(false); }
}
