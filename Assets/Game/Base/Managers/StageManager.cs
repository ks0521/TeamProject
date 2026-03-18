using System;
using System.Collections.Generic;
using System.Threading;
using Base.Data;
using Base.Save;
using Battle;
using Cysharp.Threading.Tasks;
using Personal.GyuSeong;
using UnityEngine;
using Random = UnityEngine.Random;

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
    public class StageManager : MonoBehaviour
    {
        public int testChapter;
        public int testStage;
        public StageEntry testEntry;
        [SerializeField] private MonsterPoolManager monsterPool; //몬스터 풀
        [SerializeField] private Stage stage; //스테이지 객체
        [SerializeField] private StageSO stageSO; //스테이지 정보
        [SerializeField] private StageProgress stageProgress; //저장된 스테이지 해금 , 현재 스테이지 상태
        [SerializeField] private StageType type; //스테이지의 종류(일반, 도전, 잠김)
        private bool isLoading; //로딩시에만 한시적으로 중복 스테이지 입장 허용
        public event Action<int, int> OnChangeStage;
        public void Init()
        {
            stageProgress = GameDataManager.Instance.GetStageInfo();
            isLoading = true;
            ChangeStage(stageProgress.selectedChapter,stageProgress.selectedStage);
            isLoading = false;
        }

        public List<TestMonster> GetStageMonsters()
        {
            return stage.monstersList;
        }
        /// <summary> 스테이지 변경, 도전 / 일반 스테이지 판별은 이 메서드에서 함</summary>
        public void ChangeStage(int selectedChapter, int selectedStage)
        {
            //현재 도전 스테이지보다 더 나중 스테이지 진행시(잠겨있는 스테이지 입장 시도 시) 오류 처리
            if (selectedChapter > stageProgress.nextChallengeChapter ||
                (selectedChapter == stageProgress.nextChallengeChapter && selectedStage > stageProgress.nextChallengeStage))
            {
                Debug.LogWarning("잠겨있는 스테이지에 접근중입니다");
                return;
            }

            if (selectedChapter == stageProgress.selectedChapter && selectedStage == stageProgress.selectedStage && !isLoading)
            {
                Debug.LogWarning($"{selectedChapter} - {selectedStage}는 이미 진행중인 스테이지입니다. ");
                return;
            }
            if (selectedChapter == stageProgress.nextChallengeChapter && selectedStage == stageProgress.nextChallengeStage)
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
            Debug.Log($"Stage Changed to {selectedChapter} - {selectedStage}");
            stage?.Destroy(); //기존 스테이지 있으면 정리
            monsterPool.ChangeStage(stageSO); // 몬스터풀에 바뀐 스테이지 정보 전달(새 몬스터 생성 위해 필요)
            stage = new Stage(stageSO); // 신규 스테이지 생성
            OnChangeStage?.Invoke(stageSO.chapter,stageSO.stage); // 바뀐 챕터 - 스테이지 정보 전달
            stageProgress = GameDataManager.Instance.StageChanged(stageSO.chapter, stageSO.stage);
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
        
        /// <summary> input chapter - stage 가 base chapter - stage보다 빠른지 느린지 판별</summary>
        /// <returns>input chapter - stage가 base chapter - stage보다 뒤라면 양수, 같다면 0 , 앞이라면 음수</returns>
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
        [SerializeField]public List<TestMonster> monstersList = new(); //현재 스테이지 내 몬스터 리스트
        public IReadOnlyList<TestMonster> MonsterList => monstersList;
        [SerializeField]private StageSO stageSO; //현재 스테이지의 SO
        [SerializeField]private bool canSpawning; //스폰 여부 트리거
        public int CurChapter => stageSO.chapter;
        public int CurStage => stageSO.stage;
        private CancellationTokenSource spawnerToken; //유니태스크 종료 토큰
        private float spawnDelay; // 몬스터 스폰 딜레이
        
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
            spawnDelay = 5f;
            Spawning(spawnerToken.Token).Forget();
            canSpawning = true;
        }
        /// <summary> 실제 몬스터 스폰 비동기 메서드</summary>
        /// <param name="token">종료 토큰</param>
        async UniTaskVoid Spawning(CancellationToken token)
                {
                    try
                    {
                        while (true)
                        {
                            await UniTask.WaitWhile(() => !canSpawning, cancellationToken: token);
                            GameObject mon = MonsterPoolManager.poolDic[stageSO.preset[0].monster.key].UsePool();
                            float randx = Random.Range(-4f, 4f);
                            float randy = Random.Range(-4f, 4f);
                            mon.SetActive(true);
                            mon.transform.position = new Vector3(randx, randy, 0);
                            Register(mon.GetComponent<TestMonster>());
                            await UniTask.Delay(TimeSpan.FromSeconds(spawnDelay), cancellationToken: token);
                            await UniTask.WaitWhile(() => monstersList.Count >= 20, cancellationToken: token);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        Debug.Log("스테이지 전환에 따른 스포너 정상 종료");
                    }
                } 
        
        /// <summary> 새 몬스터 풀에서 꺼내왔을 때 스테이지에서 확인할 수 있게 연결</summary>
        /// <param name="monster"> 꺼내온 몬스터 </param>
        public void Register(TestMonster monster)
        {
            Debug.Log("리스트 내 신규 몬스터 등록");
            
            monstersList.Add(monster);
            monster.OnMonsterKilled += ItemDrop;
        }
        /// <summary> 몬스터가 스테이지에서 사라졌을 시(사망 or 스테이지 변경으로 인한 강제삭제) 스테이지에서 분리</summary>
        /// <param name="monster"> 사라지는 몬스터 </param>
        public void UnRegister(TestMonster monster)
        {
            Debug.Log("리스트 내 몬스터 등록 해제");
            
            monstersList.Remove(monster);
            monster.OnMonsterKilled -= ItemDrop;
        }
        
        /// <summary> 스테이지 변경으로 인한 기존 스테이지 종료시 실행</summary>
        public void Destroy()
        {
            spawnerToken?.Cancel();
            spawnerToken?.Dispose();
            for (int i = monstersList.Count - 1; i >= 0; i--)
            {
                TestMonster monster = monstersList[i];
                monstersList.Remove(monster);
                UnRegister(monster); //해당 몬스터와 연결된 이벤트 삭제
                monster.ForcedReturn(); //모든 몬스터 제거, 초기화
            }
            
            //player.Reset(); // 플레이어 상태(체력, 버프/디버프 등) 초기화
            //AudioManager.StopBattle() //맵 관련 효과음(스킬 효과음 등..) 정지
        }
        /// <summary> 드랍테이블 내 아이템 드랍후 몬스터 스테이지에서 참조 제거</summary>
        /// <param name="monster"></param>
        public void ItemDrop(TestMonster monster)
        {
            List<DropedItem> items = stageSO.dropTable.
                GetDroppedItems(PlayerRuntimeStatus.Instance.finalRewardStatus.itemDropRateBonus);
            Debug.Log($"{items.Count}종 아이템 드랍");
            UnRegister(monster);
        }
    }

    
}