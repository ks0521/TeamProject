using System;
using System.Collections;
using System.Collections.Generic;
using Base.Save;
using UnityEngine;

namespace Base.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
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
            GameDataManager.Instance.Init();
            stageManager.Init();
        }
    }
}