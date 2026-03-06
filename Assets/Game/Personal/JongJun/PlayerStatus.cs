using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HJJ
{
    //내 캐릭터가 가져야 할 변수들(public화 또는 프로퍼티화 필요)
    [CreateAssetMenu(fileName = "PlayerStatus", menuName = "ScriptableObject/Player Status")]
    public class PlayerStatus : ScriptableObject
    {
        private string playMode; //Auto or Manual

        /// <summary>
        /// 이하의 내용은 [CommonStatus.cs]에서 관리
        /// private int hp;
        /// private float atk;
        /// private float atkSpeed;
        /// private float moveSpeed;
        /// private float criticalAtkRate;
        /// private float criticalResistanceRate;
        /// </summary>

        private float dropGoldRate;
        private float dropItemRate;
        private float expRate;
        private bool isDead;

        private bool isEquippedWeapon;
        private bool isEquippedHat;
        private bool isEquippedArmor;
        private bool isEquippedAccessory;

        //슬롯에 올린 스킬의 사용 가능 여부(스킬이 반환하는 이벤트 구독?)
        //스킬 쿨타임의 경우 PlayerSkill.cs 등에서 코루틴 등으로 관리
        private bool canUseSkill1;
        private bool canUseSkill2;
        private bool canUseSkill3;
        private bool canUseSkill4;
        private bool canUseSkill5;
        private bool canUseSkill6;
    }
}
