using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Personal.HagYun
{
    public static class OverlapChecker
    {
        static readonly Collider2D[] targetCols = new Collider2D[64];
        public static Collider2D[] TargetCols => targetCols;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Collider2D GetTargetCol(int index) => targetCols[index];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetCircleTargetsCount(Vector2 pos, float range, LayerMask lm)
        {
            return Physics2D.OverlapCircleNonAlloc(pos, range, targetCols, lm);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetCapsuleTargetsCount(Vector2 pos, Vector2 capsuleSize, CapsuleDirection2D dir, LayerMask lm)
        {
            return Physics2D.OverlapCapsuleNonAlloc(pos, capsuleSize, dir, 0, targetCols, lm);
        }
        static ContactFilter2D filter = new ContactFilter2D() { useLayerMask = true, layerMask = 0, useTriggers = false };
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetCollderTargetsCount(Collider2D col, LayerMask lm)
        {
            filter.layerMask = lm;
            return Physics2D.OverlapCollider(col, filter, targetCols);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetNearTarget(Vector2 thisPos, Collider2D[] colArr, int cnt, out Collider2D targetCol)
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
            float minDis = (colArr[0].transform.position.ToV2() - thisPos).sqrMagnitude;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetNearTarget(Vector2 thisPos, int cnt, out Collider2D targetCol)
        {
            return TryGetNearTarget(thisPos, targetCols, cnt, out targetCol);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetNearTarget(this Transform thisTrans, Collider2D[] colArr, int cnt, out Collider2D targetCol)
        {
            return TryGetNearTarget(thisTrans.position.ToV2(), colArr, cnt, out targetCol);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetNearTarget(this Transform thisTrans, int cnt, out Collider2D targetCol)
        {
            return TryGetNearTarget(thisTrans, targetCols, cnt, out targetCol);
        }
    }
}