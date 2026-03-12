using System.Collections;
using System.Collections.Generic;
using Battle;
using UnityEngine;

namespace Base.Managers
{
    /// <summary> 스테이지 전환, 상태관리 , 초기화 담당</summary>
    public class StageManager : MonoBehaviour
    {
        [SerializeField] private int curStage;
        [SerializeField] private int curChapter;
        [SerializeField] private StageSO stageSO;
        // Start is called before the first frame update
        void Start()
        {
        }

        public void Init()
        {
            
        }
        public void ChangeStage()
        {
            
        }
    }
}