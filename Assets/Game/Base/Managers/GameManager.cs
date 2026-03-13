using System;
using System.Collections;
using System.Collections.Generic;
using Base.Save;
using UnityEngine;

namespace Base.Managers
{
    public class JJ_GameManager : MonoBehaviour
    {
        public static JJ_GameManager Instance;
        [SerializeField] private JJ_StageManager stageManager;
        [SerializeField] private StatusCalculator calculator;
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
            JJ_GameDataManager.Instance.Init();
            stageManager.Init();
        }
    }
}