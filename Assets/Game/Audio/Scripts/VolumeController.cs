using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider sliderMasterVolume;
    [SerializeField] private Slider sliderBGMVolume;
    [SerializeField] private Slider sliderSkillVolume;
    [SerializeField] private Slider sliderSFXVolume;

    void Awake()
    {
        sliderMasterVolume.onValueChanged.AddListener(SetMasterVolume);
        sliderBGMVolume.onValueChanged.AddListener(SetBGMVolume);
        sliderSkillVolume.onValueChanged.AddListener(SetSkillVolume);
        sliderSFXVolume.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("Master Volume", Mathf.Log10(volume) * 20);
    }
    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
    }
    public void SetSkillVolume(float volume)
    {
        audioMixer.SetFloat("Skill Sound", Mathf.Log10(volume) * 20);
    }
    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
    }
}
