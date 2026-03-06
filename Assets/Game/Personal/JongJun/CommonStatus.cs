using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HJJ
{
    //플레이어와 몬스터가 공유하는 변수들
    [CreateAssetMenu(fileName = "CommonStatus", menuName = "ScriptableObject/Common Status")]
    public class CommonStatus : ScriptableObject
    {
        private bool isDead;
        private int hp;
        private int maxHp;
        private float atk;
        private float atkSpeed;
        private float moveSpeed;
        private float criticalAtkRate;
        private float criticalDamageRate;
    }
}
