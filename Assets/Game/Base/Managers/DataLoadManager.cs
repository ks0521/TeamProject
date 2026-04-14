using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Base.Manager
{
    public class DataLoadManager : MonoBehaviour
    {
        public DataLoadManager Instance;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}