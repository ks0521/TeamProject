using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HJJ
{
    //장비가 가져야 할 변수
    [CreateAssetMenu(fileName = "EquipmentStatus", menuName = "ScriptableObject/Equipment Status")]
    public class EquipmentStatus : ScriptableObject
    {
        public enum EquipType
        {
            Weapon_Bow,
            Weapon_Cane,
            Weapon_Sword,
            Armor_Hat,
            Armor_Armor,
            Accessory
        }

        public enum EquipClass
        {
            Normal,
            Rare,
            Legendary
        }

        private string equipName;
        private EquipType equipType;
        private EquipClass equipClass;

        private int weaponAtk;
        private float weaponCriticalPercentRate;
        private float weaponCriticalDamageRate;
        private int armorHp;
        private float addGoldRate;
        private float addDropItemRate;
        private float addExpRate;

        private int enchantLevel;
        private int enchantExp;
    }
}
