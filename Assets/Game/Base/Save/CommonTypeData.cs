using System;

namespace Base.Save
{
    //런타임과 세이브 동시에 사용하는 필드 모음
    [Serializable]
    public class StageProgressState
    {
        public int selectedNormalStage; //직전 일반스테이지
        public int selectedNormalChapter; //현재 일반 스테이지
        public int nextChallangeStage; //도전 가능한 스테이지
        public int nextChallangeChapter; //도전 가능한 챕터
    }

    //currency + iteminventory는 분리저장?
    [Serializable]
    public class PlayerCurrencyState
    {
        // public int level;
        // public int skillPoint;
        // public int maxSkillPoint;
        public int exp;
        public int gold;
        public int statStone;
        public int ruby;
    }

    [Serializable]
    public class PlayerInfo
    {
        public int level;
        public int skillPoint;
        public int maxSkillPoint;
        public int weaponGachaLevel;
        public int curWeaponGachaCount;
    }
    /// <summary> 아이템 정보 </summary>
    [Serializable]public class EquipmentEntry
    {
        public int key; //아이템 키
        public int enhancementLevel; //강화 수치
        public int ownedCount; //개수
        public bool isDiscovered; //해금 여부
    }
    
    [Serializable]
    public class LastSessionTime
    {
        public long lastConnectTime;
    }
}