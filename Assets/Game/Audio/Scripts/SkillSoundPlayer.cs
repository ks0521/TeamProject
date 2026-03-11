using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSoundPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource skillSource;

    [SerializeField] private AudioClip normalCast;
    [SerializeField] private AudioClip normalHit;
    [SerializeField] private AudioClip skillCast;
    [SerializeField] private AudioClip skillHit;

    void Awake()
    {
        skillSource = gameObject.GetComponent<AudioSource>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayNormalAttackSound();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayNormalHitSound();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PlaySkillCastSound();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            PlaySkillHitSound();
        }
    }


    public void PlayNormalAttackSound()
    {
        skillSource.clip = normalCast;

        if (skillSource.clip != null) skillSource.PlayOneShot(normalCast);
        else Debug.LogWarning("SkillSound�� AudioSource�� Ŭ���� �Ҵ���� �ʾҽ��ϴ�!");
    }
    public void PlayNormalHitSound()
    {
        skillSource.clip = normalHit;

        if (skillSource.clip != null) skillSource.PlayOneShot(normalHit);
        else Debug.LogWarning("SkillSound�� AudioSource�� Ŭ���� �Ҵ���� �ʾҽ��ϴ�!");
    }


    //������ ���� �Ҹ��� ����������, ���߿��� �Ӽ����� ���� ���� �ʿ�
    public void PlaySkillCastSound()
    {
        skillSource.clip = skillCast;

        if (skillSource.clip != null) skillSource.PlayOneShot(skillCast);
        else Debug.LogWarning("SkillSound�� AudioSource�� Ŭ���� �Ҵ���� �ʾҽ��ϴ�!");
    }
    public void PlaySkillHitSound()
    {
        skillSource.clip = skillHit;

        if (skillSource.clip != null) skillSource.PlayOneShot(skillHit);
        else Debug.LogWarning("SkillSound�� AudioSource�� Ŭ���� �Ҵ���� �ʾҽ��ϴ�!");
    }
}