using Base.Managers;
using QuestSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestUIPresenter : MonoBehaviour
{
    [SerializeField] private QuestManager questManager;
    private void OnEnable()
    {
        questManager = GameManager.Instance.GetGameSystem<QuestManager>();
        questManager.LoadProgress();
        questManager.RefreshQuests();
    }
}
