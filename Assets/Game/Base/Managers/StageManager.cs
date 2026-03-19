using System;
using System.Collections.Generic;
using Base.Data;
using Base.Save;
using Battle;
using Personal.GyuSeong;
using UnityEngine;

namespace Base.Managers
{
    [Serializable]
    /// <summary> UI에서 사용하기 위한 정보 모음집</summary>
    public struct StageEntry
    {
        public int chapter; //챕터
        public int stage; //스테이지
        public StageSO stageSO; //해당 챕터 - 스테이지의 SO
        public StageType type; //스테이지의 도전상태(일반, 도전, 잠금)
    }

    /// <summary> 스테이지 전환, 상태관리 , 초기화 담당</summary>
    public class StageManager : MonoBehaviour, IManager
    {
        [Header("디버그용")] public int testChapter;
        public int testStage;
        public StageEntry testEntry;
        public bool BlockSpawning; //테스트용으로 몬스터 스폰 없이 테스트만 하고싶을때 활성화
        [Header("실사용")] private StageRule stageRule;

        private RuntimeProgressState Progress => PlayerProgressManager.Instance.progress;
        [SerializeField] private int curChapter = 0;
        [SerializeField] private int curStage = 0;
        [SerializeField] private MonsterPoolManager monsterPool; //몬스터 풀
        [SerializeField] private Stage stage; //스테이지 객체
        [SerializeField] private StageSO stageSO; //스테이지 정보
        [SerializeField] private StageProgress stageProgress; //저장된 스테이지 해금 , 현재 스테이지 상태
        [SerializeField] private StageType type; //스테이지의 종류(일반, 도전, 잠김)
        public event Action<StageSO> OnChangeStage;

        public void Init()
        {
            stageProgress = GetStageProgress();
            ChangeStage(stageProgress.selectedNormalChapter, stageProgress.selectedNormalStage);
        }

        public int GetOrder() => 2;
        public List<TestMonster> GetStageMonsters() => stage.monstersList;

        /// <summary>스테이지 변경(도전 / 일반 / 잠김 스테이지 판별은 이 메서드에서 진행)</summary>
        /// <param name="selectedChapter"> 변경하려는 챕터</param>
        /// <param name="selectedStage">변경하려는 스테이지</param>
        public void ChangeStage(int selectedChapter, int selectedStage)
        {
            //현재 도전 스테이지보다 더 나중 스테이지 진행시(잠겨있는 스테이지 입장 시도 시) 오류 처리
            if (selectedChapter > stageProgress.nextChallengeChapter ||
                (selectedChapter == stageProgress.nextChallengeChapter &&
                 selectedStage > stageProgress.nextChallengeStage))
            {
                Debug.LogWarning("잠겨있는 스테이지에 접근중입니다");
                return;
            }

            if (curChapter == selectedChapter && curStage == selectedStage)
            {
                Debug.LogWarning($"{selectedChapter} - {selectedStage}는 이미 진행중인 스테이지입니다. ");
                return;
            }

            if (selectedChapter == stageProgress.nextChallengeChapter &&
                selectedStage == stageProgress.nextChallengeStage)
            {
                stageSO = GameData.StageDB.GetSO(selectedChapter, selectedStage, StageType.Challenge);
            }
            else
            {
                stageSO = GameData.StageDB.GetSO(selectedChapter, selectedStage, StageType.Normal);
            }

            if (stageSO is null)
            {
                Debug.LogWarning($"{selectedChapter}-{selectedStage}SO를 불러오지 못해 스테이지를 바꿀 수 없습니다. ");
                return;
            }

            curChapter = selectedChapter;
            curStage = selectedStage;
            Debug.Log($"Stage Changed to {selectedChapter} - {selectedStage}");
            stage?.Destroy(); //기존 스테이지 있으면 정리
            stageRule?.Destroy();

            OnChangeStage?.Invoke(stageSO); // 바뀐 챕터 - 스테이지 정보 전달
            monsterPool.ChangeStage(stageSO); // 몬스터풀에 바뀐 스테이지 정보 전달(새 몬스터 생성 위해 필요)
            stage = new Stage(stageSO, monsterPool); // 신규 스테이지 생성
            if (stageSO.type == StageType.Normal)
            {
                stageProgress = SelectNormalStage(stageSO.chapter, stageSO.stage);
                stageRule = new NormalStageRule(stageSO);
                stage.OnMonsterKilled += stageRule.MonsterKilled;
            }
            else if (stageSO.type == StageType.Challenge || stageSO.type == StageType.Boss)
            {
                stageRule = new ChallengeStageRule(stageSO);
                stage.OnMonsterKilled += stageRule.MonsterKilled;
                ((ChallengeStageRule)stageRule).ChallengeSuccess += OnChallengeSucceeded;
            }

            stage.Enter();
            stageRule.Enter();
            if (BlockSpawning)
            {
                stage.canSpawning = false;
                Debug.Log("스테이지 적 스폰 비활성화됨");
            }
        }

        /// <summary> 특정 챕터 - 스테이지의 상태를 확인</summary>
        /// <param name="selectedChapter">찾는 챕터</param>
        /// <param name="selectedStage">찾는 스테이지</param>
        /// <returns>해당 챕터 - 스테이지의 SO 및 타입(일반, 도전, 잠금)</returns>
        public StageEntry GetStageEntry(int selectedChapter, int selectedStage)
        {
            StageEntry entry = new()
            {
                chapter = selectedChapter,
                stage = selectedStage,
            };
            // 알고싶은 챕터가 현재 최고 스테이지보다 앞인지 뒤인지 확인
            int compare = CompareStage(selectedChapter, selectedStage,
                stageProgress.nextChallengeChapter, stageProgress.nextChallengeStage);
            //일반 스테이지
            if (compare < 0)
            {
                entry.type = StageType.Normal;
                entry.stageSO = GameData.StageDB.GetSO(selectedChapter, selectedStage, StageType.Normal);
            }
            //도전 스테이지 판단
            else if (compare == 0)
            {
                //도전 스테이지
                entry.type = StageType.Challenge;
                entry.stageSO = GameData.StageDB.GetSO(selectedChapter, selectedStage, StageType.Challenge);
            }
            //잠긴 스테이지
            else
            {
                //잠긴 스테이지
                entry.type = StageType.Locked;
                entry.stageSO = GameData.StageDB.GetSO(selectedChapter, selectedStage, StageType.Normal);
            }

            return entry;
        }

        public StageProgress GetStageProgress()
        {
            return new StageProgress()
            {
                selectedNormalChapter = Progress.stage.selectedNormalChapter,
                selectedNormalStage = Progress.stage.selectedNormalStage,
                nextChallengeChapter = Progress.stage.nextChallangeChapter,
                nextChallengeStage = Progress.stage.nextChallangeStage
            };
        }

        /// <summary> 노말 스테이지 변경 </summary>
        /// <returns> 변경된 런타임 스테이지 데이터</returns>
        private StageProgress SelectNormalStage(int changeChapter, int changeStage)
        {
            Progress.stage.selectedNormalChapter = changeChapter;
            Progress.stage.selectedNormalStage = changeStage;
            PlayerProgressManager.Instance.SaveProgress();
            return new StageProgress()
            {
                selectedNormalChapter = Progress.stage.selectedNormalChapter,
                selectedNormalStage = Progress.stage.selectedNormalStage,
                nextChallengeChapter = Progress.stage.nextChallangeChapter,
                nextChallengeStage = Progress.stage.nextChallangeStage
            };
        }

        private StageProgress ProgressChallengeStage(StageSO clearStage)
        {
            //보스를 잡았으면 다음 챕터 2-1
            if (clearStage.type == StageType.Boss)
            {
                Progress.stage.nextChallangeChapter = clearStage.chapter + 1;
                Progress.stage.nextChallangeStage = 2;
            }
            //아니면 스테이지 +1만
            else
            {
                Progress.stage.nextChallangeStage++;
            }

            PlayerProgressManager.Instance.SaveProgress();
            return new StageProgress()
            {
                selectedNormalChapter = Progress.stage.selectedNormalChapter,
                selectedNormalStage = Progress.stage.selectedNormalStage,
                nextChallengeChapter = Progress.stage.nextChallangeChapter,
                nextChallengeStage = Progress.stage.nextChallangeStage
            };
        }
        private void Start()
        {
            Debug.Log("F1 : 스테이지 진입 테스트 / F2 : 스테이지 엔트리 테스트 ");
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                Debug.Log($"스테이지 진입 테스트 : {testChapter}, {testStage}");
                ChangeStage(testChapter, testStage);
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                testEntry = GetStageEntry(testChapter, testStage);
                Debug.Log($"스테이지 엔트리 테스트");
            }
        }

        private void OnChallengeSucceeded(StageSO clearStage)
        {
            Debug.Log("스테이지 클리어, 클리어 기록이 저장됩니다. ");
            //stageProgress = PlayerProgressManager.Instance.ProgressChallengeStage(clearStage); <- 디버그 목적으로 막아놓음
            Debug.Log("직전 사냥했던 일반스테이지로 돌아갑니다.");
            ChangeStage(stageProgress.selectedNormalChapter, stageProgress.selectedNormalStage);
        }


        /// <summary> input chapter - stage 가 base chapter - stage보다 빠른지 느린지 판별</summary>
        /// <returns>input chapter - stage가 base chapter - stage보다 뒤라면 양수, 같다면 0 , 앞이라면 음수</returns>
        private int CompareStage(int inputChapter, int inputStage, int baseChapter, int baseStage)
        {
            if (inputChapter != baseChapter)
                return inputChapter.CompareTo(baseChapter);
            return inputStage.CompareTo(baseStage);
        }
    }
}