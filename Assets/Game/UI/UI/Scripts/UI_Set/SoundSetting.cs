using Base.Managers;
using UnityEngine;
using UnityEngine.UI;

public class SoundSetting : MonoBehaviour
{
    [SerializeField] private Slider master;
    [SerializeField] private Slider bgm;
    [SerializeField] private Slider skill;
    [SerializeField] private Slider sfx;
    [SerializeField] private VolumeController volumeController;
    private void OnEnable()
    {
        volumeController = GameManager.Instance.GetGameSystem<AudioManager>()._volumeController;
        
        if(volumeController.audioMixer.GetFloat("Master Volume", out float value))
            master.value = Mathf.Pow(10f, value / 20f);
        if(volumeController.audioMixer.GetFloat("BGM", out value))
            bgm.value = Mathf.Pow(10f, value / 20f);
        if(volumeController.audioMixer.GetFloat("Skill Sound", out value))
            skill.value = Mathf.Pow(10f, value / 20f);
        if(volumeController.audioMixer.GetFloat("SFX", out value))
            sfx.value = Mathf.Pow(10f, value / 20f);
        
        master.onValueChanged.AddListener(volumeController.SetMasterVolume);
        bgm.onValueChanged.AddListener(volumeController.SetBGMVolume);
        skill.onValueChanged.AddListener(volumeController.SetSkillVolume);
        sfx.onValueChanged.AddListener(volumeController.SetSFXVolume);
    }

    private void OnDisable()
    {
        master.onValueChanged.RemoveAllListeners();
        bgm.onValueChanged.RemoveAllListeners();
        skill.onValueChanged.RemoveAllListeners();
        sfx.onValueChanged.RemoveAllListeners();
        
        PlayerPrefs.Save(); //오디오 설정 반영
    }
}
