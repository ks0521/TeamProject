using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SFXPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip popupOpen;
    [SerializeField] private AudioClip popupClose;

    [SerializeField] private AudioClip equipItem;
    [SerializeField] private AudioClip unequipItem;

    //공통 사운드들
    public void PlayClickSound()
    {

    }
    public void PlayClickButtonSound()
    {
        sfxSource.clip = buttonClick;

        if (sfxSource.clip != null) sfxSource.PlayOneShot(buttonClick);
        else Debug.LogWarning("AudioSource에 클립이 할당되지 않았습니다!");
    }
    public void PlayClickEmptySound()
    {

    }

    public void PlayPopupOpenSound()
    {
        sfxSource.clip = popupOpen;

        if (sfxSource.clip != null) sfxSource.PlayOneShot(popupOpen);
        else Debug.LogWarning("AudioSource에 클립이 할당되지 않았습니다!");
    }
    public void PlayPopupCloseSound()
    {
        sfxSource.clip = popupClose;

        if (sfxSource.clip != null) sfxSource.PlayOneShot(popupClose);
        else Debug.LogWarning("AudioSource에 클립이 할당되지 않았습니다!");
    }


    //전투 관련 효과음들
    public void PlayGetGoldSound()
    {

    }
    public void PlayGetItemSound()
    {
        //나중에 고등급 장비 전용 효과음 재생 로직 넣을 것
    }
    public void PlayLevelupSound()
    {

    }
    public void PlayBeAttackedSound()
    {

    }
    public void PlayBossAttackSound()
    {

    }
    public void PlayBossSkillSound()
    {

    }
    public void PlayBossDeadSound()
    {

    }
    public void PlayWinSound()
    {

    }
    public void PlayLoseSound() //사망 포함, BGM 교체
    {

    }


    //장비창 효과음
    public void PlayEquipItemSound()
    {
        sfxSource.clip = equipItem;

        if (sfxSource.clip != null) sfxSource.PlayOneShot(equipItem);
        else Debug.LogWarning("AudioSource에 클립이 할당되지 않았습니다!");
    }
    public void PlayUnequipItemSound()
    {
        sfxSource.clip = unequipItem;

        if (sfxSource.clip != null) sfxSource.PlayOneShot(unequipItem);
        else Debug.LogWarning("AudioSource에 클립이 할당되지 않았습니다!");
    }
    public void PlayEnchantItemSound()
    {

    }
    public void PlaySynthesizeItemSound()
    {

    }


    //스킬창 효과음(임시로 sfxPlayer 배정, 추후 바뀔 수 있음)
    public void PlayAddSkillSound() //스킬 등록 및 교체
    {

    }
    public void PlayUnaddSkillSound()
    {

    }
    public void PlayUseSkillPointSound()
    {

    }
    public void PlayInitSkillPointSound()
    {

    }



    //미접속 보상 획득 효과음
    public void PlayGetIdleRewardSound()
    {

    }
}
