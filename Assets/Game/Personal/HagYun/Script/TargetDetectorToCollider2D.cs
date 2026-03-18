using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Personal.HagYun
{
    public class TargetDetectorUsingCircleCollider2D : MonoBehaviour
    {
        HashSet<Collider2D> detectedTarget = new HashSet<Collider2D>();
        public bool IsDetectedTarget => detectedTarget.Count > 0;
        public int DetectedTargetCnt => detectedTarget.Count;
        [SerializeField] LayerMask lm;
        [SerializeField] CircleCollider2D col;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if ((lm.value & (1 << collision.gameObject.layer)) != 0)
            {
                detectedTarget.Add(collision);
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            detectedTarget.Remove(collision);
        }
        public void SetCollider(CircleCollider2D col) => this.col = col;
        public void TargetCheckToUpdate(bool isAtking = false)
        {
            if (IsDetectedTarget)
            {
                //if (Time.frameCount % 10 != 0) return; // 10프레임 일 때만 통과
                //if (isAtking) return; // isAtking이 false일 때만 통과
                detectedTarget.RemoveWhere(e => e == null || !e.gameObject.activeInHierarchy);
            }
        }
        public void ColliderRadiusChange(float radius)
        {
            col.radius = radius;
        }
    }
}