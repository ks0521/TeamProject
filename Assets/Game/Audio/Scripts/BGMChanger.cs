using Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class manager
{
    [SerializeField] private BGMChanger bgm;
}

public class BGMChanger : MonoBehaviour
{
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioClip bgmForest;
    [SerializeField] private AudioClip bgmGrave;
    private StageSO curStage;
    public void Init(StageSO stage)
    {
        ChangeMap(stage);
        bgmSource.Play();
    }
    public void ChangeMap(StageSO stage)
    {
        if (stage.chapter == curStage?.chapter) return;
        switch (stage.chapter)
        {
            case 1:
                bgmSource.clip = bgmForest;
                break;
            case 2:
                bgmSource.clip = bgmGrave;
                break;
        }
        bgmSource.Play();
    }
}