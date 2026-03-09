using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SkillSoundTest : MonoBehaviour
{
    private AudioSource skillSource;

    private void Awake()
    {
        skillSource = gameObject.GetComponent<AudioSource>();
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlaySkillSound();
        }
    }

    void PlaySkillSound()
    {
        if (skillSource.clip != null) skillSource.PlayOneShot(skillSource.clip);
        else Debug.LogWarning("SkillSound의 AudioSource에 오디오 클립이 할당되지 않았습니다!");
    }
}
