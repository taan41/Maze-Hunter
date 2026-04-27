using System;
using UnityEngine;

[Serializable]
public class GlobalSettings
{
	public event Action OnAudioSettingsChanged;

	[Header("Audio")]
	public float masterVolume = 1f;
	public float musicVolume = 1f;
	public float sfxVolume = 1f;

	[Header("Graphics")]
	public int windowModeIndex = (int)FullScreenMode.Windowed;
	public bool postProcessing = true;

	[Header("Game VFX")]
	public bool enableVFX = true;
	public bool swordTrails = true;
	public bool gunMuzzleFlashes = true;
	public bool bulletHoles = true;
	public bool bloodSplatter = true;

	[ContextMenu("Restore Settings")]
	public void Restore()
	{
		if (!PlayerPrefs.HasKey("HasSettingsSet"))
		{
			Reset();
			return;
		}

		masterVolume = PlayerPrefs.GetFloat("MasterVolume", masterVolume);
		musicVolume = PlayerPrefs.GetFloat("MusicVolume", musicVolume);
		sfxVolume = PlayerPrefs.GetFloat("SFXVolume", sfxVolume);

		windowModeIndex = PlayerPrefs.GetInt("WindowModeIndex", windowModeIndex);
		postProcessing = PlayerPrefs.GetInt("PostProcessing", postProcessing ? 1 : 0) == 1;
		enableVFX = PlayerPrefs.GetInt("EnableVFX", enableVFX ? 1 : 0) == 1;
		swordTrails = PlayerPrefs.GetInt("SwordTrails", swordTrails ? 1 : 0) == 1;
		gunMuzzleFlashes = PlayerPrefs.GetInt("GunMuzzleFlashes", gunMuzzleFlashes ? 1 : 0) == 1;
		bulletHoles = PlayerPrefs.GetInt("BulletDecals", bulletHoles ? 1 : 0) == 1;
		bloodSplatter = PlayerPrefs.GetInt("BloodSplatter", bloodSplatter ? 1 : 0) == 1;

		ApplyGraphicsSettings();
	}

	public void Reset()
	{
		masterVolume = 1f;
		musicVolume = 1f;
		sfxVolume = 1f;

		windowModeIndex = (int)FullScreenMode.Windowed;
		postProcessing = true;

		enableVFX = true;
		swordTrails = true;
		gunMuzzleFlashes = true;
		bulletHoles = true;
		bloodSplatter = true;

		SaveAudio();
		SaveGraphics();
		SaveVFX();

		PlayerPrefs.SetInt("HasSettingsSet", 1);
		PlayerPrefs.Save();
	}

	public void SaveAudio()
	{
		PlayerPrefs.SetFloat("MasterVolume", masterVolume);
		PlayerPrefs.SetFloat("MusicVolume", musicVolume);
		PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
		PlayerPrefs.Save();

		OnAudioSettingsChanged?.Invoke();
	}

	public void SaveGraphics()
	{
		PlayerPrefs.SetInt("WindowModeIndex", windowModeIndex);
		PlayerPrefs.SetInt("PostProcessing", postProcessing ? 1 : 0);
		PlayerPrefs.Save();

		ApplyGraphicsSettings();
	}

	public void SaveVFX()
	{
		PlayerPrefs.SetInt("EnableVFX", enableVFX ? 1 : 0);
		PlayerPrefs.SetInt("SwordTrails", swordTrails ? 1 : 0);
		PlayerPrefs.SetInt("GunMuzzleFlashes", gunMuzzleFlashes ? 1 : 0);
		PlayerPrefs.SetInt("BulletDecals", bulletHoles ? 1 : 0);
		PlayerPrefs.SetInt("BloodSplatter", bloodSplatter ? 1 : 0);
		PlayerPrefs.Save();
	}

	void ApplyGraphicsSettings()
	{
		FullScreenMode mode = (FullScreenMode)windowModeIndex;
		Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, mode);

		Camera.main.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>().renderPostProcessing = postProcessing;
	}
}