using Base.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Personal.HagYun
{
    public class SkillUIManager : MonoBehaviour, IManager
    {
        [SerializeField] private SkillPopupPresenter skillTreePopupPresenter;
        void Start()
        {
            Init();
        }
        public void Init()
        {
            if (skillTreePopupPresenter != null) skillTreePopupPresenter.Init();
        }
        void OnDestroy()
        {
            if (skillTreePopupPresenter != null) skillTreePopupPresenter.OnDestroyFeat();
        }
        public int GetOrder() => 100;
    }
}