using Base.Data;
using Base.Managers;
using Base.Save;
using Growth.Equipment;
using Shop.Gacha;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
namespace Shop.Gacha
{
    public class GachaManager : MonoBehaviour, IManager
    {
        private GameDataProvider gameDataProvider;
        private RuntimeProgressData progressState;
        private EventHub hub;
        private ItemDropManager dropManager;

        [Header("가챠 SO 모음")]
        [SerializeField] private List<GachaConfigSO> gachaConfigSO;
        private Dictionary<EquipType, GachaConfigSO> gachaDic = new();
        private readonly Dictionary<EquipType, GachaRuntimeData> runtimeDic = new();

        [Serializable]
        private class GachaRuntimeData
        {
            public int currentLevel;
            public int totalDrawCount;
        }
        public void Init()
        {
            gameDataProvider = GameManager.Instance.GetGameSystem<GameDataProvider>();
            progressState = GameManager.Instance.GetGameSystem<ProgressManager>().progress;
            hub = GameManager.Instance.GetGameSystem<EventHub>();
            dropManager = GameManager.Instance.GetGameSystem<ItemDropManager>();

            gachaDic.Clear();

            if (gachaConfigSO == null)
            {
                Debug.LogWarning("가챠 SO 가 비어있음");
            }


            for(int i = 0; i < gachaConfigSO.Count; i++)
            {
                GachaConfigSO config = gachaConfigSO[i];

                if (config == null)
                {
                    Debug.LogWarning($"gachaConfigSO {i} 가 null");
                    continue;
                }

                EquipType type = config.targetEquipType;

                if (gachaDic.ContainsKey(config.targetEquipType))
                {
                    Debug.LogWarning($"{config.targetEquipType} 타입 SO 가 중복 등록됨");
                    continue;
                }

                gachaDic.Add(type, config);
                runtimeDic.Add(type , new GachaRuntimeData());

                ValidateConfig(config);

                //가챠 레벨 가져오기
            }
        }
        private void ValidateConfig(GachaConfigSO config)
        {
            if (config.maxLevel <= 0)
            {
                Debug.LogWarning($"{config.name} : maxLevel 은 1 이상이어야 합니다.");
            }

            if (config.levelUpDraw == null)
            {
                Debug.LogWarning($"{config.name} : levelUpDrawRequirements 가 null 입니다.");
                return;
            }

            if (config.levelUpDraw.Count != config.maxLevel - 1)
            {
                Debug.LogWarning(
                    $"{config.name} : levelUpDraw 개수({config.levelUpDraw.Count})가 maxLevel 개수 ({config.maxLevel - 1})가 다릅니다.");
            }
        }//SO에 MaxLevel 값 확인 , levelUpDraw 개수 확인(인스펙터 검사용 함수)
        public int GetOrder() => 220;
        public GachaConfigSO GetGachaSO(EquipType equipType)
        {
            if (gachaDic.TryGetValue(equipType, out var configSO))
            {
                return configSO;
            }

            Debug.Log($"{equipType} 타입 SO를 못 찾음");
            return null;
        }


        public int GetCurrentLevel(EquipType equipType)
        {
            if (!TryGetRuntimeData(equipType, out GachaRuntimeData runtime))
                return 1;

            return runtime.currentLevel;
        }



        private bool TryGetRuntimeData(EquipType equipType, out GachaRuntimeData runtime)
        {
            return runtimeDic.TryGetValue(equipType, out runtime);
        }//Type 에 맞는 런타임 데이터 가져오기

    }

}
