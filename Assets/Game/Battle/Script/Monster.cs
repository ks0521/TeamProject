using System;
using System.Collections;
using System.Collections.Generic;
using Base.Data;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Battle
{
    public enum CharacterState
    {
        Idle, Move, Attack
    }

    public class Monster : Character
    {
        public MonsterSO monsterSO;
        public const float MonsterAttackRange = 0.6f;
        public override BattleStat CurrentBattleStat => monsterSO.battleStat;
        protected override float AttackRange => MonsterAttackRange;
        public const float ApproachStopRange = 0.15f;
        public event Action<float, float> OnMonsterHpChanged; //내부이벤트로 허브등록 X
        public CharacterState state;
        public event Action<Monster> OnMonsterKilled;
        //public Transform player;


        protected override void Init()
        {
            state = CharacterState.Idle;
            base.Init();
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.GetComponent<Character>();
                targetTransform = target.transform;
            }
        }

        /// <summary>스테이지 변경등의 이유로 사라질 때 실행</summary>
        public void ForcedReturn()
        {
            //현재는 구현할 필요 없습니다. 
            Debug.Log("오브젝트 강제 정리");
            Destroy(gameObject);
        }

        /// <summary> 플레이어에게 처치당했을 시 실행</summary>
        protected override void OnDead()
        {
            if (isDead) //여러번 죽지 않게하기
                return;
            isDead = true;
            Debug.Log("몬스터 처치됨");
            OnMonsterKilled?.Invoke(this);
        }

        protected override void UpdateFeat()
        {
        }

        public override void Hit(float damage)
        {
            base.Hit(damage);
            OnMonsterHpChanged?.Invoke(Hp, CurrentBattleStat.maxHp);
        }

        private void FixedUpdate()
        {
            if (isDead || target is null)
                return;
        }

        protected override void FixedUpdateFeat()
        {
        }

        // public void ChasePlayer()
        // {
        //     if(Vector2.Distance(transform.position, target.transform.position) > ApproachStopRange)
        //         transform.position = Vector2.MoveTowards(transform.position,target.transform.position, CurrentBattleStat.moveSpeed * Time.deltaTime);
        // }
    }
}