using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Base.Data
{
    [Serializable]
    public struct PoolData
    {
        public int count;
        public GameObject obj;
    }

    public class ObjectPool
    {
        private PoolData data;
        private GameObject parent;
        private Queue<GameObject> pool;

        public ObjectPool(PoolData data, GameObject parent)
        {
            this.data = data;
            this.parent = parent;
            AddPool(data.count);
        }
        public void AddPool(int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject obj = GameObject.Instantiate(data.obj, parent.transform);
                obj.SetActive(false);
                pool.Enqueue(obj);
            }
        }

        public GameObject UsePool()
        {
            return null;
        }
    }
    public class ObjectPoolManager : MonoBehaviour
    {
        [SerializeField] private List<PoolData> poolDatas;
        public static Dictionary<int, ObjectPool> poolDic;
        
        
    }
}