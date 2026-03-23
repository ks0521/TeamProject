using System;
using System.Collections.Generic;
using System.Threading;
using Base.Data;
using Base.Managers;
using Battle;
using Cysharp.Threading.Tasks;
using Personal.GyuSeong;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
    public class Stage
    {
        [SerializeField] private MonsterPoolManager monsterPool;
        [SerializeField] private StageSO stageSO; //현재 스테이지의 SO
        public List<TestMonster> monstersList = new(); //현재 스테이지 내 몬스터 리스트
        private CancellationTokenSource spawnerToken; //유니태스크 종료 토큰
        public bool canSpawning; //스폰 여부 트리거(디버깅용)
        [SerializeField] private float spawnDelay; // 몬스터 스폰 딜레이
        private BoxCollider2D spawnArea;
        public event Action<TestMonster> OnMonsterKilled;
        //초기화
        public Stage(StageSO stage, MonsterPoolManager monsterPool, BoxCollider2D spawnArea)
        {
            //바꾸려는 챕터와 스테이지의 정보를 SO에서 얻어옴
            this.monsterPool = monsterPool;
            this.spawnArea = spawnArea;
            stageSO = stage;
            spawnDelay = 2f;
            Debug.Log($"Chapter.{stageSO.stage} Stage {stageSO.chapter} 초기화");
        }
        //실제 스테이지 시작 지점
        public void Enter()
        {
            Debug.Log($"Chapter.{stageSO.stage} Stage {stageSO.chapter} 시작");
            spawnerToken = new CancellationTokenSource();
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
                    float randx = Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x);
                    float randy = Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y);

                    GameObject mon = monsterPool.UsePool(stageSO.preset[0].monster.key);
                    mon.SetActive(true);
                    mon.transform.position = new Vector3(randx, randy, 0);
                    Register(mon.GetComponent<TestMonster>());
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

        /// <summary> 새 몬스터 풀에서 꺼내왔을 때 스테이지에서 확인할 수 있게 연결</summary>
        /// <param name="monster"> 꺼내온 몬스터 </param>
        private void Register(TestMonster monster)
        {
            //Debug.Log("리스트 내 신규 몬스터 등록");

            monstersList.Add(monster);
            monster.OnMonsterKilled += MonsterKilled;
        }

        /// <summary> 몬스터 처치시 스테이지 내부 처리부</summary>
        /// <param name="monster"></param>
        private void MonsterKilled(TestMonster monster)
        {
            //스테이지 클리어 등 작업전에 몬스터 반환먼저 하기
            UnRegister(monster);
            OnMonsterKilled?.Invoke(monster); 
        }
        
        /// <summary> 몬스터가 스테이지에서 사라졌을 시(사망 or 스테이지 변경으로 인한 강제삭제) 스테이지에서 분리</summary>
        /// <param name="monster"> 사라지는 몬스터 </param>
        private void UnRegister(TestMonster monster)
        {
            //Debug.Log("리스트 내 몬스터 등록 해제");

            monstersList.Remove(monster);
            monsterPool.ReturnPool(monster.monsterSO.key, monster.gameObject);
            monster.OnMonsterKilled -= MonsterKilled;
        }

        /// <summary> 스테이지 변경으로 인한 기존 스테이지 종료시 실행</summary>
        public void Destroy()
        {
            canSpawning = false;
            spawnerToken?.Cancel();
            spawnerToken?.Dispose();
            for (int i = monstersList.Count - 1; i >= 0; i--)
            {
                TestMonster monster = monstersList[i];
                monstersList.Remove(monster);
                monster.OnMonsterKilled -= MonsterKilled; //해당 몬스터와 연결된 이벤트 삭제
                monster.ForcedReturn(); //모든 몬스터 제거, 초기화
            }

            //player.Reset(); // 플레이어 상태(체력, 버프/디버프 등) 초기화
            //AudioManager.StopBattle() //맵 관련 효과음(스킬 효과음 등..) 정지
        }

    }
