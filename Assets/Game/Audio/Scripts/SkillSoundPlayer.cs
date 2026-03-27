using UnityEngine;

public class SkillSoundPlayer : MonoBehaviour
{
    [SerializeField] public AudioSource skillSource;

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
        #if UNITY_EDITOR
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
        #endif
    }


    public void PlayNormalAttackSound()
    {
        skillSource.clip = normalCast;

        if (skillSource.clip != null) skillSource.PlayOneShot(normalCast);
        else Debug.LogWarning("SkillSound의 AudioSource에 클립이 할당되지 않았습니다!");
    }
    public void PlayNormalHitSound()
    {
        skillSource.clip = normalHit;

        if (skillSource.clip != null) skillSource.PlayOneShot(normalHit);
        else Debug.LogWarning("SkillSound의 AudioSource에 클립이 할당되지 않았습니다!");
    }


    //지금은 단일 소리로 가정했지만, 나중에는 속성마다 로직 구성 필요
    public void PlaySkillCastSound()
    {
        skillSource.clip = skillCast;

        if (skillSource.clip != null) skillSource.PlayOneShot(skillCast);
        else Debug.LogWarning("SkillSound의 AudioSource에 클립이 할당되지 않았습니다!");
    }
    public void PlaySkillHitSound()
    {
        skillSource.clip = skillHit;

        if (skillSource.clip != null) skillSource.PlayOneShot(skillHit);
        else Debug.LogWarning("SkillSound의 AudioSource에 오디오 클립이 할당되지 않았습니다!");
    }
}