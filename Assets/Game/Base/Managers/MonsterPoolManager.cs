using System;
using System.Collections.Generic;
using Battle;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Base.Data
{
    [Serializable]
    public struct PoolData
    {
        public GameObject obj;
        public int count;
        public PoolData(GameObject obj,int count)
        {
            this.obj = obj;
            this.count = count;
        }
    }

    public class ObjectPool
    {
        private int count;
        private PoolData data;
        private GameObject parent;
        private Queue<GameObject> pool = new();

        public ObjectPool(MonsterSO monster, int count, GameObject parent)
        {
            data = new PoolData(monster.prefeb, count);
            this.parent = parent;
            AddPool(data.count);
        }
        public void AddPool(int count)
        {
            for (int i = 0; i < count; i++)
            {
                //각 so마다 몬스터 so 불러오기 <- key + 프리팹
                GameObject obj = GameObject.Instantiate(data.obj, parent.transform);
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
            Debug.Log($"{useObj.name} 사용함");
            return useObj;
        }

        public void ReturnPool(GameObject returnObject)
        {
            returnObject.SetActive(false);
            pool.Enqueue(returnObject);
            Debug.Log($"{returnObject.name} 반환됨");
        }

        public void ClearPool()
        {
            Debug.Log("풀 정리중");
            foreach (var obj in pool)
            {
               Object.Destroy(obj);
            }
        }
    }
    public class MonsterPoolManager : MonoBehaviour
    {
        private Dictionary<int, ObjectPool> poolDic = new(); //키 : 몬스터 키, 밸류 : 몬스터 프리팹
        
        public void ChangeStage(StageSO stage)
        {
            Debug.Log($"{stage.chapter} chap, {stage.stage} stage 풀로 변경");
            //스테이지 변경 전 기존 풀 정리(사용중이던 풀들은 )
            foreach (var pools in poolDic)
            {
                pools.Value.ClearPool();
            }
            poolDic?.Clear();
            
            foreach (var preset in stage.preset)
            {
                Debug.Log($"{preset.monster.name} {preset.weights * 3}만큼 풀에 생성");
                poolDic.Add(preset.monster.key, new ObjectPool(preset.monster, preset.weights* 3, gameObject));
            }
        }

        public GameObject UsePool(int key)
        {
            GameObject obj = poolDic[key].UsePool();
            return obj;
        }

        public void ReturnPool(int key, GameObject returnObj)
        {
            poolDic[key].ReturnPool(returnObj);
        }
    }
}