using Base.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace Battle
{
    [CreateAssetMenu(menuName = "Game/Battle/Monster")]
    public class MonsterSO : ScriptableObject
    {
        public Sprite icon; //몬스터 사진
        public GameObject prefeb; //몬스터 프리팹
        public int key; //몬스터 키
        public TotalStat totalStat;
        public BattleStat battleStatStat; //전투스탯
        //public DropTableSO dropTable; //아이템 드랍 테이블(장비 + 이벤트 아이템 등...) <- stageSO로 통합
    }
}