using System;
using System.Collections.Generic;
using Base.Data;
using Base.Save;
using Battle;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Base.Managers
{
    /// <summary>
    /// UI에서 사용하기 위한 정보 모음집
    /// </summary>
    [Serializable]
    public struct StageEntry
    {
        public int chapter; //챕터
        public int stage; //스테이지
        public StageSO stageSO; //해당 챕터 - 스테이지의 SO
        public StageType type; //스테이지의 도전상태(일반, 도전, 잠금)
    }
    
    public struct ChallengeUIData
    {
        public float currentTime;
        public float maxTime;
        public int currentKill;
        public int targetKill;
    }
    /// <summary> 스테이지 전환, 상태관리 , 초기화 담당</summary>
    public class StageManager : MonoBehaviour, IManager
    {
        public bool BlockSpawning; //특정 이벤트로 몬스터 스폰 막고싶은 때 사용
        public bool BlockProceed; //특정 이벤트로 스테이지 변경 막고싶을 때 사용
        
        public StageSO CurrentStageSo => currentStageSO;
        public List<Monster> Monsters => stage.monstersList; //현재 스테이지에 있는 몬스터 리스트를 반환
        [SerializeField] private BoxCollider2D spawnArea; //몬스터 스폰 공간
        
        private int curChapter; //현재 진행중인 챕터
        private int curStage; //현재 진행중인 스테이지
        private bool isStageResultProcessing; //현재 스테이지 진행여부 플래그
        private bool isRebirthProcessing; //일반 스테이지 사망 후 부활 처리 중복 방지
        private RuntimeProgressData progress; //축약용 프로퍼티
        private ProgressManager progressManager;
        private MonsterPoolManager monsterPool; //몬스터 풀
        private GameDataDictionaries datahub;
        private EventHub eventHub; //이벤트 허브
        private Stage stage; //스테이지 객체
        private StageRule stageRule; // 현재 진행중인 스테이지 규약
        private StageSO currentStageSO; // 현재 진행중인 스테이지 정보
        private StageProgress stageProgress; //현재 진행중인 스테이지와 최대 도달 스테이지 묶음
        private StageType type; //스테이지의 종류(일반, 도전, 잠김)
        
        public void Init()
        {
            eventHub = GameManager.Instance.GetGameSystem<EventHub>();
            monsterPool = GameManager.Instance.GetGameSystem<MonsterPoolManager>();
            datahub = GameManager.Instance.GetGameSystem<GameDataDictionaries>();
            progressManager = GameManager.Instance.GetGameSystem<ProgressManager>();
            stageProgress = GetStageProgress();
            ChangeStage(stageProgress.selectedNormalChapter, stageProgress.selectedNormalStage);
            
            eventHub.OnDeadPlayer += OnPlayerDie;
        }

        private void OnDestroy()
        {
            eventHub.OnDeadPlayer -= OnPlayerDie;
        }

        public int GetOrder() => 10;
        /// <summary>스테이지 변경(도전 / 일반 / 잠김 스테이지 판별은 이 메서드에서 진행)</summary>
        /// <param name="selectedChapter"> 변경하려는 챕터</param>
        /// <param name="selectedStage">변경하려는 스테이지</param>
        public void ChangeStage(int selectedChapter, int selectedStage)
        {
            Debug.Log($"ChangeStage 호출 / {selectedChapter}-{selectedStage} / frame:{Time.frameCount}");

            //현재 도전 스테이지보다 더 나중 스테이지 진행시(잠겨있는 스테이지 입장 시도 시) 오류 처리
            if (selectedChapter > stageProgress.nextChallengeChapter ||
                (selectedChapter == stageProgress.nextChallengeChapter &&
                 selectedStage > stageProgress.nextChallengeStage))
            {
                Debug.LogWarning("잠겨있는 스테이지에 접근중입니다");
                return;
            }
            //이미 진행중인 스테이지 진입시도 거절
            if (curChapter == selectedChapter && curStage == selectedStage)
            {
                Debug.LogWarning($"{selectedChapter} - {selectedStage}는 이미 진행중인 스테이지입니다. ");
                return;
            }

            isStageResultProcessing = false;
            //기존 스테이지에서 연결한 이벤트 제거
            
            if (stage != null)
            {
                stage.OnMonsterKilledInStage -= stageRule.MonsterKilledInStage;
                stage.OnMonsterKilledInStage -= MonsterKillChain;
            }
            if (stageRule is ChallengeStageRule oldChallengeRule)
            {
                oldChallengeRule.ChallengeSuccess -= OnChallengeSucceeded;
                oldChallengeRule.ChallengeFail -= OnChallengeFailed;
            }
            
            //입장하려는 스테이지가 도전 스테이지
            if (selectedChapter == stageProgress.nextChallengeChapter &&
                selectedStage == stageProgress.nextChallengeStage)
            {
                currentStageSO = datahub.stageTable.GetSO(selectedChapter, selectedStage, StageType.Boss);
                if (currentStageSO == null)
                    currentStageSO = datahub.stageTable.GetSO(selectedChapter, selectedStage, StageType.Challenge);
            }
            else
            {
                currentStageSO = datahub.stageTable.GetSO(selectedChapter, selectedStage, StageType.Normal);
            }

            if (currentStageSO is null)
            {
                Debug.LogWarning($"{selectedChapter}-{selectedStage}SO를 불러오지 못해 스테이지를 바꿀 수 없습니다. ");
                return;
            }
            
            //기존 스테이지 정리 + 새 스테이지 & 스테이지 룰 생성
            stage?.Destroy(); //기존 스테이지 있으면 정리
            curChapter = selectedChapter;
            curStage = selectedStage;
            eventHub.StageChanged(currentStageSO); // 바뀐 챕터 - 스테이지 정보 전달
            //monsterPool.ChangeStage(currentStageSO); // 몬스터풀 설정은 stage에서 관리
            stage = new Stage(currentStageSO, monsterPool, spawnArea); // 신규 스테이지 생성
            switch (currentStageSO.clearType)
            {
                case ClearType.None:
                    stageProgress = SelectNormalStage(currentStageSO.chapter, currentStageSO.stage);
                    stageRule = new NormalStageRule(currentStageSO);
                    break;
                case ClearType.KillCount:
                    stageRule = new KillCount(currentStageSO);
                    break;
                case ClearType.BossKill:
                    stageRule = new BossKill(currentStageSO);
                    break;
                case ClearType.Survival:
                    break;
            }
            if (stageRule is ChallengeStageRule challengeRule)
            {
                challengeRule.ChallengeSuccess += OnChallengeSucceeded;
                challengeRule.ChallengeFail += OnChallengeFailed;
            }
            stage.OnMonsterKilledInStage += stageRule.MonsterKilledInStage;
            stage.OnMonsterKilledInStage += MonsterKillChain;

            //스테이지와 스테이지 룰 다 초기화된 이후 시작
            stage.Enter();
            stageRule.Enter();
            if (BlockSpawning)
            {
                stage.canSpawning = false;
                Debug.Log("스테이지 적 스폰 비활성화됨");
            }
            eventHub.StageChangeClear(currentStageSO);
        }

        /// <summary> 특정 챕터 - 스테이지의 상태를 확인</summary>
        /// <param name="selectedChapter">찾는 챕터</param>
        /// <param name="selectedStage">찾는 스테이지</param>
        /// <returns>해당 챕터 - 스테이지의 SO 및 타입(일반, 도전, 잠금)</returns>
        public StageEntry GetStageEntry(int selectedChapter, int selectedStage)
        {
            StageEntry entry = new() { chapter = selectedChapter, stage = selectedStage, };
            // 알고싶은 챕터가 현재 최고 스테이지보다 앞인지 뒤인지 확인
            int compare = CompareStage(selectedChapter, selectedStage,
                stageProgress.nextChallengeChapter, stageProgress.nextChallengeStage);
            //일반 스테이지
            if (compare < 0)
            {
                entry.type = StageType.Normal;
                entry.stageSO = datahub.stageTable.GetSO(selectedChapter, selectedStage, StageType.Normal);
            }
            //도전 / 보스 스테이지
            else if (compare == 0)
            {
                entry.stageSO = datahub.stageTable.GetSO(selectedChapter, selectedStage, StageType.Boss);
                Debug.Log(entry.stageSO);
                if (entry.stageSO != null)
                {
                    entry.type = StageType.Boss;
                }
                else
                {
                    entry.stageSO = datahub.stageTable.GetSO(selectedChapter, selectedStage, StageType.Challenge);
                    entry.type = StageType.Challenge;
                }
            }
            //잠긴 스테이지
            else
            {
                entry.type = StageType.Locked;
                entry.stageSO = datahub.stageTable.GetSO(selectedChapter, selectedStage, StageType.Normal);
            }
            return entry;
        }

        public StageProgress GetStageProgress() => new StageProgress()
            {
                selectedNormalChapter = progressManager.progress.stage.selectedNormalChapter,
                selectedNormalStage = progressManager.progress.stage.selectedNormalStage,
                nextChallengeChapter = progressManager.progress.stage.nextChallangeChapter, //현재 도전 챕터(최대 진행 챕터)
                nextChallengeStage = progressManager.progress.stage.nextChallangeStage//현재 도전 스테이지(최대 진행 스테이지)
            };

        public bool TryGetTarget(Transform finder,out Monster target)
        {
            float minDist = Single.MaxValue;
            float dist;
            if (Monsters is null || Monsters.Count == 0)
            {
                target = null;
                return false;
            }

            target = Monsters[0]; // 일단 버그 방지
            foreach (var monster in Monsters)
            {
                if (monster.IsDead) continue;
                dist = Vector3.SqrMagnitude(monster.transform.position - finder.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    target = monster;
                }
            }
            //모든 타겟이 죽어있을 때의 예외값 처리
            if (target == null || target.IsDead) 
            {
                target = null;
                return false;
            }
            return true;
        }

        // public bool TryGetTarget(int count, out IReadOnlyList<Monster> targets)
        // {
        //     
        // }
        /// <summary> 노말 스테이지 변경 </summary>
        /// <returns> 변경된 런타임 스테이지 데이터</returns>
        private StageProgress SelectNormalStage(int changeChapter, int changeStage)
        {
            progressManager.StageProgress.selectedNormalChapter = changeChapter;
            progressManager.StageProgress.selectedNormalStage = changeStage;
            progressManager.SaveProgress();
            return new StageProgress()
            {
                selectedNormalChapter = progressManager.StageProgress.selectedNormalChapter,
                selectedNormalStage = progressManager.StageProgress.selectedNormalStage,
                nextChallengeChapter = progressManager.StageProgress.nextChallangeChapter,
                nextChallengeStage = progressManager.StageProgress.nextChallangeStage
            };
        }

        private StageProgress ProgressChallengeStage(StageSO clearStage)
        {
            if (!BlockProceed)
            {
                //보스를 잡았으면 다음 챕터 2-1
                if (clearStage.stageType == StageType.Boss)
                {
                    progressManager.StageProgress.nextChallangeChapter = clearStage.chapter + 1;
                    progressManager.StageProgress.nextChallangeStage = 2;
                }
                //아니면 스테이지 +1만
                else
                {
                    progressManager.StageProgress.nextChallangeStage++;
                }
            }
            progressManager.SaveProgress();
            return new StageProgress()
            {
                selectedNormalChapter = progressManager.StageProgress.selectedNormalChapter,
                selectedNormalStage = progressManager.StageProgress.selectedNormalStage,
                nextChallengeChapter = progressManager.StageProgress.nextChallangeChapter,
                nextChallengeStage = progressManager.StageProgress.nextChallangeStage
            };
        }
        /// <summary> 스테이지 몬스터 스폰 멈추기</summary>
        private void StopCurrentStage()
        {
            if (stage != null)
            {
                stage.canSpawning = false;
                stage.Clear();
            }
            stageRule?.Destroy();
            //stageRule = null;
        }
        private void OnChallengeSucceeded()
        {
            Debug.Log("스테이지 클리어 시도");
            if (isStageResultProcessing) return;
            isStageResultProcessing = true;

            StopCurrentStage();
            DelayClear(3f, this.GetCancellationTokenOnDestroy()).Forget();
        }
        async UniTaskVoid DelayClear(float delay, CancellationToken token)
        {
            Debug.Log("스테이지 클리어, 클리어 기록이 저장됩니다. ");
            stageProgress = ProgressChallengeStage(currentStageSO); 
            
            eventHub.StageCleared(currentStageSO);
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: token);
            Debug.Log("직전 사냥했던 일반스테이지로 돌아갑니다.");
            ChangeStage(stageProgress.selectedNormalChapter, stageProgress.selectedNormalStage);
        }
        
        private void OnChallengeFailed()
        {
            if (isStageResultProcessing) return;
            isStageResultProcessing = true;

            StopCurrentStage();
            DelayFail(3f, this.GetCancellationTokenOnDestroy()).Forget();
        }
        async UniTaskVoid DelayFail(float delay, CancellationToken token)
        {
            Debug.Log("스테이지 실패, 이전 스테이지로 돌아갑니다. ");
            eventHub.StageFailed(currentStageSO);
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: token);
            Debug.Log("직전 사냥했던 일반스테이지로 돌아갑니다.");
            ChangeStage(stageProgress.selectedNormalChapter, stageProgress.selectedNormalStage);
        }
        /// <summary> 일반 스테이지에서 플레이어 죽었을 때 부활과정 </summary>
        /// <param name="character"></param>
        void OnPlayerDie(Character character)
        {
            if (currentStageSO == null) return;
            Debug.Log("StageMaanger : 플레이어 사망");
            if (currentStageSO.stageType == StageType.Normal)
            {
                DelayRebirth(3f, this.GetCancellationTokenOnDestroy()).Forget();
                return;
            }

            OnChallengeFailed();
        }
        /// <summary> 일반스테이지 부활 딜레이</summary>
        async UniTaskVoid DelayRebirth(float delay, CancellationToken token)
        {
            Debug.Log($"스테이지 초기화, {delay}초 후 몬스터가 다시 생성됩니다. ");
            stage.canSpawning = false;
            stage.Clear();
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: token);
            stage.canSpawning = true;
        }
        public bool TryGetChallengeData(out ChallengeUIData data)
        {
            data = default;
            if (stageRule is not ChallengeStageRule challengeStageRule)
            {
                return false;
            }

            data.currentKill = challengeStageRule.KillScore;
            data.targetKill = currentStageSO.targetKillScore;
            data.currentTime = challengeStageRule.RemainTime;
            data.maxTime = currentStageSO.deadLine;
            return true;
        }

        void MonsterKillChain(Monster monster) => eventHub.MonsterKill(monster.monsterSO);
        
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