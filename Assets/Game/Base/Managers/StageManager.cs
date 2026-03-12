using System;
using System.Collections.Generic;
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
        [SerializeField] private ScriptableObjectHub hub;
        void Awake()
        {
            
        }

        public void Init()
        {
            stageInfo = GameDataManager.Instance.GetStageInfo();
            ChangeStage();
        }
        public void ChangeStage()
        {
            stage?.Destroy();
            stage = new Stage(stageInfo.selectedChapter, stageInfo.selectedStage);
        }
    }

    public class Stage
    {
        private List<Monster> monstersList = new();

        public Stage(int selectedChapter, int selectedStage)
        {
            Debug.Log($"Chapter.{selectedChapter} Stage {selectedStage} 진입");
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