using System.Collections;
using System.Collections.Generic;
using Battle;
using Growth.StatUpgrade;
using Reward;
using UnityEngine;

namespace Base.Data
{
    /// <summary> SO허브같은 게임에 필요한 데이터를 모아놓는 클래스,
    /// 실제 사용은 GameData로 할 것 </summary>
    public class GameDataProvider : MonoBehaviour
    {
        public static GameDataProvider Instance;
        public ScriptableObjectHub hub;

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
    }
    /// <summary> GameDataProvider내 허브의 값들을 편하게 사용할 수 있게 만들어놓은 클래스</summary>
    public static class GameData
    {
        public static DropTableDictionarySO DropDB => GameDataProvider.Instance.hub.dropTable;
        public static StageDictionarySO StageDB => GameDataProvider.Instance.hub.stageTable;
        public static StatusSO StatusDB => GameDataProvider.Instance.hub.statusTable;
        public static SkillDictionarySO SkillDB => GameDataProvider.Instance.hub.SkillDictionarySo;
    }
}