using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    void Start()
    {
        Invoke(nameof(PlayMusic), 0.1f);

        if (musicVolumeSlider != null)
        {
            float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
            musicVolumeSlider.value = savedVolume;
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxVolumeSlider != null)
        {
            float savedVolume = PlayerPrefs.GetFloat("SoundVolume", 1f);
            sfxVolumeSlider.value = savedVolume;
            sfxVolumeSlider.onValueChanged.AddListener(SetSfxVolume);
        }
    }

    public void Play()
    {
        LevelManager.Instance.LoadScene("Level_1", "CrossFade");
        MusicManager.Instance.PlayMusic("Level1");
    }

    public void OnVolumeChanged(float volume)
    {
        MusicManager.Instance.SetVolume(volume);
    }

    public void GoMainMenu()
    {
        Time.timeScale = 1;
        LevelManager.Instance.LoadScene("MainMenu", "CrossFade");
        MusicManager.Instance.PlayMusic("MainMenu");
    }

    public void Quit()
    {
        Application.Quit();
    }

    void PlayMusic()
    {
        MusicManager.Instance?.PlayMusic("MainMenu");
    }

    private void SetMusicVolume(float volume)
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.SetVolume(volume);
        }
    }

    private void SetSfxVolume(float volume)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetVolume(volume);
        }
    }
}
