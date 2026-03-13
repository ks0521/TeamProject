using System;
using System.Collections;
using System.Collections.Generic;
using Base.Data;
using UnityEngine;
namespace Battle
{
    public class Monster : Character
    {
        private float hp;
        public float Hp
        {
            get => hp;
            private set
            {
                hp = value;
                if (hp <= 0f)
                {
                    Killed();
                }
            }
        }
        public const float ApproachStopRange = 0.15f;
        public MonsterSO monsterSo;
        public BattleStat stat;
        public Transform player;
        private void Awake()
        {
            stat = monsterSo.battleStat;
        }
        /// <summary> 풀에서 꺼내질때 초기화</summary>
        private void OnEnable()
        {
            Init();
        }

        private void Init()
        {
            if (monsterSo == null)
            {
                Debug.LogWarning("몬스터 SO가 삽입되지 않았습니다!");
                return;
            }
            //몬스터 스폰 시작하기전에 무조건 플레이어 활성화 되어있어야 함
            player = GameObject.FindGameObjectWithTag("Player").transform;
            if (player == null)
            {
                Debug.LogWarning("플레이어 태그 찾을 수 없음");
            }
            stat = monsterSo.battleStat;
            hp = stat.maxHp;
        }
        /// <summary> 플레이어에게 처치당했을 시 실행</summary>
        public void Killed()
        {
            Debug.Log("몬스터 처치됨");   
        }
        /// <summary>스테이지 변경등의 이유로 사라질 때 실행</summary>
        public void ForcedReturn()
        {
            Debug.Log("오브젝트 풀에 강제 반환");
        }

        protected override void UpdateFeat()
        {
            throw new NotImplementedException();
        }

        private void FixedUpdate()
        {
            if (player is null) 
                return;
            
            ChasePlayer();
        }

        protected override void FixedUpdateFeat()
        {
            throw new NotImplementedException();
        }

        public void ChasePlayer()
        {
            if(Vector2.Distance(transform.position,player.transform.position) > ApproachStopRange)
            transform.position = Vector2.MoveTowards(transform.position,player.transform.position,1 * Time.deltaTime);
        }
    }
}