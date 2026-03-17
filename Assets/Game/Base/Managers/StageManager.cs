using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Base.Data;
using Base.Save;
using Battle;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Base.Managers
{
    [Serializable]
    /// <summary> UI에서 사용하기 위한 정보 모음집</summary>
    public struct StageEntry
    {
        public int chapter;
        public int stage;
        public StageSO stageSO;
        public StageType type;
    }
    /// <summary> 스테이지 전환, 상태관리 , 초기화 담당</summary>
    public class JJ_StageManager : MonoBehaviour
    {
        public int testChapter;
        public int testStage;
        public StageEntry testEntry;
        [SerializeField] private MonsterPoolManager monsterPool;
        [SerializeField] private Stage stage;
        [SerializeField] private StageSO stageSO;
        [SerializeField] private StageInfo stageInfo;
        [SerializeField] private StageType type;

        public void Init()
        {
            stageInfo = JJ_GameDataManager.Instance.GetStageInfo();
            ChangeStage(stageInfo.selectedChapter,stageInfo.selectedStage);
        }
        public void ChangeStage(int selectedChapter, int selectedStage)
        {
            //현재 도전 스테이지보다 더 나중 스테이지 진행시(잠겨있는 스테이지 입장 시도 시) 오류 처리
            if (selectedChapter > stageInfo.nextChallengeChapter ||
                (selectedChapter == stageInfo.nextChallengeChapter && selectedStage > stageInfo.nextChallengeStage))
            {
                Debug.LogWarning("잠겨있는 스테이지에 접근중입니다. ");
                return;
            }
            stage?.Destroy();
            if (selectedChapter == stageInfo.nextChallengeChapter && selectedStage == stageInfo.nextChallengeStage)
            {
                stageSO = GameData.StageDB.GetSO(selectedChapter, selectedStage, StageType.Challenge);
            }
            else
            {
                stageSO = GameData.StageDB.GetSO(selectedChapter, selectedStage, StageType.Normal);
            }
            Debug.Log($"{selectedChapter} {selectedStage}");
            monsterPool.ChangeStage(stageSO);
            stage = new Stage(stageSO);
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
            int compare = CompareStage(selectedChapter, selectedStage, 
                stageInfo.nextChallengeChapter, stageInfo.nextChallengeStage);
            if (compare < 0)
            {
                //일반스테이지
                entry.type = StageType.Normal;
                entry.stageSO = GameData.StageDB.GetSO(selectedChapter, selectedStage, StageType.Normal);
            }
            else if (compare == 0)
            {
                //도전 스테이지
                entry.type = StageType.Challenge;
                entry.stageSO = GameData.StageDB.GetSO(selectedChapter, selectedStage, StageType.Challenge);
            }
            else
            {
                //잠긴 스테이지
                entry.type = StageType.Locked;
                entry.stageSO = GameData.StageDB.GetSO(selectedChapter, selectedStage, StageType.Normal);
            }
            return entry;
        }
        public int CompareStage(int inputChapter, int inputStage, int baseChapter, int baseStage)
        {
            if (inputChapter != baseChapter)
                return inputChapter.CompareTo(baseChapter);
            return inputStage.CompareTo(baseStage);
        }
    }
    [Serializable]
    public class Stage
    {
        public event Action<int, int> OnChangeStage;
        [SerializeField]private List<TestMonster> monstersList = new();
        [SerializeField]private StageSO stageSO;
        [SerializeField]private bool canSpawning;
        private CancellationTokenSource spawnerToken;

        public void Register(TestMonster monster)
        {
            Debug.Log("리스트 내 신규 몬스터 등록");
            monstersList.Add(monster);
            monster.OnMonsterKilled += ItemDrop;
        }
        public void UnRegister(TestMonster monster)
        {
            Debug.Log("리스트 내 사망 몬스터 해제");
            monstersList.Remove(monster);
            monster.OnMonsterKilled -= ItemDrop;
        }
        
        public Stage(StageSO stage)
        {
            //바꾸려는 챕터와 스테이지의 정보를 SO에서 얻어옴
            stageSO = stage;
            if (stageSO == null)
            {
                Debug.LogWarning("StageManager : StageSO를 가져오지 못했습니다");
                return;
            }
            Debug.Log($"Chapter.{stageSO.stage} Stage {stageSO.chapter} 진입");
            spawnerToken = new CancellationTokenSource();
            Spawning(0.5f, spawnerToken.Token).Forget();
            canSpawning = true;
            OnChangeStage?.Invoke(stageSO.chapter,stageSO.stage);
        }

        async UniTaskVoid Spawning(float delay, CancellationToken token)
        {
            try
            {
                while (true)
                {
                    await UniTask.WaitWhile(() => !canSpawning, cancellationToken: token);
                    GameObject mon = MonsterPoolManager.poolDic[stageSO.preset[0].monster.key].UsePool();
                    mon.SetActive(true);
                    mon.transform.position = Vector3.zero;
                    Register(mon.GetComponent<TestMonster>());
                    await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: token);
                    await UniTask.WaitWhile(() => monstersList.Count >= 20, cancellationToken: token);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log("스테이지 전환에 따른 스포너 정상 종료");
            }
        } 
        public void Destroy()
        {
            spawnerToken?.Cancel();
            spawnerToken?.Dispose();
            foreach (var monster in monstersList)
            {
                monster.ForcedReturn(); //모든 몬스터 제거, 초기화
            }
            //player.Reset(); // 플레이어 상태(체력, 버프/디버프 등) 초기화
            //AudioManager.StopBattle() //맵 관련 효과음(스킬 효과음 등..) 정지
        }
        //챌린지 모드 전용 함수 추가(제한시간)
        //
        public void ItemDrop(TestMonster monster)
        {
            List<DropedItem> items = stageSO.dropTable.
                GetDroppedItems(PlayerRuntimeStatus.Instance.finalRewardStatus.itemDropRateBonus);
            Debug.Log($"{items.Count}종 아이템 드랍");
            UnRegister(monster);
        }
    }

    
}