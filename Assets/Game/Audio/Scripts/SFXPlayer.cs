using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SFXPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource sfxSource;

    [Header("공통")]
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip popupOpen;
    [SerializeField] private AudioClip popupClose;

    [Header("전투")]
    [SerializeField] private AudioClip getGold;
    [SerializeField] private AudioClip getItem;
    [SerializeField] private AudioClip levelUp;
    [SerializeField] private AudioClip beAttacked;
    [SerializeField] private AudioClip bossAttack;
    [SerializeField] private AudioClip bossSkill;
    [SerializeField] private AudioClip bossDead;
    [SerializeField] private AudioClip win;
    [SerializeField] private AudioClip lose;

    [Header("장비")]
    [SerializeField] private AudioClip equipItem;
    [SerializeField] private AudioClip unequipItem;
    [SerializeField] private AudioClip enchantItem;
    [SerializeField] private AudioClip synthesizeItem;

    [Header("스킬창")]
    [SerializeField] private AudioClip addSkill;
    [SerializeField] private AudioClip unaddSkill;
    [SerializeField] private AudioClip useSP;
    [SerializeField] private AudioClip initSP;

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

        if (sfxSource.clip != null) sfxSource.PlayOneShot(sfxSource.clip);
        else Debug.LogWarning("AudioSource에 클립이 할당되지 않았습니다!");
    }
    public void PlayPopupCloseSound()
    {
        sfxSource.clip = popupClose;

        if (sfxSource.clip != null) sfxSource.PlayOneShot(sfxSource.clip);
        else Debug.LogWarning("AudioSource에 클립이 할당되지 않았습니다!");
    }


    //전투 관련 효과음들
    public void PlayGetGoldSound()
    {
        sfxSource.clip = getGold;

        if (sfxSource.clip != null) sfxSource.PlayOneShot(sfxSource.clip);
        else Debug.LogWarning("AudioSource에 클립이 할당되지 않았습니다!");
    }
    public void PlayGetItemSound()
    {
        //나중에 고등급 장비 전용 효과음 재생 로직 넣을 것
        sfxSource.clip = getItem;

        if (sfxSource.clip != null) sfxSource.PlayOneShot(sfxSource.clip);
        else Debug.LogWarning("AudioSource에 클립이 할당되지 않았습니다!");
    }
    public void PlayLevelupSound()
    {
        sfxSource.clip = levelUp;

        if (sfxSource.clip != null) sfxSource.PlayOneShot(sfxSource.clip);
        else Debug.LogWarning("AudioSource에 클립이 할당되지 않았습니다!");
    }
    public void PlayBeAttackedSound()
    {
        sfxSource.clip = beAttacked;

        if (sfxSource.clip != null) sfxSource.PlayOneShot(sfxSource.clip);
        else Debug.LogWarning("AudioSource에 클립이 할당되지 않았습니다!");
    }
    public void PlayBossAttackSound()
    {
        sfxSource.clip = bossAttack;

        if (sfxSource.clip != null) sfxSource.PlayOneShot(sfxSource.clip);
        else Debug.LogWarning("AudioSource에 클립이 할당되지 않았습니다!");
    }
    public void PlayBossSkillSound()
    {
        sfxSource.clip = bossSkill;

        if (sfxSource.clip != null) sfxSource.PlayOneShot(sfxSource.clip);
        else Debug.LogWarning("AudioSource에 클립이 할당되지 않았습니다!");
    }
    public void PlayBossDeadSound()
    {
        sfxSource.clip = bossDead;

        if (sfxSource.clip != null) sfxSource.PlayOneShot(sfxSource.clip);
        else Debug.LogWarning("AudioSource에 클립이 할당되지 않았습니다!");
    }
    public void PlayWinSound()
    {
        sfxSource.clip = win;

        if (sfxSource.clip != null) sfxSource.PlayOneShot(sfxSource.clip);
        else Debug.LogWarning("AudioSource에 클립이 할당되지 않았습니다!");
    }
    public void PlayLoseSound() //사망 포함, BGM 교체
    {
        sfxSource.clip = lose;

        if (sfxSource.clip != null) sfxSource.PlayOneShot(sfxSource.clip);
        else Debug.LogWarning("AudioSource에 클립이 할당되지 않았습니다!");
    }


    //장비창 효과음
    public void PlayEquipItemSound()
    {
        sfxSource.clip = equipItem;

        if (sfxSource.clip != null) sfxSource.PlayOneShot(sfxSource.clip);
        else Debug.LogWarning("AudioSource에 클립이 할당되지 않았습니다!");
    }
    public void PlayUnequipItemSound()
    {
        sfxSource.clip = unequipItem;

        if (sfxSource.clip != null) sfxSource.PlayOneShot(sfxSource.clip);
        else Debug.LogWarning("AudioSource에 클립이 할당되지 않았습니다!");
    }
    public void PlayEnchantItemSound()
    {
        sfxSource.clip = enchantItem;

        if (sfxSource.clip != null) sfxSource.PlayOneShot(sfxSource.clip);
        else Debug.LogWarning("AudioSource에 클립이 할당되지 않았습니다!");
    }
    public void PlaySynthesizeItemSound()
    {
        sfxSource.clip = synthesizeItem;

        if (sfxSource.clip != null) sfxSource.PlayOneShot(sfxSource.clip);
        else Debug.LogWarning("AudioSource에 클립이 할당되지 않았습니다!");
    }


    //스킬창 효과음(임시로 sfxPlayer 배정, 추후 바뀔 수 있음)
    public void PlayAddSkillSound() //스킬 등록 및 교체
    {
        sfxSource.clip = addSkill;

        if (sfxSource.clip != null) sfxSource.PlayOneShot(sfxSource.clip);
        else Debug.LogWarning("AudioSource에 클립이 할당되지 않았습니다!");
    }
    public void PlayUnaddSkillSound()
    {
        sfxSource.clip = unaddSkill;

        if (sfxSource.clip != null) sfxSource.PlayOneShot(sfxSource.clip);
        else Debug.LogWarning("AudioSource에 클립이 할당되지 않았습니다!");
    }
    public void PlayUseSkillPointSound()
    {
        sfxSource.clip = useSP;

        if (sfxSource.clip != null) sfxSource.PlayOneShot(sfxSource.clip);
        else Debug.LogWarning("AudioSource에 클립이 할당되지 않았습니다!");
    }
    public void PlayInitSkillPointSound()
    {
        sfxSource.clip = initSP;

        if (sfxSource.clip != null) sfxSource.PlayOneShot(sfxSource.clip);
        else Debug.LogWarning("AudioSource에 클립이 할당되지 않았습니다!");
    }



    //미접속 보상 획득 효과음
    public void PlayGetIdleRewardSound()
    {

    }
}
