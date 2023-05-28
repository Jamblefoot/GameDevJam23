using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsControl : MonoBehaviour
{
    public static SettingsControl singleton;

    public Canvas settingsCanvas;

    [Header("Audio")]
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider effectsVolumeSlider;

    [Header("Graphics")]
    [SerializeField] TMP_Dropdown qualityDropdown;
    [SerializeField] GameObject customQuality;
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] TMP_Dropdown screenModeDropdown;
    [SerializeField] TMP_Dropdown aaDropdown;
    [SerializeField] TMP_Dropdown vsyncDropdown;
    [SerializeField] TMP_Dropdown anisotropicDropdown;
    [SerializeField] TMP_Dropdown shadowDropdown;
    [SerializeField] TMP_Dropdown shadowResDropdown;
    [SerializeField] Slider shadowDistanceSlider;
    [SerializeField] Slider lodBiasSlider;

    FullScreenMode[] screenModes;

    void Awake()
    {
        if (SettingsControl.singleton != null)
        {
            Destroy(gameObject);
            return;
        }
        else SettingsControl.singleton = this;

        DontDestroyOnLoad(gameObject);

        //FILL OUT RESOLUTION OPTIONS
        if (resolutionDropdown != null)
        {
            List<string> resOptions = new List<string>();
            foreach (Resolution r in Screen.resolutions)
            {
                resOptions.Add(r.ToString());
            }
            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(resOptions);
        }
        //FILL OUT SCREEN MODE DROPDOWN
        if (screenModeDropdown != null)
        {
            SetupScreenModes();
        }

        SetAudioSettings();
        SetDisplayOptions();
        GetPlayerPrefGraphics();
        SetQualityOptions();
    }

    //////////\\\\\\\\\
    //     AUDIO     \\
    //////////\\\\\\\\\

    void SetAudioSettings()
    {
        float playerPref = PlayerPrefs.GetFloat("MasterVolume", 0.8f);
        SetMasterVolume(playerPref);
        SetMasterVolumeSlider(playerPref);
        playerPref = PlayerPrefs.GetFloat("MusicVolume", 0.3f);
        SetMusicVolume(playerPref);
        SetMusicVolumeSlider(playerPref);
        playerPref = PlayerPrefs.GetFloat("EffectsVolume", 1f);
        SetEffectsVolume(playerPref);
        SetEffectsVolumeSlider(playerPref);
    }

    void SetMasterVolumeSlider(float setting)
    {
        if (masterVolumeSlider == null)
        {
            Debug.LogWarning("No Master Volume Slider found");
            return;
        }

        masterVolumeSlider.SetValueWithoutNotify(setting);
    }
    public void SetMasterVolume(float setting)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(setting) * 20);
    }

    void SetMusicVolumeSlider(float setting)
    {
        if (musicVolumeSlider == null)
        {
            Debug.LogWarning("No Music Volume Slider found");
            return;
        }

        musicVolumeSlider.SetValueWithoutNotify(setting);
    }
    public void SetMusicVolume(float setting)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(setting) * 20);
    }

    void SetEffectsVolumeSlider(float setting)
    {
        if (effectsVolumeSlider == null)
        {
            Debug.LogWarning("No Effects Volume Slider found");
            return;
        }

        effectsVolumeSlider.SetValueWithoutNotify(setting);
    }
    public void SetEffectsVolume(float setting)
    {
        audioMixer.SetFloat("EffectsVolume", Mathf.Log10(setting) * 20);
    }

    //////////\\\\\\\\\
    //   GRAPHICS    \\
    //////////\\\\\\\\\

    void GetPlayerPrefGraphics()
    {
        int prefInt = PlayerPrefs.GetInt("Quality", -1);
        if (prefInt >= 0)
        {
            if (prefInt == 6) //is custom
                SetCustomQuality();
            else
            {
                SetQualityLevel(prefInt);
                qualityDropdown.SetValueWithoutNotify(prefInt);
            }
        }

        prefInt = PlayerPrefs.GetInt("Resolution", -1);
        if (prefInt >= 0)
            SetResolution(prefInt);

        prefInt = PlayerPrefs.GetInt("ScreenMode", -1);
        if (prefInt >= 0)
            SetScreenMode(prefInt);

        prefInt = PlayerPrefs.GetInt("AntiAliasing", -1);
        if (prefInt >= 0)
            SetAntialiasing(prefInt);
        prefInt = PlayerPrefs.GetInt("VSync", -1);
        if (prefInt >= 0)
            SetVsync(prefInt);
        prefInt = PlayerPrefs.GetInt("Anisotropic", -1);
        if (prefInt >= 0)
            SetAnisotropicFiltering(prefInt);
        prefInt = PlayerPrefs.GetInt("Shadow", -1);
        if (prefInt >= 0)
            SetShadows(prefInt);
        prefInt = PlayerPrefs.GetInt("ShadowRes", -1);
        if (prefInt >= 0)
            SetShadowResolution(prefInt);
        float prefFloat = PlayerPrefs.GetFloat("ShadowDist", -1);
        if (prefFloat >= 0)
            SetShadowDistance(prefFloat);
        prefFloat = PlayerPrefs.GetFloat("LODBias", -1);
        if (prefFloat >= 0)
            SetLODBias(prefFloat);
    }

    void SetupScreenModes()
    {
        List<string> modes = new List<string>();
        switch (Application.platform)
        {
            case RuntimePlatform.OSXPlayer:
                Debug.Log("Setting possible screen modes for MacOS platform");
                screenModes = new FullScreenMode[] { FullScreenMode.FullScreenWindow, FullScreenMode.MaximizedWindow, FullScreenMode.Windowed };
                modes.InsertRange(0, new string[] { "Full Screen", "Maxed Window", "Windowed" });
                break;
            case RuntimePlatform.WindowsPlayer:
                Debug.Log("Setting possible screen modes for Windows platform");
                screenModes = new FullScreenMode[] { FullScreenMode.ExclusiveFullScreen, FullScreenMode.FullScreenWindow, FullScreenMode.Windowed };
                modes.InsertRange(0, new string[] { "Full Screen", "Borderless Window", "Windowed" });
                break;
            case RuntimePlatform.LinuxPlayer:
                Debug.Log("Setting possible screen modes for Linux platform");
                screenModes = new FullScreenMode[] { FullScreenMode.FullScreenWindow, FullScreenMode.Windowed };
                modes.InsertRange(0, new string[] { "Full Screen", "Windowed" });
                break;
            default:
                screenModes = new FullScreenMode[] { FullScreenMode.FullScreenWindow };
                modes.Add("Full Screen");
                break;
        }
        screenModeDropdown.ClearOptions();
        screenModeDropdown.AddOptions(modes);
        for (int i = 0; i < screenModes.Length; i++)
        {
            if (screenModes[i] == Screen.fullScreenMode)
            {
                screenModeDropdown.SetValueWithoutNotify(i);
                break;
            }
        }
    }
    public void SetScreenMode(int setting)
    {
        if (setting < 0 || setting >= screenModes.Length)
            return;

        Screen.fullScreenMode = screenModes[setting];
    }

    void SetDisplayOptions()
    {
        SetResolutionDropdown();
    }
    void SetQualityOptions()
    {
        //qualityDropdown.SetValueWithoutNotify(QualitySettings.GetQualityLevel());

        SetAntialiasingDropdown();
        SetVsyncDropdown();
        SetAnisotropicDropdown();
        SetShadowDropdown();
        SetShadowResolutionDropdown();
        SetShadowDistanceSlider();
        SetLODBiasSlider();
    }

    void SetResolutionDropdown()
    {
        if (resolutionDropdown == null) return;

        //TODO screen resolution initial setup doesn't seem to be working yet

        int index = 0;
        Debug.Log("Setting screen resolution dropdown. Current screen resolution is width: "
                    + Screen.width.ToString() + ", height: " + Screen.height.ToString()
                    + ", refresh rate: " + Screen.currentResolution.refreshRate.ToString());
        //DebugConsole.instance.Log("Setting screen resolution dropdown. Current screen resolution is width: "
        //            + Screen.width.ToString() + ", height: " + Screen.height.ToString()
        //            + ", refresh rate: " + Screen.currentResolution.refreshRate.ToString());
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            if (Screen.resolutions[i].width == Screen.width
                && Screen.resolutions[i].height == Screen.height
                && Screen.resolutions[i].refreshRate == Screen.currentResolution.refreshRate)
            {
                index = i;
                break;
            }
        }

        resolutionDropdown.value = index;
    }
    public void SetResolution(int newResIndex)
    {
        if (resolutionDropdown == null) return;

        Resolution res = Screen.resolutions[newResIndex];//resolutionDropdown.value];
        if (Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen)
        {
            res = Screen.currentResolution;
            //Screen.fullScreen = true;
            Screen.SetResolution(res.width, res.height, true, res.refreshRate);
        }
        else
        {
            //Screen.fullScreen = false;
            Screen.SetResolution(res.width, res.height, false);
        }

        //lastScreenMode = Screen.fullScreenMode;
    }

    public void SetQualityLevel(int level)
    {
        QualitySettings.SetQualityLevel(level, true);
        SetQualityOptions();
    }
    public void SetCustomQuality()
    {
        Debug.Log("QUALITY BEING SET TO CUSTOM!!!!!");

        if (customQuality == null)
        {
            Debug.LogWarning("There's no custom quality overlay thing");
            return;
        }

        customQuality.SetActive(true);

        qualityDropdown.options.Add(new TMP_Dropdown.OptionData("Custom"));
        qualityDropdown.SetValueWithoutNotify(qualityDropdown.options.Count - 1);
        qualityDropdown.options.RemoveAt(qualityDropdown.options.Count - 1);
        //List<Dropdown.OptionData> qualityOptions = new List<Dropdown.OptionData>(qualityDropdown.options);
        //qualityDropdown.ClearOptions();
        //qualityOptions.Add(new Dropdown.OptionData("Custom"));
        //qualityDropdown.AddOptions(qualityOptions);
        //qualityDropdown.SetValueWithoutNotify(qualityOptions.Count - 1);

        //SetSaveFlag((int)Changed.Quality);
    }

    void SetAntialiasingDropdown()
    {
        if (aaDropdown == null)
        {
            Debug.LogWarning("NO ANTIALIASING DROPDOWN FOUND");
            return;
        }

        switch (QualitySettings.antiAliasing)
        {
            case 0:
                aaDropdown.SetValueWithoutNotify(0);//value = 0;
                break;
            case 2:
                aaDropdown.SetValueWithoutNotify(1);//value = 1;
                break;
            case 4:
                aaDropdown.SetValueWithoutNotify(2);//value = 2;
                break;
            case 8:
                aaDropdown.SetValueWithoutNotify(3);//value = 3;
                break;
        }
    }
    public void SetAntialiasing(int setting)
    {
        if (setting == 0)
            QualitySettings.antiAliasing = 0;
        else QualitySettings.antiAliasing = Mathf.FloorToInt(Mathf.Pow(2, setting));
    }

    void SetVsyncDropdown()
    {
        if (vsyncDropdown == null)
        {
            Debug.LogWarning("NO VSYNC DROPDOWN FOUND");
            return;
        }

        vsyncDropdown.SetValueWithoutNotify(QualitySettings.vSyncCount);
    }
    public void SetVsync(int setting)
    {
        QualitySettings.vSyncCount = setting;
    }

    void SetAnisotropicDropdown()
    {
        if (anisotropicDropdown == null)
        {
            Debug.LogWarning("NO ANISOTROPIC DROPDOWN FOUND");
            return;
        }

        anisotropicDropdown.SetValueWithoutNotify((int)QualitySettings.anisotropicFiltering);
    }
    public void SetAnisotropicFiltering(int setting)
    {
        QualitySettings.anisotropicFiltering = (AnisotropicFiltering)setting;
    }

    void SetShadowDropdown()
    {
        if (shadowDropdown == null)
        {
            Debug.LogWarning("NO SHADOW DROPDOWN FOUND");
            return;
        }

        shadowDropdown.SetValueWithoutNotify((int)QualitySettings.shadows);
    }
    public void SetShadows(int setting)
    {
        QualitySettings.shadows = (ShadowQuality)setting;
    }

    void SetShadowResolutionDropdown()
    {
        if (shadowResDropdown == null)
        {
            Debug.LogWarning("NO SHADOW RESOLUTION DROPDOWN FOUND");
            return;
        }

        shadowResDropdown.SetValueWithoutNotify((int)QualitySettings.shadowResolution);
    }
    public void SetShadowResolution(int setting)
    {
        QualitySettings.shadowResolution = (ShadowResolution)setting;
    }

    void SetShadowDistanceSlider()
    {
        if (shadowDistanceSlider == null)
        {
            Debug.LogWarning("NO SHADOW DISTANCE SLIDER FOUND");
            return;
        }

        shadowDistanceSlider.SetValueWithoutNotify(QualitySettings.shadowDistance);
    }
    public void SetShadowDistance(float setting)
    {
        QualitySettings.shadowDistance = setting;
    }

    void SetLODBiasSlider()
    {
        if (lodBiasSlider == null)
        {
            Debug.LogWarning("NO LOD BIAS SLIDER FOUND");
            return;
        }

        lodBiasSlider.SetValueWithoutNotify(QualitySettings.lodBias);
    }
    public void SetLODBias(float setting)
    {
        QualitySettings.lodBias = setting;
    }
}
