using System;
using System.Collections.Generic;
using System.Linq;
using Base.Save;
using UnityEngine;

namespace Base.Managers
{
    public interface IManager
    {
        public abstract void Init();
        public abstract int GetOrder();
        //규성 : 0~ 99 / 학윤님 : 100~199 / 관규님 : 200 ~ 299 / 종준님 : 300~399
    }
    [Serializable]
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        private List<IManager> managers;
        [SerializeField] private StageManager stageManager;
        private void Awake()
        {
            //첫 시작시 실행
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        private void Start()
        {
            //시작시 IManager붙은 컴포넌트 전부 찾고 GetOrder순 정렬
            managers = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IManager>().ToList();
            managers.Sort((x,y) => x.GetOrder().CompareTo(y.GetOrder()));
            foreach (var manager in managers)
            {
                Debug.Log($"{manager} 초기화");
                manager.Init();
            }
        }
        /// <summary> IManager붙은 스크립트 찾기 </summary>
        /// <typeparam name="T"> 찾고싶은 매니저 스크립트</typeparam>
        /// <returns> 해당 매니저 스크립트</returns>
        public T GetManager<T>() where T : IManager
        {
            return managers.OfType<T>().FirstOrDefault();
        }
        //버그 : 스탯 강화시 최종 레벨을 넘어가서 강화할 수 있는 현상(ex. 공격속도 강화 max = 200, 180레벨에서 *100 시 최대레벨을 돌파한
        //280레벨 달성)
    }
}