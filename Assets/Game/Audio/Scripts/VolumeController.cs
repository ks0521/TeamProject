using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    [SerializeField] public AudioMixer audioMixer;
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
        //저장된 오디오 값 없으면 중간으로 통일
        audioMixer.SetFloat("Master Volume", 
            Mathf.Log10(Mathf.Clamp(PlayerPrefs.GetFloat("Master Volume",0.5f),0.0001f,1f)) * 20);
        audioMixer.SetFloat("BGM", 
            Mathf.Log10(Mathf.Clamp(PlayerPrefs.GetFloat("BGM",0.5f),0.0001f,1f)) * 20);
        audioMixer.SetFloat("Skill Sound", 
            Mathf.Log10(Mathf.Clamp(PlayerPrefs.GetFloat("Skill Sound",0.5f),0.0001f,1f)) * 20);
        audioMixer.SetFloat("SFX", 
            Mathf.Log10(Mathf.Clamp(PlayerPrefs.GetFloat("SFX",0.5f),0.0001f,1f)) * 20);
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("Master Volume", Mathf.Log10(Mathf.Clamp(volume,0.0001f,1f)) * 20);
        PlayerPrefs.SetFloat("Master Volume", volume);
    }
    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGM", Mathf.Log10(Mathf.Clamp(volume,0.0001f,1f)) * 20);
        PlayerPrefs.SetFloat("BGM", volume);
    }
    public void SetSkillVolume(float volume)
    {
        audioMixer.SetFloat("Skill Sound", Mathf.Log10(Mathf.Clamp(volume,0.0001f,1f)) * 20);
        PlayerPrefs.SetFloat("Skill Sound", volume);
    }
    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(Mathf.Clamp(volume,0.0001f,1f)) * 20);
        PlayerPrefs.SetFloat("SFX", volume);
    }
}
