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

    public void InitVolumeSliders()
    {
        if (sliderMasterVolume != null) sliderMasterVolume.onValueChanged.AddListener(SetMasterVolume);
        if (sliderBGMVolume != null) sliderBGMVolume.onValueChanged.AddListener(SetBGMVolume);
        if (sliderSkillVolume != null) sliderSkillVolume.onValueChanged.AddListener(SetSkillVolume);
        if (sliderSFXVolume != null) sliderSFXVolume.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("Master Volume", Mathf.Log10(Mathf.Clamp(volume, min:0.0001f, max:1f)) * 20);
    }
    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("Master Volume", Mathf.Log10(Mathf.Clamp(volume, min: 0.0001f, max: 1f)) * 20);
    }
    public void SetSkillVolume(float volume)
    {
        audioMixer.SetFloat("Master Volume", Mathf.Log10(Mathf.Clamp(volume, min: 0.0001f, max: 1f)) * 20);
    }
    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("Master Volume", Mathf.Log10(Mathf.Clamp(volume, min: 0.0001f, max: 1f)) * 20);
    }
}
