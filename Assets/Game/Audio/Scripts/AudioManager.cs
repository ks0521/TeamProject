using Base.Data;
using Base.Managers;
using Battle;
using Growth.Skill;
using QuestSystem;
using UnityEngine;

//BGMChanger, SkillSoundPlayer를 여기에서 참조
public class AudioManager : MonoBehaviour, IManager
{
    public static AudioManager instance;

    //앞으로 오디오과 관련된 새로운 로직이 필요할 경우
    //여기에 그 cs 파일을 등록하고 'cs파일명?.실행할함수명'을 입력하면 됩니다
    [Header("하위 cs 파일들")]
    [SerializeField] private VolumeController _volumeController;
    [SerializeField] private BGMChanger _bgmChanger;
    [SerializeField] private SkillSoundPlayer _skillPlayer;
    [SerializeField] private SFXPlayer _sfxPlayer;
    [SerializeField] private QuestManager _questManager;
    //[SerializeField] private PopupManager _popupManager;
    //[SerializeField] private QuestManager _questManager;


    private EventHub eventHub;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        //디버그용
        //Init();
    }

    public void Init()
    {
        _volumeController?.InitVolumeSliders();

        var hub = GameManager.Instance.GetGameSystem<EventHub>();
        if (hub != null && _sfxPlayer != null)
        {
            //필요한 이벤트들은 여기서 추가
            //hub.OnSkillUsed += OnSkillUsed;

            hub.OnButtonClicked -= _sfxPlayer.PlayClickButtonSound;
            hub.OnButtonClicked += _sfxPlayer.PlayClickButtonSound;
            hub.OnPopupOpened -= _sfxPlayer.PlayPopupOpenSound;
            hub.OnPopupOpened += _sfxPlayer.PlayPopupOpenSound;
            hub.OnPopupClosed -= _sfxPlayer.PlayPopupCloseSound;
            hub.OnPopupClosed += _sfxPlayer.PlayPopupCloseSound;

            hub.OnMonsterHit -= _sfxPlayer.PlayHitSound;
            hub.OnMonsterHit += _sfxPlayer.PlayHitSound;
            hub.OnPlayerHit -= _sfxPlayer.PlayBeAttackedSound; 
            hub.OnPlayerHit += _sfxPlayer.PlayBeAttackedSound;

            hub.OnClearStage -= PlayWinSound;
            hub.OnClearStage += PlayWinSound;
            hub.OnFailStage -= PlayLoseSound;
            hub.OnFailStage += PlayLoseSound;

            hub.OnGetCurrency -= PlayGetGoldSound;
            hub.OnGetCurrency += PlayGetGoldSound;
            hub.OnGetItems -= PlayGetItemSound;
            hub.OnGetItems += PlayGetItemSound;
            hub.OnLevelChange -= PlayLevelupSound;
            hub.OnLevelChange += PlayLevelupSound;

            hub.OnSkillSet -= PlayAddSkillSound;
            hub.OnSkillSet += PlayAddSkillSound;
            hub.OnSkillUnset -= PlayUnaddSkillSound;
            hub.OnSkillUnset += PlayUnaddSkillSound;

            hub.OnQuestCompleted -= PlayQuestClearSound;
            hub.OnQuestCompleted += PlayQuestClearSound;

            Debug.Log("AudioManager: 모든 효과음 이벤트 연결 완료");
        }
    }

    public int GetOrder() => 300;

    #region BGM
    public void ChangeMap(BGMChanger.MapType mapType)
    {
        _bgmChanger?.ChangeMap(mapType);
    }
    #endregion

    #region Skill Sounds
    void OnSkillUsed(SkillSO skillSO)
    {
        if (skillSO.skillSound != null)
        {
            _skillPlayer.skillSource?.PlayOneShot(skillSO.skillSound);
        }
    }
    public void PlaySkillCastSound() //스킬 캐스팅 사운드
    {
        _skillPlayer?.PlaySkillCastSound();
    }
    public void PlaySkillHitSound() //스킬 피격 사운드
    {
        _skillPlayer?.PlaySkillHitSound();
    }
    public void PlayNormalAttackSound() //플레이어 일반공격 사운드
    {
        _skillPlayer?.PlayNormalAttackSound();
    }
    public void PlayBeAttackedSound() //플레이어 피격 사운드
    {
        _sfxPlayer?.PlayBeAttackedSound();
    }
    public void PlayBossAttackSound() //보스 공격 사운드
    {
        _sfxPlayer?.PlayBossAttackSound();
    }
    public void PlayBossSkillSound() //보스 스킬 사운드
    {
        _sfxPlayer?.PlayBossSkillSound();
    }
    public void PlayBossDeadSound() // 보스 사망 사운드
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
    public void PlayLevelupSound(int level)
    {
        _sfxPlayer?.PlayLevelupSound();
    }
    public void PlayWinSound(StageSO stage)
    {
        _sfxPlayer?.PlayWinSound();
    }
    public void PlayLoseSound(StageSO stage) //사망 포함, BGM 교체
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

    //이제 퀘스트 완료는 EventHub가 감지함
    public void PlayQuestClearSound(QuestData questData)
    {
        _sfxPlayer?.PlayQuestClearSound();
    }


    //스킬창 효과음(임시로 sfxPlayer 배정, 추후 바뀔 수 있음)
    public void PlayAddSkillSound(int order) //스킬 등록 및 교체
    {
        _sfxPlayer?.PlayAddSkillSound();
    }
    public void PlayUnaddSkillSound(int order)
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
