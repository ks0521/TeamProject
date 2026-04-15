using Base.Data;
using Base.Managers;
using Battle;
using Cysharp.Threading.Tasks;
using Growth.Skill;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace UI.Skill_Set
{
    public class SkillButtonPresenter : MonoBehaviour, IManager
    {
        [SerializeField] private SkillButtonView[] btnViewArr;

        private EventHub eventHub;
        // private EquipSkillController plEquipSkillController;
        private SkillManager skillMgr;
        // private EquipSkill[] plEquipSkill;
        private IReadOnlyList<EquipSkill> plEquipSkill;
        private Player pl;
        // 0 : 기본, 1 : 선택됨
        [SerializeField] private Sprite[] borderArr;
        public int GetOrder() => 99; //UIPresnter와 겹쳐서 99로 조정했습니다
        public void Init()
        {
            eventHub = GameManager.Instance.GetGameSystem<EventHub>();
            if (eventHub == null)
            {
                // Debug.LogWarning("SkillButtonPresenter : eventHub 찾지 못함");
                return;
            }
            pl = GameManager.Instance.GetGameSystem<PlayerManager>().Player;
            if (pl == null)
            {
                // Debug.LogWarning("SkillButtonPresenter : Player 찾지 못함");
                return;
            }
            // plEquipSkillController = pl.ESController;
            // plEquipSkill = plEquipSkillController.EquipSkillList;
            skillMgr = GameManager.Instance.GetGameSystem<SkillManager>();
            plEquipSkill = skillMgr.PlayerEquipSkillList;
            if (plEquipSkill == null) Debug.LogWarning("SkillButtonPresenter : plEquipSkill 없음");
            for (int i = 0; i < 6; i++)
            {
                if (plEquipSkill[i] == null)
                {
                    Debug.LogWarning($"{i}번째 EquipSkill 없음");
                    continue;
                }
                int index = i;
                SkillButtonView tBtnView = btnViewArr[index];
                if (btnViewArr[index] == null)
                {
                    Debug.LogWarning($"{index}번 TestBtnView 연결 안 됨");
                    continue;
                }
                tBtnView.ButtonEventSubscribe(() => SkillUseToMonster(index));
                CooltimeCheckTask(index).Forget();

                var targetEquipSkill = plEquipSkill[index];
                if (targetEquipSkill.isEquipped && targetEquipSkill.Skill.ActiveSkillData is ActiveSkillSO skillData)
                {// ActiveSkillSO skillData = plEquipSkill[index].Skill.ActiveSkillData;
                    tBtnView.SkillIconImageChange(skillData.skillIcon);
                }
                else
                    tBtnView.SkillIconImageUnset();
            }
            EquipSkillEventSet();
        }
        void SkillUseToMonster(int index)
         => eventHub.PlayerSkillUse(index);
        // => plEquipSkillController.TryAtkSkillUseToMonster(index);
        public void OnDestroyFeat()
        {
            EquipSkillEventRemove();
            foreach (SkillButtonView btnView in btnViewArr)
            {
                btnView.OnDestroyFeat();
            }
        }
        async UniTaskVoid CooltimeCheckTask(int index)
        {
            CancellationToken ct = this.GetCancellationTokenOnDestroy();
            while (true)
            {
                if (pl.IsDead)
                {
                    await UniTask.WaitUntil(() => !pl.IsDead, PlayerLoopTiming.Update, ct);
                }
                CooltimeUpdate(index);
                await UniTask.DelayFrame(10, PlayerLoopTiming.Update, ct);

            }
        }
        void CooltimeUpdate(int index)
        {
            EquipSkill tESkill = plEquipSkill[index];
            SkillButtonView tbv = btnViewArr[index];
            if (tESkill == null || tbv == null) return;
            else if (!tESkill.IsCooltime)
            {
                if (tbv.IsCooltimeMaskActiveState) tbv.CooltimeEnd();
                return;
            }
            if (!tbv.IsCooltimeMaskActiveState) tbv.CooltimeStart();
            float curCooltime = tESkill.CurCooltime;
            float cooltimeValue = curCooltime / tESkill.MaxCooltime;
            tbv.CooltimeValueUpdate(cooltimeValue);
            tbv.CurCooltimeTextUpdate(curCooltime);
        }
        void BtnCooltimeStartEvent(int index) => btnViewArr[index].CooltimeStart();
        void BtnCooltimeEndEvent(int index) => btnViewArr[index].CooltimeEnd();
        void SkillIconSet(int slotIndex, ActiveSkill aSkill)
        {
            Debug.Log($"{slotIndex}번 버튼에 {aSkill.SkillData.key}번 스킬 {aSkill.SkillData.skillName} 장착");
            btnViewArr[slotIndex].SkillIconImageChange(aSkill.ActiveSkillData.skillIcon);
        }
        void SkillIconUnset(int index)
        {
            Debug.Log($"{index}번 버튼 장착 해제");
            btnViewArr[index].SkillIconImageUnset();
        }
        int selectSkillNum = 0;
        void SkillSelect(int index)
        {
            SkillButtonView targetSkillBtnView = btnViewArr[index];
            if (!targetSkillBtnView.IsSelected)
            {
                btnViewArr[selectSkillNum].SkillUnset(borderArr[0]);
            }
            selectSkillNum = index;
            targetSkillBtnView.SkillSelect(borderArr[1]);
        }
        void SkillSelectCancel(int index)
        {
            SkillButtonView targetSkillBtnView = btnViewArr[index];
            if (targetSkillBtnView.IsSelected)
            {
                targetSkillBtnView.SkillUnset(borderArr[0]);
            }
        }
        void EquipSkillEventSet()
        {
            eventHub.OnSkillUsed += BtnCooltimeStartEvent;
            eventHub.OnSkillCoolEnd += BtnCooltimeEndEvent;

            eventHub.OnSkillUnset += SkillIconUnset;

            eventHub.OnSkillEquipComplete += SkillIconSet;
        }
        void EquipSkillEventRemove()
        {
            eventHub.OnSkillUsed -= BtnCooltimeStartEvent;
            eventHub.OnSkillCoolEnd -= BtnCooltimeEndEvent;

            eventHub.OnSkillUnset -= SkillIconUnset;

            eventHub.OnSkillEquipComplete -= SkillIconSet;
        }

    }
}