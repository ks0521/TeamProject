using System;
using System.Collections;
using System.Collections.Generic;
using Base.Data;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Battle
{
    public class Monster : Character
    {
        public MonsterSO monsterSO;
        public const float MonsterAttackRange = 0.6f;
        protected override BattleStat CurrentBattleStat => monsterSO.battleStat;
        protected override float AttackRange => MonsterAttackRange;
        public const float ApproachStopRange = 0.15f;
        //public Transform player;


        protected override void AwakeInit()
        {
            base.AwakeInit();
        }
        protected override void OnEnableInit()
        {
            if (monsterSO == null)
            {
                Debug.LogWarning("몬스터 SO가 삽입되지 않았습니다!");
                return;
            }
            //몬스터 스폰 시작하기전에 무조건 플레이어 활성화 되어있어야 함
            //player = GameObject.FindGameObjectWithTag("Player").transform;
            target = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            //if (player == null)
            if (target == null)
            {
                Debug.LogWarning("플레이어 태그 찾을 수 없음");
            }
            base.OnEnableInit();
        }
        protected override void StartInit()
        {
            base.StartInit();

        }
        /// <summary> 플레이어에게 처치당했을 시 실행</summary>
        protected override void OnDead()
        {
            if (isDead) //여러번 죽지 않게하기
                return;
            isDead = true;
            Debug.Log("몬스터 처치됨");
            cEvent.RaiseDead(this);
        }
        /// <summary>스테이지 변경등의 이유로 사라질 때 해당 프리팹 삭제</summary>
        public void ForcedReturn()
        {
            Debug.Log("오브젝트 풀에 강제 반환");
            Destroy(gameObject);
        }
        protected override void UpdateFeat()
        {
            
        }

        private void FixedUpdate()
        {
            if (isDead || target is null) 
                return;
        }

        protected override void FixedUpdateFeat()
        {

        }

        public void ChasePlayer()
        {
            if(Vector2.Distance(transform.position, target.transform.position) > ApproachStopRange)
                transform.position = Vector2.MoveTowards(transform.position,target.transform.position, CurrentBattleStat.moveSpeed * Time.deltaTime);
        }
    }
}