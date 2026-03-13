using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class Monster : Character
    {
        public MonsterSO monsterSo;
        protected override void FixedUpdateFeat()
        {

        }

        protected override void UpdateFeat()
        {

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
    }
}