using Base.Data;
using UnityEngine;

namespace Battle
{
    [CreateAssetMenu(menuName = "Game/Battle/Monster")]
    public class MonsterSO : ScriptableObject
    {
        public BattleStat battleStat; //전투스탯

        public int rewardGold; //골드
        public int rewardStatStone; //강화석
        public int rewardExp; //경험치

        //public DropTableSO dropTable; //아이템 드랍 테이블(장비 + 이벤트 아이템 등...) <- stageSO로 통합
    }
}