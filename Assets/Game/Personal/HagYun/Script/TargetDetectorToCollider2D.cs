using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Personal.HagYun
{
    public class TargetDetectorUsingCircleCollider2D : MonoBehaviour
    {
        HashSet<Collider2D> detectedTarget = new HashSet<Collider2D>(200);
        public bool IsDetectedTarget => detectedTarget.Count > 0;
        public int DetectedTargetCnt => detectedTarget.Count;
        public int detectedTargetCnt;
        [SerializeField] LayerMask lm = 1 << 8;
        [SerializeField] CircleCollider2D col;
        bool isDontDetect;
        public void Init(CircleCollider2D col)
        {
            isDontDetect = false;
            SetCollider(col);
        }
        public void SetCollider(CircleCollider2D col) => this.col = col;
        public void ColliderRadiusChange(float range)
        {
            col.radius = range;
        }
        public void ToggleDetecte() => isDontDetect = !isDontDetect;
        private void OnDestroy()
        {
            detectedTarget.Clear();
            detectedTarget = null;
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (isDontDetect) return;
            if ((lm.value & (1 << collision.gameObject.layer)) != 0)
            {
                detectedTarget.Add(collision);
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (isDontDetect) return;
            detectedTarget.Remove(collision);
        }
        private void Update()
        {
            if (!IsDetectedTarget) return;
            else if (isDontDetect) return;
            detectedTargetCnt = DetectedTargetCnt;
            //if (Time.frameCount % 10 != 0) return; // 10프레임 일 때만 통과
            detectedTarget.RemoveWhere(e => e == null || !e.gameObject.activeInHierarchy);
        }
        public int GetDetectedTarget(Collider2D[] arr)
        {
            if (!IsDetectedTarget || arr == null) return -1;
            int cnt = Mathf.Min(detectedTarget.Count, arr.Length);
            detectedTarget.CopyTo(arr, 0, cnt);
            return cnt;
        }
        static Collider2D[] saveDetectCol = new Collider2D[64];
        public int GetDetectedTarget(List<Collider2D> list)
        {
            if (!IsDetectedTarget || list == null) return -1;
            if (list.Count > 0) list.Clear();
            int cnt = GetDetectedTarget(saveDetectCol);
            for(int i = 0; i < saveDetectCol.Length; i++)
            {
                if (saveDetectCol[i] == null) break;
                list.Add(saveDetectCol[i]);
            }
            return cnt;
        }
    }
}