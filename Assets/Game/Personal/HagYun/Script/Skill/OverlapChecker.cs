using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Personal.HagYun
{
    /// <summary> Physics2D의 Overlap Circle/Capsule/Collider2D 를 통해 target 탐지 및 대상을 검출하는 클래스, 
    /// Layer를 통해 target을 검출 </summary>
    public static class OverlapChecker
    {
        static readonly Collider2D[] targetColArr = new Collider2D[64];
        /// <summary> 타겟의 콜라이더를 저장하는 배열 </summary>
        public static Collider2D[] TargetCols => targetColArr;

        /// <summary> GetCircleTargetsCount(Capsule/Collider2D)를 실행하여 탐색된 Collider2D 배열에서 index번 Collider2D 반환 </summary>
        /// <param name="index">대상 index</param>
        /// <returns>index번의 Collider2D 반환 (null이 반환될 수 있음)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Collider2D GetTargetCol(int index) => targetColArr[index];

        /// <summary> Physics2D.OverlapCircleNonAlloc 함수를 이용해 '원형 범위'의 Collider2D 검출 </summary>
        /// <param name="pos">현재 자신의 위치</param>
        /// <param name="range">탐색할 원 영역의 반지름</param>
        /// <param name="lm">대상 Layer</param>
        /// <returns>Collider2D가 검출된 갯수 반환</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetCircleTargetsCount(this Vector2 pos, float range, LayerMask lm)
        {
            return Physics2D.OverlapCircleNonAlloc(pos, range, targetColArr, lm);
        }

        /// <summary> Physics2D.OverlapCapsuleNonAlloc 함수를 이용해 '캡슐 범위'의 Collider2D 검출 </summary>
        /// <param name="pos">현재 자신의 위치</param>
        /// <param name="capsuleSize">탐색할 캡슐 영역의 크기(가로, 세로)</param>
        /// <param name="dir">캡슐의 긴 부위의 방향(Horizontal : 가로)(Vertical : 세로), 
        /// capsuleSize의 큰 값과 방향이 일치되어야 함</param>
        /// <param name="lm">대상 Layer</param>
        /// <returns>Collider2D가 검출된 갯수 반환</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetCapsuleTargetsCount(this Vector2 pos, Vector2 capsuleSize, CapsuleDirection2D dir, LayerMask lm)
        {
            return Physics2D.OverlapCapsuleNonAlloc(pos, capsuleSize, dir, 0, targetColArr, lm);
        }

        // Physics2D.OverlapCollider의 인수 filter에 사용될 구조체
        static ContactFilter2D filter = new ContactFilter2D() { useLayerMask = true, layerMask = 0, useTriggers = false };
        /// <summary> Physics2D.OverlapCollider 함수를 이용해 '대상 Collider2D 범위'의 Collider2D 검출 </summary>
        /// <param name="col">다른 Collider2D를 탐색할 Collider2D</param>
        /// <param name="lm">대상 Layer</param>
        /// <returns>Collider2D가 검출된 갯수 반환</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetCollderTargetsCount(this Collider2D col, LayerMask lm)
        {
            filter.layerMask = lm;
            return Physics2D.OverlapCollider(col, filter, targetColArr);
        }

        /// <summary> 범용 버전_현재 위치를 기준으로, 매개변수 Collider2D 배열의 요소를 순회하여 
        /// 가장 가까운 거리의 target을 검출하는 함수 </summary>
        /// <param name="thisPos">현재 자신의 위치</param>
        /// <param name="colArr">순회할 Collider2D 배열, 연속으로 배치되어 있어야 함</param>
        /// <param name="cnt">순회할 배열에 담긴 Collider2D 요소 갯수</param>
        /// <param name="targetCol">검출되었을 시 return할 대상 Collider2D</param>
        /// <returns>targetCol이 return되었는지 여부를 return</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetNearTarget(this Vector2 thisPos, Collider2D[] colArr, int cnt, out Collider2D targetCol)
        {
            if (cnt <= 0 || colArr == null)
            {
                targetCol = null;
                return false;
            }
            else if (cnt == 1)
            {
                targetCol = colArr[0];
                return targetCol != null;
            }
            float minDis = ((Vector2)colArr[0].transform.position - thisPos).sqrMagnitude;
            int targetNum = 0;
            for (int i = 1; i < cnt; i++)
            {
                Collider2D col = colArr[i];
                if (col == null) continue;
                Vector2 colPos = col.transform.position;
                float curDis = (colPos - thisPos).sqrMagnitude;
                if (curDis < minDis)
                {
                    minDis = curDis;
                    targetNum = i;
                }
            }
            targetCol = colArr[targetNum];
            return true;
        }

        /// <summary> 기본 버전_현재 위치를 기준으로, OverlapChecker class 내부 Collider2D 배열의 요소를 순회하여 
        /// 가장 가까운 거리의 target을 검출하는 함수 </summary>
        /// <param name="thisPos">현재 자신의 위치</param>
        /// <param name="cnt">순회할 배열에 담긴 Collider2D 요소 갯수</param>
        /// <param name="targetCol">검출되었을 시 return할 대상 Collider2D</param>
        /// <returns>targetCol이 return되었는지 여부를 return</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetNearTarget(this Vector2 thisPos, int cnt, out Collider2D targetCol)
        {
            return TryGetNearTarget(thisPos, targetColArr, cnt, out targetCol);
        }

        /// <summary> 범용 버전_현재 Transform을 기준으로, 매개변수 Collider2D 배열의 요소를 순회하여 
        /// 가장 가까운 거리의 target을 검출하는 함수 </summary>
        /// <param name="thisTrans">현재 자신의 Transform</param>
        /// <param name="colArr">순회할 Collider2D 배열, 연속으로 배치되어 있어야 함</param>
        /// <param name="cnt">순회할 배열에 담긴 Collider2D 요소 갯수</param>
        /// <param name="targetCol">검출되었을 시 return할 대상 Collider2D</param>
        /// <returns>targetCol이 return되었는지 여부를 return</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetNearTarget(this Transform thisTrans, Collider2D[] colArr, int cnt, out Collider2D targetCol)
        {
            return TryGetNearTarget(thisTrans.position, colArr, cnt, out targetCol);
        }

        /// <summary> 기본 버전_현재 Transform을 기준으로, OverlapChecker class 내부 Collider2D 배열의 요소를 순회하여 
        /// 가장 가까운 거리의 target을 검출하는 함수 </summary>
        /// <param name="thisTrans">현재 자신의 Transform</param>
        /// <param name="cnt">순회할 배열에 담긴 Collider2D 요소 갯수</param>
        /// <param name="targetCol">검출되었을 시 return할 대상 Collider2D</param>
        /// <returns>targetCol이 return되었는지 여부를 return</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetNearTarget(this Transform thisTrans, int cnt, out Collider2D targetCol)
        {
            return TryGetNearTarget(thisTrans, targetColArr, cnt, out targetCol);
        }

        /// <summary> 범용 버전_현재 Transform을 기준으로, 매개변수 Collider2D 배열 갯수 이내로 
        /// OverlapCircleNonAlloc 을 통해 타겟 탐색 후,
        /// 가장 가까운 거리의 target을 검출하는 함수 </summary>
        /// <param name="thisTrans">현재 자신의 Transform</param>
        /// <param name="range">탐색할 원 영역의 반지름</param>
        /// <param name="lm">대상 Layer</param>
        /// <param name="colArr">순회할 Collider2D 배열</param>
        /// <param name="targetCol">검출되었을 시 return할 대상 Collider2D</param>
        /// <returns>targetCol이 return되었는지 여부를 return</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetNearTargetByCircle(this Transform thisTrans, float range, LayerMask lm, Collider2D[] colArr, out Collider2D targetCol)
        {
            Vector2 thisPos = thisTrans.position;
            int cnt = GetCircleTargetsCount(thisPos, range, lm);
            return TryGetNearTarget(thisPos, colArr, cnt, out targetCol);
        }

        /// <summary> 기본 버전_현재 Transform을 기준으로, OverlapChecker class 내부 Collider2D 배열 갯수 이내로 
        /// OverlapCircleNonAlloc 을 통해 타겟 탐색 후,
        /// 가장 가까운 거리의 target을 검출하는 함수 </summary>
        /// <param name="thisTrans">현재 자신의 Transform</param>
        /// <param name="range">탐색할 원 영역의 반지름</param>
        /// <param name="lm">대상 Layer</param>
        /// <param name="targetCol">검출되었을 시 return할 대상 Collider2D</param>
        /// <returns>targetCol이 return되었는지 여부를 return</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetNearTargetByCircle(this Transform thisTrans, float range, LayerMask lm, out Collider2D targetCol)
        {
            return TryGetNearTargetByCircle(thisTrans, range, lm, targetColArr, out targetCol);
        }

        /// <summary> 범용 버전_현재 Transform을 기준으로, 매개변수 Collider2D 배열 갯수 이내로 
        /// OverlapCapsuleNonAlloc 을 통해 타겟 탐색 후,
        /// 가장 가까운 거리의 target을 검출하는 함수 </summary>
        /// <param name="thisTrans">현재 자신의 Transform</param>
        /// <param name="capsuleSize">탐색할 캡슐 영역의 크기(가로, 세로)</param>
        /// <param name="dir">캡슐의 긴 부위의 방향(Horizontal : 가로)(Vertical : 세로), 
        /// capsuleSize의 큰 값과 방향이 일치되어야 함</param>
        /// <param name="lm">대상 Layer</param>
        /// <param name="colArr">순회할 Collider2D 배열</param>
        /// <param name="targetCol">검출되었을 시 return할 대상 Collider2D</param>
        /// <returns>targetCol이 return되었는지 여부를 return</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetNearTargetByCapsule(this Transform thisTrans, Vector2 capsuleSize, CapsuleDirection2D dir, 
        LayerMask lm, Collider2D[] colArr, out Collider2D targetCol)
        {
            Vector2 thisPos = thisTrans.position;
            int cnt = GetCapsuleTargetsCount(thisPos, capsuleSize, dir, lm);
            return TryGetNearTarget(thisPos, colArr, cnt, out targetCol);
        }

        /// <summary> 기본 버전_현재 Transform을 기준으로, OverlapChecker class 내부 Collider2D 배열 갯수 이내로 
        /// OverlapCapsuleNonAlloc 을 통해 타겟 탐색 후,
        /// 가장 가까운 거리의 target을 검출하는 함수 </summary>
        /// <param name="thisTrans">현재 자신의 Transform</param>
        /// <param name="capsuleSize">탐색할 캡슐 영역의 크기(가로, 세로)</param>
        /// <param name="dir">캡슐의 긴 부위의 방향(Horizontal : 가로)(Vertical : 세로), 
        /// capsuleSize의 큰 값과 방향이 일치되어야 함</param>
        /// <param name="lm">대상 Layer</param>
        /// <param name="targetCol">검출되었을 시 return할 대상 Collider2D</param>
        /// <returns>targetCol이 return되었는지 여부를 return</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetNearTargetByCapsule(this Transform thisTrans, Vector2 capsuleSize, CapsuleDirection2D dir, 
        LayerMask lm, out Collider2D targetCol)
        {
            return TryGetNearTargetByCapsule(thisTrans, capsuleSize, dir, lm, targetColArr, out targetCol);
        }

        /// <summary> 범용 버전_현재 Transform을 기준으로, 매개변수 Collider2D 배열 갯수 이내로 
        /// OverlapCollider 를 통해 타겟 탐색 후,
        /// 가장 가까운 거리의 target을 검출하는 함수 </summary>
        /// <param name="col">다른 Collider2D를 탐색할 Collider2D</param>
        /// <param name="lm">대상 Layer</param>
        /// <param name="colArr">순회할 Collider2D 배열</param>
        /// <param name="targetCol">검출되었을 시 return할 대상 Collider2D</param>
        /// <returns>targetCol이 return되었는지 여부를 return</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetNearTargetByCollider2D(this Collider2D col, LayerMask lm, Collider2D[] colArr,
        out Collider2D targetCol)
        {
            int cnt = GetCollderTargetsCount(col, lm);
            return TryGetNearTarget(col.transform.position, colArr, cnt, out targetCol);
        }

        /// <summary> 기본 버전_현재 Transform을 기준으로, OverlapChecker class 내부 Collider2D 배열 갯수 이내로 
        /// OverlapCollider 를 통해 타겟 탐색 후,
        /// 가장 가까운 거리의 target을 검출하는 함수 </summary>
        /// <param name="col">다른 Collider2D를 탐색할 Collider2D</param>
        /// <param name="lm">대상 Layer</param>
        /// <param name="targetCol">검출되었을 시 return할 대상 Collider2D</param>
        /// <returns>targetCol이 return되었는지 여부를 return</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetNearTargetByCollider2D(this Collider2D col, LayerMask lm, out Collider2D targetCol)
        {
            return TryGetNearTargetByCollider2D(col, lm, targetColArr, out targetCol);
        }


        
        /// <summary> 현재 위치를 기준으로, 매개변수 T List 배열의 요소를 순회하여 
        /// 가장 가까운 거리의 target을 검출하는 함수 </summary>
        /// <param name="thisPos">현재 자신의 위치</param>
        /// <param name="targetList">순회할 MonoBehaviour List 배열</param>
        /// <param name="target">검출되었을 시 반환될 대상 MonoBehaviour</param>
        /// <returns>MonoBehaviour가 반환되었는지 여부를 반환</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetNearTargetByList<T>(this Vector2 thisPos, List<T> targetList, out T target) where T : MonoBehaviour
        {
            target = null;
            if (targetList == null) return false;
            int cnt = targetList.Count;
            if (cnt <= 0) return false;
            else if (cnt == 1)
            {
                target = targetList[0];
                return target != null;
            }
            float minDis = ((Vector2)targetList[0].transform.position - thisPos).sqrMagnitude;
            int targetNum = 0;
            for (int i = 1; i < cnt; i++)
            {
                T tTarget = targetList[i];
                Vector2 tTargetPos = tTarget.transform.position;
                float curDis = (tTargetPos - thisPos).sqrMagnitude;
                if (curDis < minDis)
                {
                    minDis = curDis;
                    targetNum = i;
                }
            }
            target = targetList[targetNum];
            return true;
        }

        /// <summary> 현재 Transform을 기준으로, 매개변수 T List 배열의 요소를 순회하여 
        /// 가장 가까운 거리의 target을 검출하는 함수 </summary>
        /// <param name="thisTrans">현재 자신의 Transform</param>
        /// <param name="targetList">순회할 MonoBehaviour List 배열</param>
        /// <param name="target">검출되었을 시 반환될 대상 MonoBehaviour</param>
        /// <returns>MonoBehaviour가 반환되었는지 여부를 반환</returns>
        public static bool TryGetNearTargetByList<T>(this Transform thisTrans, List<T> targetList, out T target) where T : MonoBehaviour
        {
            return TryGetNearTargetByList(thisTrans, targetList, out target);
        }
    }
}