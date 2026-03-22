using Base.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace Game_UI.Scripts.PopupManager
{
    public class PopupManager : MonoBehaviour , IManager
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
        [SerializeField] private GameObject abilityPop;
        [SerializeField] private GameObject equipmentPop;
        [SerializeField] private GameObject skillPop;
        [SerializeField] private GameObject stagePop;
        [SerializeField] private GameObject dungeonPop;
        [SerializeField] private GameObject gameEndPop;

        private Stack<GameObject> popupStack = new();
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
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (popupStack.Count == 0)
                {
                    OpenPopup(gameEndPop);
                }
                else
                {
                    CloseLastPopup();
                }
            }
        }

        public void Init()
        {
            popupStack.Clear();

            if (abilityPop != null)
            {
                abilityPop.SetActive(false);
            }
            if (equipmentPop != null)
            {
                equipmentPop.SetActive(false);
            }
            if (skillPop != null)
            {
                skillPop.SetActive(false);
            }
            if (stagePop != null)
            {
                stagePop.SetActive(false);
            }
            if (dungeonPop != null)
            {
                dungeonPop.SetActive(false);
            }
            if (gameEndPop != null)
            {
                gameEndPop.SetActive(false);
            }
        }
        public int GetOrder() => 200; 
        
       
        
        void OpenPopup(GameObject pop)
        {
            if (pop == null)
            {
                return;
            }
            if (pop.activeSelf)
            {
                return;
            }
            pop.transform.SetAsLastSibling(); //팝업 제일 앞으로 옮겨주는 코드
            pop.SetActive(true);
            popupStack.Push(pop);
        }//팝업 열기
        void ClosePopup(GameObject pop)
        {
            if (pop == null)
            {
                return;
            }
            if (!pop.activeSelf)
            {
                return;
            }
            pop.SetActive(false);
            RemoveFromStack(pop);
        }//팝업 닫기
        private void RemoveFromStack(GameObject target)
        {
            Stack<GameObject> tempStack = new Stack<GameObject>();

            while (popupStack.Count > 0)
            {
                GameObject current = popupStack.Pop();

                if (current == target)
                {
                    break;
                }

                tempStack.Push(current);
            }

            while (tempStack.Count > 0)
            {
                popupStack.Push(tempStack.Pop());
            }
        }//중간 팝업 삭제
        void CloseLastPopup()
        {
            if (popupStack.Count == 0)
            {
                return;
            }
            GameObject lastPop = popupStack.Pop();
            if (lastPop != null)
            {
                lastPop.SetActive(false);
            }
        }//제일 마지막 팝업 닫기

        public void OpenAbilityPop() { OpenPopup(abilityPop); } //버튼 OnClick 연결용 함수
        public void OpenEquipmentPop() { OpenPopup(equipmentPop); }
        public void OpenSkillPop() { OpenPopup(skillPop); }
        public void OpenStagePop() { OpenPopup(stagePop); }
        public void OpenDungeonPop() { OpenPopup(dungeonPop); }

        public void CloseAbilityPop() { ClosePopup(abilityPop); }
        public void CloseEquipmentPop() { ClosePopup(equipmentPop); }
        public void CloseSkillPop() { ClosePopup(skillPop); }
        public void CloseStagePop() { ClosePopup(stagePop); }
        public void CloseDungeonPop() { ClosePopup(dungeonPop); }

    }

}
