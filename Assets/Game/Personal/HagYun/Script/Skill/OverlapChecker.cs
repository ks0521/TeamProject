using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Personal.HagYun
{
    public static class OverlapChecker 
    {
        static readonly Collider2D[] targetCol = new Collider2D[64];
        public static Collider2D[] TargetCol => targetCol;
        public static Collider2D GetTargetCol(int index) => targetCol[index];
        public static int GetCircleTargetsCount(Vector2 pos, float range, LayerMask lm)
        {
            return Physics2D.OverlapCircleNonAlloc(pos, range, targetCol, lm);
        }
        public static int GetCapsuleTargetsCount(Vector2 pos, Vector2 capsuleSize, CapsuleDirection2D dir, LayerMask lm)
        {
            return Physics2D.OverlapCapsuleNonAlloc(pos, capsuleSize, dir, 0, targetCol, lm);
        }
        public static int GetCollderTargetsCount(Collider2D col, LayerMask lm)
        {
            ContactFilter2D filter = new ContactFilter2D() { useLayerMask = true, layerMask = lm, useTriggers = false };
            return Physics2D.OverlapCollider(col, filter, targetCol);
        }
    }
}