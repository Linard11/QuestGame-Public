using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    #region PlayerPref Keys

    public const string MasterVolumeKey = "Settings.Volume.Master";
    public const string MusicVolumeKey = "Settings.Volume.Music";
    public const string SFXVolumeKey = "Settings.Volume.SFX";

    public const string InvertYKey = "Settings.Controls.InvertY";
    public const string MouseSensitivityKey = "Settings.Controls.Sensitivity.Mouse";
    public const string ControllerSensitivityKey = "Settings.Controls.Sensitivity.Controller";

    #endregion

    #region Default Values

    public const float DefaultMasterVolume = 1.0f;
    public const float DefaultMusicVolume = 1.0f;
    public const float DefaultSFXVolume = 1.0f;

    public const bool DefaultInvertYToggle = true;
    public const float DefaultMouseSensitivity = 1.0f;
    public const float DefaultControllerSensitivity = 1.0f;

    #endregion

    #region Inspector

    [Header("Volume")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [Header("Controls")]
    [SerializeField] private Toggle invertYToggle;
    [SerializeField] private Slider mouseSensitivitySlider;
    [SerializeField] private Slider controllerSensitivitySlider;

    #endregion

    #region Unity Event Functions

    private void Awake()
    {
        Initialize(masterVolumeSlider, MasterVolumeKey, DefaultMasterVolume);
        Initialize(musicVolumeSlider, MusicVolumeKey, DefaultMusicVolume);
        Initialize(sfxVolumeSlider, SFXVolumeKey, DefaultSFXVolume);

        // todo toggle
        Initialize(invertYToggle, InvertYKey, DefaultInvertYToggle);
        Initialize(mouseSensitivitySlider, MouseSensitivityKey, DefaultMouseSensitivity);
        Initialize(controllerSensitivitySlider, ControllerSensitivityKey, DefaultControllerSensitivity);
    }

    #endregion

    private void Initialize(Slider slider, string key, float defaultValue)
    {
        slider.SetValueWithoutNotify(PlayerPrefs.GetFloat(key, defaultValue));

        slider.onValueChanged.AddListener(
            (float value) =>
            {
                PlayerPrefs.SetFloat(key, value);
            }
            );
    }

    private void Initialize(Toggle toggle, string key, bool defaultValue)
    {
        toggle.SetIsOnWithoutNotify(GetBool(key, defaultValue));

        toggle.onValueChanged.AddListener(
            (bool value) =>
            {
                SetBool(key, value);
            }
            );
    }

    public static void SetBool(string key, bool value)
    {
        int intValue = value ? 1 : 0;
        PlayerPrefs.SetInt(key, intValue);
    }

    public static bool GetBool(string key, bool defaultValue)
    {
        int defaultIntValue = defaultValue ? 1 : 0;
        int intValue = PlayerPrefs.GetInt(key, defaultIntValue);
        return intValue != 0;
    }
}
