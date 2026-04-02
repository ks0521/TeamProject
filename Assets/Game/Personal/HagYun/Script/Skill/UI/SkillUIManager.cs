using Base.Data;
using Base.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Personal.HagYun
{
    public class SkillUIManager : MonoBehaviour, IManager
    {
        [SerializeField] private SkillPopupPresenter skillTreePopupPresenter;
        // skill 에셋 등록용
        [SerializeField] private Skill[] skillObjArr;
        [SerializeField] private EquipSkillController esController;
        // active skill 장착 시 skill이 들어갈 곳
        [SerializeField] private SkillPool skillPool;
        public SkillPool ActiveSkillPool => skillPool;
        [SerializeField] EventHub eventHub;
        void Start()
        {
            Init();
        }
        public void Init()
        {
            if (skillTreePopupPresenter != null) skillTreePopupPresenter.Init();
            if(skillPool != null)skillPool.ActiveSkillAddInit(skillObjArr);
            if(esController != null)esController.Init(null);
        }
        void OnDestroy()
        {
            if (skillTreePopupPresenter != null) skillTreePopupPresenter.OnDestroyFeat();
        }
        public int GetOrder() => 100;
    }
}