using Base.Save;
using UnityEngine;

namespace Base.Managers
{
    /// <summary> 게임 시작 시 데이터 불러오기 + GameSaveData 관리</summary>
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance;
        public GameSaveData curData = new();
        
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }
            Instance = this;
            Init();
        }
        private void Init()
        {
            Load();
            //기타 작업 시작
        }
        /// <summary> 데이터 저장하기 </summary>
        public void Save()
        {
            
        }

        /// <summary> 데이터 불러오기 </summary>
        public void Load()
        {
            //해당 값을 저장된 json에서 받아오게 변경
        }
    }
}