using System;
using System.Collections.Generic;
using System.Threading;
using Base.Data;
using Base.Managers;
using Battle;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
    public class Stage
    {
        public bool canSpawning; //스폰 여부 트리거(디버깅용)
        public event Action<Monster> OnMonsterKilledInStage;
        public List<Monster> monstersList = new(); //현재 스테이지 내 몬스터 리스트
        
        [SerializeField] private MonsterPoolManager monsterPool;
        [SerializeField] private StageSO stageSO; //현재 스테이지의 SO
        [SerializeField] private float spawnDelay; // 몬스터 스폰 딜레이
        
        private BoxCollider2D spawnArea;
        private CancellationTokenSource spawnerToken; //유니태스크 종료 토큰
        private EventHub eventHub;
        #region 스테이지 시작
        public Stage(StageSO stage, MonsterPoolManager monsterPool, BoxCollider2D spawnArea)
        {
            eventHub = GameManager.Instance.GetGameSystem<EventHub>();
            monsterPool.ChangeStage(stage);
            this.monsterPool = monsterPool;
            this.spawnArea = spawnArea;
            stageSO = stage;
            canSpawning = false;
            SpawnTypeSelect(stageSO.spawnType);
            Debug.Log($"Chapter.{stageSO.stage} Stage {stageSO.chapter} 초기화");
        }

        void SpawnTypeSelect(SpawnType spawnType)
        {
            switch (spawnType)
            {
                case SpawnType.Endless:
                    spawnerToken = new CancellationTokenSource();
                    EndlessSpawn(spawnerToken.Token).Forget();
                    spawnDelay = 1f;
                    break;
                case SpawnType.Boss:
                    BossSpawn();
                    break;
                case SpawnType.Wave:

                    break;
            }
        }
        //실제 스테이지 시작 지점
        public void Enter()
        {
            canSpawning = true;
            Debug.Log($"Chapter.{stageSO.stage} Stage {stageSO.chapter} 시작");
        }
        #endregion
        #region 스폰 타입

        
        void BossSpawn()
        {
            float randx = Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x);
            float randy = Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y);

            GameObject monsterObj = monsterPool.UsePool(stageSO.preset[0].monster.key);
            Monster monster = monsterObj.GetComponent<Monster>();
            monster.SetUp(stageSO.preset[0].monster);
            monster.Init();
            ((Boss)monster).InitBoss();
            monsterObj.transform.position = new Vector3(randx, randy, 0);
            monsterObj.SetActive(true);
            eventHub.BossSpawned(monster);
            Register(monster);
        }
        /// <summary> 실제 몬스터 스폰 비동기 메서드</summary>
        /// <param name="token">종료 토큰</param>
        async UniTaskVoid EndlessSpawn(CancellationToken token)
        {
            try
            {
                while (true)
                {
                    await UniTask.WaitWhile(() => !canSpawning, cancellationToken: token);
                    float randx = Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x);
                    float randy = Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y);
                    int randIdx = WeightCalc(stageSO.preset);
                    GameObject monsterObj = monsterPool.UsePool(stageSO.preset[randIdx].monster.key);
                    Monster monster = monsterObj.GetComponent<Monster>();
                    monster.SetUp(stageSO.preset[randIdx].monster);
                    monster.Init();
                    monsterObj.transform.position = new Vector3(randx, randy, 0);
                    monsterObj.SetActive(true);
                    Register(monster);
                    //Debug.Log($"Spawn : {mon.transform.position}");
                    await UniTask.Delay(TimeSpan.FromSeconds(spawnDelay), cancellationToken: token);
                    await UniTask.WaitWhile(() => monstersList.Count >= 10, cancellationToken: token);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log("스테이지 전환에 따른 스포너 정상 종료");
            }
        }
        #endregion
        #region 몬스터 등록 / 해제
        /// <summary> 새 몬스터 풀에서 꺼내왔을 때 스테이지에서 확인할 수 있게 연결</summary>
        /// <param name="monster"> 꺼내온 몬스터 </param>
        private void Register(Monster monster)
        {
            //Debug.Log("리스트 내 신규 몬스터 등록");
            
            monstersList.Add(monster);
            monster.OnMonsterKilled += MonsterKilled;
        }

        /// <summary> 몬스터 처치시 스테이지 내부 처리부</summary>
        /// <param name="monster"></param>
        private void MonsterKilled(Monster monster)
        {
            //스테이지 클리어 등 작업전에 몬스터 반환먼저 하기
            UnRegister(monster);
            monsterPool.ReturnPool(monster.monsterSO.key, monster.gameObject);
            OnMonsterKilledInStage?.Invoke(monster); 
        }
        
        /// <summary> 몬스터가 스테이지에서 사라졌을 시(사망 or 스테이지 변경으로 인한 강제삭제) 스테이지에서 분리</summary>
        /// <param name="monster"> 사라지는 몬스터 </param>
        private void UnRegister(Monster monster)
        {
            //Debug.Log("리스트 내 몬스터 등록 해제");

            monstersList.Remove(monster);
            monster.OnMonsterKilled -= MonsterKilled;
        }
        #endregion
        #region 스테이지 관리
        /// <summary> 스테이지 내 몬스터 정리 </summary>
        public void Clear()
        {
            for(int i= monstersList.Count - 1 ; i>=0 ; i--)
            {
                monsterPool.ReturnPool(monstersList[i].monsterSO.key,monstersList[i].gameObject);
                UnRegister(monstersList[i]);
            }
        }
        /// <summary> 스테이지 변경으로 인한 기존 스테이지 종료시 실행</summary>
        public void Destroy()
        {
            canSpawning = false;
            spawnerToken?.Cancel();
            spawnerToken?.Dispose();
            for (int i = monstersList.Count - 1; i >= 0; i--)
            {
                Monster monster = monstersList[i];
                UnRegister(monster);
                monster.ForcedDelete(); //모든 몬스터 제거, 초기화
            }

            //player.Reset(); // 플레이어 상태(체력, 버프/디버프 등) 초기화
            //AudioManager.StopBattle() //맵 관련 효과음(스킬 효과음 등..) 정지
        }
        #endregion
        int WeightCalc(in List<MonsterPreset> presets)
                          {
                              int total = 0;
                              foreach (var preset in presets)
                              {
                                  if (preset.weights <= 0)
                                  {
                                      Debug.LogWarning($"가중치가 0보다 작은 값({preset.weights}) 입력됨");
                                      return 0;
                                  }
                                  total += preset.weights;
                              }
                              if (total <= 0)
                              {
                                  Debug.LogWarning($"가중합이 0미만입니다({total}");
                                  return 0;
                              }
                  
                              int weightSum = Random.Range(0, total);
                              for (int i = 0; i < presets.Count; i++)
                              {
                                  if (presets[i].weights > weightSum)
                                  {
                                      //Debug.Log($"{i}번째 {presets[i].monster.name} 사용");
                                      return i;// rarity
                                  }
                                  
                                  weightSum -= presets[i].weights;
                              }
                              
                              Debug.LogWarning("가중합 계산 오류");
                              return 0;
                          }
    }