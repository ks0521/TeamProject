using System.Collections;
using System.Collections.Generic;
using Personal.GyuSeong;
using UnityEngine;
using UnityEngine.Serialization;

namespace Personal.GyuSeong
{
    public enum EquipType{Weapon}
    [CreateAssetMenu]
    public class EquipmentTest : ItemSO
    {
        public EquipType equipType;
        [Header("Attack")]
        public int incAtk; //공격력 증가(상수)
        public float multipleAtk; //공격력% 증가(배율)
        [Header("Defence")]
        public int incHp; //hp증가(상수)
        public float multipleHp; //hp% 증가(배율)
        public float dmgReduce; //받는 피해 비율 감소(배율)
        [Header("Reward")]
        public float itemDropRateBonus; //아이템 드랍률(배율)
        public float incGold; //골드 획득량 증가(배율)
        public float incExp; //경험치 획득량 증가(배율)
        public float incStat; //스탯 강화석 획득량 증가(배율)
        [Header("Utility")] 
        public float incSpeed; //이동속도 증가 (배율)
        public float atkSpeed; //공속 증가 (배율)
    }
}