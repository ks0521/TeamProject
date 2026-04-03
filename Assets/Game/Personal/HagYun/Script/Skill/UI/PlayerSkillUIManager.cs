using Base.Data;
using Base.Managers;
using Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Personal.HagYun
{
    public class PlayerSkillUIManager : MonoBehaviour, IManager
    {
        [SerializeField] private SkillPopupPresenter skillTreePopupPresenter;


        [SerializeField] private SkillPool skillPool;
        public SkillPool SkillPool => skillPool;

        Player pl;
        [SerializeField] private EquipSkillController esController;

        [SerializeField] EventHub eventHub;
        void Start()
        {
            Init();
        }
        public void Init()
        {
            if (skillTreePopupPresenter != null) skillTreePopupPresenter.Init();
            if (skillPool != null) skillPool.Init();
            if (esController != null) esController.Init(pl);
        }
        void OnDestroy()
        {
            if (skillTreePopupPresenter != null) skillTreePopupPresenter.OnDestroyFeat();
        }
        public int GetOrder() => 100;
    }
}