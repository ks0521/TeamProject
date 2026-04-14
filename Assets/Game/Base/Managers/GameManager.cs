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
            //초기화 및 도감추가만 우선적으로 시행
            gameSystems = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include,FindObjectsSortMode.None).OfType<IGameSystem>().ToList();
            foreach (IGameSystem gameSystem in gameSystems)
            {
                Type type = gameSystem.GetType();

                if (dic.ContainsKey(type))
                {
                    Debug.LogError($"중복 IGameSystem 타입 감지: {type.Name} / 새 오브젝트: {((MonoBehaviour)gameSystem).name}");
                    continue;
                }

                dic.Add(type, gameSystem);
            }
        }

        private void Start()
        {
            //시작시 IManager붙은 컴포넌트 전부 찾고 GetOrder순 정렬
            gameSystems.Sort((x, y) => x.GetOrder().CompareTo(y.GetOrder()));
            foreach (IGameSystem gameSystem in gameSystems)
            {
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
                return (T)system; //define
            }
            Debug.LogWarning($"찾으려는 {typeof(T)}타입은 존재하지 않습니다. ");
            return default;
        }
        //변수대입 + 존재여부 확인용
        public bool TryGetGameSystem<T>(out T variable) where T : IGameSystem 
        {
            if (dic.TryGetValue(typeof(T),out var system))
            {
                variable = (T)system;
                return true; //define
            }
            Debug.LogWarning($"찾으려는 {typeof(T)}타입은 존재하지 않습니다. ");
            variable = default(T);
            return false;
        }
    }
}