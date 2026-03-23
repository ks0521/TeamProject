using Base.Data;
using Base.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using Base.Save;
using UnityEngine;

namespace Base.Managers
{
    public interface IGameSystem
    {
        int GetOrder();
        //규성 : 0~ 99 / 학윤님 : 100~199 / 관규님 : 200 ~ 299 / 종준님 : 300~399
    }

    public interface IManager : IGameSystem
    {
        void Init();
    }

    [Serializable]
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        private List<IGameSystem> gameSystems;
        private Dictionary<Type, IGameSystem> dic = new();
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
            gameSystems = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IGameSystem>().ToList();
            gameSystems.Sort((x, y) => x.GetOrder().CompareTo(y.GetOrder()));
            foreach (IGameSystem gameSystem in gameSystems)
            {
                dic.Add(gameSystem.GetType(), gameSystem);
                if (gameSystem is not IManager manager)
                {
                    Debug.Log($"{gameSystem} 시스템 추가");
                    continue;
                }
                Debug.Log($"{manager} 초기화");
                manager.Init();
            }
        }

        /// <summary> IGameSystem 붙은 컴포넌트 가져오기</summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>찾으려는 컴포넌트</returns>
        public T GetGameSystem<T>() where T : IGameSystem
        {
            if (dic.TryGetValue(typeof(T),out var system))
            {
                return (T)system;
            }
            Debug.LogWarning($"찾으려는 {typeof(T)}타입은 존재하지 않습니다. ");
            return default;
        }
    }
}