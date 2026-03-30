using Base.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Personal.HagYun
{
    public class SkillUIManager : MonoBehaviour, IManager
    {
        private SkillButtonPresenter skillBtnPresenter;
        public void Init()
        {
            skillBtnPresenter = GetComponent<SkillButtonPresenter>();
            skillBtnPresenter.Init();
        }
        void OnDestroy()
        {
            skillBtnPresenter.OnDestroyFeat();
        }
        public int GetOrder() => 100;
    }
}