using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

//BGMChanger, SkillSoundPlayer를 여기에서 참조
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    //앞으로 오디오과 관련된 새로운 로직이 필요할 경우
    //여기에 그 cs 파일을 등록하고 'cs파일명?.실행할함수명'을 입력하면 됩니다
    [Header("하위 cs 파일들")]
    [SerializeField] private VolumeController _volumeController;
    [SerializeField] private BGMChanger _bgmChanger;
    [SerializeField] private SkillSoundPlayer _skillPlayer;
    [SerializeField] private SFXPlayer _sfxPlayer;
    //[SerializeField] private PopupManager _popupManager;
    [SerializeField] private QuestManager _questManager;

    void Awake()
    {
        //싱글톤
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        _volumeController?.InitVolumeSliders();
    }

    #region BGM
    public void ChangeMap(BGMChanger.MapType mapType)
    {
        _bgmChanger?.ChangeMap(mapType);
    }
    #endregion

    #region Skill Sounds
    public void PlayNormalAttackSound()
    {
        _skillPlayer?.PlayNormalAttackSound();
    }
    public void PlayNormalHitSound()
    {
        _skillPlayer?.PlayNormalHitSound();
    }
    public void PlaySkillCastSound()
    {
        _skillPlayer?.PlaySkillCastSound();
    }
    public void PlaySkillHitSound()
    {
        _skillPlayer?.PlaySkillHitSound();
    }
    
    public void PlayBeAttackedSound()
    {
        _sfxPlayer?.PlayBeAttackedSound();
    }
    public void PlayBossAttackSound()
    {
        _sfxPlayer?.PlayBossAttackSound();
    }
    public void PlayBossSkillSound()
    {
        _sfxPlayer?.PlayBossSkillSound();
    }
    public void PlayBossDeadSound()
    {
        _sfxPlayer?.PlayBossDeadSound();

    }
    #endregion


    //지금은 임시로 SFXPlayer에 전부 할당했습니다
    //나중에 필요에 따라 다른 cs 파일로 분할하는 게 좋습니다
    #region SFXs
    //공통 사운드들
    public void PlayClickSound()
    {
        _sfxPlayer?.PlayClickSound();
    }
    public void PlayClickButtonSound()
    {
        _sfxPlayer?.PlayClickButtonSound();
    }
    public void PlayClickEmptySound()
    {
        _sfxPlayer?.PlayClickEmptySound();
    }
    public void PlayPopupOpenSound()
    {
        _sfxPlayer?.PlayPopupOpenSound();
    }
    public void PlayPopupCloseSound()
    {
        _sfxPlayer?.PlayPopupCloseSound();
    }


    //전투 관련 효과음들
    public void PlayGetGoldSound()
    {
        _sfxPlayer?.PlayGetItemSound();
    }
    public void PlayGetItemSound()
    {
        _sfxPlayer?.PlayGetItemSound();
        //나중에 고등급 장비 전용 효과음 재생 로직 넣을 것
    }
    public void PlayLevelupSound()
    {
        _sfxPlayer?.PlayLevelupSound();
    }
    public void PlayWinSound()
    {
        _sfxPlayer?.PlayWinSound();
    }
    public void PlayLoseSound() //사망 포함, BGM 교체
    {
        _sfxPlayer?.PlayLoseSound();
    }


    //장비창 효과음
    public void PlayEquipItemSound()
    {
        _sfxPlayer?.PlayEquipItemSound();
    }
    public void PlayUnequipItemSound()
    {
        _sfxPlayer?.PlayUnequipItemSound();
    }
    public void PlayEnchantItemSound()
    {
        _sfxPlayer?.PlayEnchantItemSound();
    }
    public void PlaySynthesizeItemSound()
    {
        _sfxPlayer?.PlaySynthesizeItemSound();
    }


    public void PlayQuestAcceptSound()
    {
        _questManager?.PlayQuestAcceptSound();
    }
    public void PlayQuestClearSound()
    {
        _questManager?.PlayQuestClearSound();
    }


    //스킬창 효과음(임시로 sfxPlayer 배정, 추후 바뀔 수 있음)
    public void PlayAddSkillSound() //스킬 등록 및 교체
    {
        _sfxPlayer?.PlayAddSkillSound();
    }
    public void PlayUnaddSkillSound()
    {
        _sfxPlayer?.PlayUnaddSkillSound();
    }
    public void PlayUseSkillPointSound()
    {
        _sfxPlayer?.PlayUseSkillPointSound();
    }
    public void PlayInitSkillPointSound()
    {
        _sfxPlayer?.PlayInitSkillPointSound();
    }


    //미접속 보상 획득 효과음
    public void PlayGetIdleRewardSound()
    {
        _sfxPlayer?.PlayGetIdleRewardSound();
    }
    #endregion
}
