using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private string _sfxParamName;
    [SerializeField] private string _musicParamName;
    [SerializeField] private float _defaultSFXVal;
    [SerializeField] private float _defaultMusicVal;
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Slider _musicslider;
    private bool _paused;

    private void Start()
    {
        _sfxSlider.value = PlayerPrefs.GetFloat(_sfxParamName, _defaultSFXVal);
        _musicslider.value = PlayerPrefs.GetFloat(_musicParamName, _defaultMusicVal);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    public void TogglePause()
    {
        _paused = !_paused;
        _pauseMenu.SetActive(_paused);
    }

    public void SetSFXAudioLevel(float sliderValue)
    {
        _mixer.SetFloat(_sfxParamName, Mathf.Log10(sliderValue) * 20);

        PlayerPrefs.SetFloat(_sfxParamName, sliderValue);
    }

    public void SetMusicAudioLevel(float sliderValue)
    {
        _mixer.SetFloat(_musicParamName, Mathf.Log10(sliderValue) * 20);

        PlayerPrefs.SetFloat(_musicParamName, sliderValue);
    }
}
