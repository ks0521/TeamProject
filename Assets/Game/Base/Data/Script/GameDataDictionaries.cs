using Base.Managers;
using Battle;
using Growth.Equipment;
using Growth.StatUpgrade;
using UnityEngine;

namespace Base.Data
{
    /// <summary> SO허브같은 게임에 필요한 데이터를 모아놓는 클래스,
    /// 실제 사용은 GameData로 할 것 </summary>
    public class GameDataDictionaries : MonoBehaviour, IGameSystem
    {
        //public static GameDataProvider Instance; //MVP 완료 후 제거
        public StageDictionarySO stageTable;
        public StatusSO statusTable;
        public SkillDictionarySO SkillTable;
        public CurrencyDataBaseSO currencyTable;
        public EquipmentDictionarySO equipmentTable;
        public ItemDictionarySO itemTable;

        public int GetOrder() => 0; //다른 매니저에서 참고하기 때문에 가장 우선 활성화
    }
}