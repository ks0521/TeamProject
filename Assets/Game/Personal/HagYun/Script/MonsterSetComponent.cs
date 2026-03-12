using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battle;
namespace Personal.HagYun
{
    public class MonsterSetComponent : MonoBehaviour
    {
        public static MonsterSetComponent ins;
        public GameObject[] monArr;
        public int Cnt { get; private set; }
        private void Awake()
        {
            ins = this;
        }
        void Start()
        {
            Init();
        }

        void Init()
        {
            Cnt = 4;
            for (int i = 0; i < Cnt; i++)
            {
                if (monArr[i] == null) break;
                int cnt = i;
                //monArr[cnt].GetComponent<Monster>().RequestMonDie += () => DeleteElement(cnt);
            }
        }
        public bool TryGetMonster(out GameObject obj)
        {
            foreach (GameObject search in monArr)
            {
                if (search != null)
                {
                    obj = search;
                    return true;
                }
            }
            obj = null;
            return false;
        }
        void DeleteElement(int index)
        {
            if (Cnt <= 0) return;
            monArr[index] = null;
            Cnt--;
        }
    }
}
