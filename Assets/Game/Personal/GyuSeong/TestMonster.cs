using System;
using Base.Data;
using Battle;
using UnityEngine;

namespace Personal.GyuSeong
{
    public class TestMonster : TestCharacter
    {
        public MonsterSO monsterSO;
        public const float MonsterAttackRange = 0.6f;
        protected override BattleStat CurrentBattleStat => monsterSO.battleStat;
        protected override float AttackRange => MonsterAttackRange;
        public event Action<TestMonster> OnMonsterKilled;
        public bool TestDead;
        /// <summary> 플레이어 피격에 의한 정상적 사망일때 실행</summary>
        protected override void OnDead()
        {
            if (isDead) //여러번 죽지 않게하기
                return;
            isDead = true;
            Debug.Log("몬스터 처치됨");
            OnMonsterKilled?.Invoke(this);
        }
        /// <summary>스테이지 변경등의 이유로 사라질 때 실행</summary>
        public void ForcedReturn()
        {
            //현재는 구현할 필요 없습니다. 
            Debug.Log("오브젝트 강제 정리");
            Destroy(gameObject);
        }

        //오브젝트 풀에서 나올때 실행
        protected override void Init()
        {
            TestDead = false;
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                target = playerObj.transform;
        }

        protected override void UpdateFeat()
        {
        }

        protected override void FixedUpdateFeat()
        {
            if (TestDead) OnDead(); //테스트용
        }

        public void Action()
        {
            //공격 거리 이상이면 이동
            //공격거리 이하면 공격(character1 부분 Attack 이용)
        }
        //추가사항으로, 보스 몬스터 공격 3가지 
        //1. 보스 중심으로 일정 범위 경고 후 데미지
        //2. 스킬 시전 시 플레이어 위치기준 일정 범위 경고 후 데미지
        //3. 플레이어 위치 기준 일정범위 경고 후 돌진
    }
}