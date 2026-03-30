using Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Base.Managers
{
    public class ItemPoolManager : MonoBehaviour, IGameSystem
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int count = 30;
        private Queue<GameObject> pool = new(); //키 : 몬스터 키, 밸류 : 몬스터 프리팹

        private void Awake()
        {
            AddPool(count);
        }

        public void AddPool(int addCount)
        {
            for (int i = 0; i < addCount; i++)
            {
                //각 so마다 몬스터 so 불러오기 <- key + 프리팹
                GameObject obj = Instantiate(prefab,gameObject.transform);
                obj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                obj.SetActive(false);
                pool.Enqueue(obj);
            }
        }

        /// <summary> 풀에서 오브젝트 하나를 꺼내서 비활성 상태로 반환, 없으면 풀을 늘린 후 반환</summary>
        /// <returns>사용하려고 하는 비활성화된 오브젝트</returns>
        public GameObject UsePool()
        {
            if (!pool.TryDequeue(out GameObject useObj))
            {
                AddPool(count / 3 + 1);
                count += (count / 3 + 1);
                return pool.Dequeue();
            }

            //Debug.Log($"{useObj.name} 사용함");
            return useObj;
        }

        public void ReturnPool(GameObject returnObject)
        {
            returnObject.SetActive(false);
            pool.Enqueue(returnObject);
            //Debug.Log($"{returnObject.name} 반환됨");
        }

        public int GetOrder() => 0;
    }
}