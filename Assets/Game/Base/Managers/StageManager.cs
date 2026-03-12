using System;
using System.Collections.Generic;
using Base.Data;
using Base.Save;
using Battle;
using UnityEngine;

namespace Base.Managers
{
    
    /// <summary> 스테이지 전환, 상태관리 , 초기화 담당</summary>
    public class StageManager : MonoBehaviour
    {
        [SerializeField] private int curStage;
        [SerializeField] private int curChapter;
        [SerializeField] private StageInfo stageInfo;
        [SerializeField] private Stage stage;
        void Awake()
        {
            
        }

        public void Init()
        {
            stageInfo = GameDataManager.Instance.GetStageInfo();
            ChangeStage(stageInfo.selectedChapter,stageInfo.selectedStage);
        }
        public void ChangeStage(int selectedChapter, int selectedStage)
        {
            stage?.Destroy();
            stage = new Stage(selectedChapter, selectedStage);
        }
    }

    public class Stage
    {
        private List<Monster> monstersList = new();
        public Stage(int chapter, int stage)
        {
            //바꾸려는 챕터와 스테이지의 정보를 SO에서 얻어옴
            StageSO stageSO = GameData.StageDB.GetSO(chapter, stage,StageType.Normal);
            Debug.Log($"Chapter.{stageSO.stage} Stage {stageSO.chapter} 진입");
        }
        
        public void Destroy()
        {
            foreach (var monster in monstersList)
            {
                monster.ForcedReturn();
            }
            
        }
        //챌린지 모드 전용 함수 추가(제한시간)
        //
    }

}