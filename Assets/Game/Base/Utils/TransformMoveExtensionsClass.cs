using System.Runtime.CompilerServices;
using UnityEngine;

namespace Base.Utils
{
    public static class TransformMoveExtensionsClass
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToV2(this in Vector3 v) => new Vector2(v.x, v.y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 DirThisToTarget(this in Vector3 thisPos, in Vector3 targetPos, float speed)
        {
            return Vector2.MoveTowards(thisPos.ToV2(), targetPos.ToV2(), speed * Time.deltaTime);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Angle(this in Vector3 v) => (Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg) - 90f;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion LookTarget(this in Vector3 thisPos, in Vector3 targetPos)
        {
            Vector3 dir = targetPos - thisPos;
            dir.z = 0;
            // return Quaternion.LookRotation(Vector3.forward, targetPos.ToV2() - thisPos.ToV2());
            return Quaternion.LookRotation(Vector3.forward, dir);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LookTarget(this Transform thisTrans, in Vector3 targetPos)
        {
            Vector3 dir = targetPos - thisTrans.position;
            thisTrans.rotation = Quaternion.Euler(0, 0, dir.Angle());
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckDirZeroToTarget(this Transform thisTrans, in Vector3 targetPos)
        {
            return thisTrans.position.ToV2() != targetPos.ToV2();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MoveToTarget(this Transform thisTrans, in Vector3 targetPos, float speed)
        {
            thisTrans.position = DirThisToTarget(thisTrans.position, targetPos, speed);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LookToTarget(this Transform thisTrans, in Vector3 targetPos)
        {
            thisTrans.rotation = LookTarget(thisTrans.position, targetPos);
        }
    }
}