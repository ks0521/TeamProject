using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Game_UI_Scripts
{
    public class PopupManager : MonoBehaviour
    {
        private static PopupManager instance;
        public static PopupManager Instance
        {
            get
            {
                if (instance == null)
                {
                    return null;
                }
                return instance;
            }
        }

        [Header("팝업")]
        [SerializeField] private List<BasePop> popup; //팝업 넣는 리스트

        private Dictionary <System.Type , BasePop> popupDic = new Dictionary <System.Type , BasePop>();
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            foreach (BasePop pop in popup)
            {
                popupDic.Add(pop.GetType(),pop);
            }

        }//싱글톤 , popupDic 에 List에 있는 popup 들 넣기
        
        void OpenPopup<T>()
        {
            System.Type popupType = typeof(T); //T에 타입 정보(클래스)를 가져온다
            if (popupDic.TryGetValue(popupType , out BasePop pop)) //Dictionary popupDic 에 있는 popupType(T) 타입 찾아서 pop 에 넣는다.
            {
                pop.gameObject.SetActive(true);
            }
        }
        void CloswPopup<T>()
        {
            System.Type popupType = typeof(T);
            if (popupDic.TryGetValue(popupType,out BasePop pop))
            {
                pop.gameObject.SetActive(false);
            }
        }
        void Start()
        {
            
        }
       


        

        // Update is called once per frame
        void Update()
        {
           
        }
    }

}
