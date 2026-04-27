using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MenuBase
{
	[Header("Panels")]
	[SerializeField] GameObject audioSettingsPanel;
	[SerializeField] GameObject graphicsSettingsPanel;

	[Header("Audio Settings")]
	[SerializeField] Slider masterVolumeSlider;
	[SerializeField] Slider musicVolumeSlider;
	[SerializeField] Slider sfxVolumeSlider;

	[Header("Graphics Settings")]
	[SerializeField] TMP_Dropdown windowModeDropdown;
	[SerializeField] Toggle postProcessingToggle;

	[Header("VFX Settings")]
	[SerializeField] Toggle enableVFXToggle;
	[SerializeField] Toggle swordTrailsToggle;
	[SerializeField] Toggle gunMuzzleFlashesToggle;
	[SerializeField] Toggle bulletDecalsToggle;
	[SerializeField] Toggle bloodSplatterToggle;

	[Header("Others")]
	[SerializeField] Button backButton;

	GlobalSettings GlobalSettings => GlobalSettingsManager.Instance.settings;

	bool initlialized = false;

	public override void Open()
	{
		if (!initlialized)
		{
			initlialized = true;
			InitAudioSettings();
			InitGraphicsSettings();
			InitVFXSettings();
			backButton.onClick.AddListener(OnBackClicked);
		}

		audioSettingsPanel.SetActive(true);
		graphicsSettingsPanel.SetActive(false);
		gameObject.SetActive(true);
	}

	public override void Close()
	{
		gameObject.SetActive(false);
		if (MenuManager.Instance != null)
		{
			MenuManager.Instance.pauseMenuPanel.Open(false);
		}
	}

	void InitAudioSettings()
	{
		masterVolumeSlider.value = GlobalSettings.masterVolume;
		musicVolumeSlider.value = GlobalSettings.musicVolume;
		sfxVolumeSlider.value = GlobalSettings.sfxVolume;

		masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
		musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
		sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
	}

	void InitGraphicsSettings()
	{
		FullScreenMode[] windowModes = (FullScreenMode[])System.Enum.GetValues(typeof(FullScreenMode));
		windowModeDropdown.ClearOptions();
		foreach (FullScreenMode mode in windowModes)
		{
			windowModeDropdown.options.Add(new TMP_Dropdown.OptionData(mode.ToString()));
		}

		windowModeDropdown.value = GlobalSettings.windowModeIndex;
		postProcessingToggle.isOn = GlobalSettings.postProcessing;

		windowModeDropdown.onValueChanged.AddListener(OnWindowModeChanged);
		postProcessingToggle.onValueChanged.AddListener(OnPostProcessingChanged);
	}

	void InitVFXSettings()
	{
		enableVFXToggle.isOn = GlobalSettings.enableVFX;
		swordTrailsToggle.isOn = GlobalSettings.swordTrails;
		gunMuzzleFlashesToggle.isOn = GlobalSettings.gunMuzzleFlashes;
		bulletDecalsToggle.isOn = GlobalSettings.bulletDecals;
		bloodSplatterToggle.isOn = GlobalSettings.bloodSplatter;

		enableVFXToggle.onValueChanged.AddListener(OnEnableVFXChanged);
		swordTrailsToggle.onValueChanged.AddListener(OnSwordTrailsChanged);
		gunMuzzleFlashesToggle.onValueChanged.AddListener(OnGunMuzzleFlashesChanged);
		bulletDecalsToggle.onValueChanged.AddListener(OnBulletDecalsChanged);
		bloodSplatterToggle.onValueChanged.AddListener(OnBloodSplatterChanged);
	}

	void OnMasterVolumeChanged(float value)
	{
		GlobalSettings.masterVolume = value;
		GlobalSettings.SaveAudio();
	}

	void OnMusicVolumeChanged(float value)
	{
		GlobalSettings.musicVolume = value;
		GlobalSettings.SaveAudio();
	}

	void OnSFXVolumeChanged(float value)
	{
		GlobalSettings.sfxVolume = value;
		GlobalSettings.SaveAudio();
	}

	void OnWindowModeChanged(int index)
	{
		GlobalSettings.windowModeIndex = index;
		GlobalSettings.SaveGraphics();
	}

	void OnPostProcessingChanged(bool value)
	{
		GlobalSettings.postProcessing = value;
		GlobalSettings.SaveGraphics();
	}

	void OnEnableVFXChanged(bool value)
	{
		GlobalSettings.enableVFX = value;
		GlobalSettings.SaveVFX();
	}

	void OnSwordTrailsChanged(bool value)
	{
		GlobalSettings.swordTrails = value;
		GlobalSettings.SaveVFX();
	}

	void OnGunMuzzleFlashesChanged(bool value)
	{
		GlobalSettings.gunMuzzleFlashes = value;
		GlobalSettings.SaveVFX();
	}

	void OnBulletDecalsChanged(bool value)
	{
		GlobalSettings.bulletDecals = value;
		GlobalSettings.SaveVFX();
	}

	void OnBloodSplatterChanged(bool value)
	{
		GlobalSettings.bloodSplatter = value;
		GlobalSettings.SaveVFX();
	}

	void OnBackClicked()
	{
		Close();
	}
}