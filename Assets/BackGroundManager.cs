using Base.Data;
using Base.Managers;
using Battle;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundManager : MonoBehaviour,IManager
{
    private EventHub eventHub;
    [SerializeField] private List<Sprite> sprites;
    private SpriteRenderer backGround;
    public int GetOrder() => 7; //StageManager보다 먼저 초기화되어야함
    public void Init()
    {
        backGround = GetComponent<SpriteRenderer>();
        eventHub = GameManager.Instance.GetGameSystem<EventHub>();
        eventHub.OnChangeStage += SetBackGround;
    }

    private void SetBackGround(StageSO stage)
    {
        if (sprites.Count < stage.chapter) return;
        backGround.sprite = sprites[stage.chapter - 1];
    }
}
