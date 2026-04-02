using Battle;
using Growth.Skill;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Personal.HagYun
{
    public abstract class Skill : MonoBehaviour
    {
        public abstract SkillSO SkillData { get; }
        [SerializeField] protected Character owner;
        [SerializeField] protected int curLv;
        public int CurLv => curLv;
        public int MaxLv => SkillData.maxLv;
        public virtual void Init(Character owner)
        {
            if (owner != null && this.owner == null) this.owner = owner;
            StatUpdate();
        }
        public abstract void StatUpdate();
        public bool TryLevelSet(int setLv, out int lvChangeCnt)
        {
            int maxLv = MaxLv;
            if (setLv < 0 || maxLv <= curLv || setLv == curLv)
            {
                lvChangeCnt = 0;
                return false;
            }
            else if (maxLv < setLv)
            {
                Debug.LogWarning("시도하려는 Setting Lv이 MaxLv보다 높습니다. Setting Lv을 MaxLv로 조정합니다.");
                setLv = maxLv;
            }
            lvChangeCnt = setLv - curLv;
            curLv = setLv;
            StatUpdate();
            return true;
        }
        public virtual void SkillImgSet(Image img)
        {
            img.sprite = SkillData.skillIcon;
        }
        public static void SkillImgUnset(Image img)
        {
            img.sprite = null;
            img.rectTransform.localEulerAngles = Vector3.zero;
        }
        public bool TryLevelOneUp()
        {
            if (MaxLv <= curLv) return false;
            curLv++;
            StatUpdate();
            return true;
        }
        public bool TryLevelMaxUp(out int lvUpCnt)
        {
            int maxLv = MaxLv;
            if (maxLv <= curLv)
            {
                lvUpCnt = 0;
                return false;
            }
            lvUpCnt = maxLv - curLv;
            curLv += lvUpCnt;
            StatUpdate();
            return true;
        }
        public bool TryLevelReset(out int lvResetCnt)
        {
            if (curLv == 0)
            {
                lvResetCnt = 0;
                return false;
            }
            lvResetCnt = curLv;
            curLv = 0;
            StatUpdate();
            return true;
        }
    }
}