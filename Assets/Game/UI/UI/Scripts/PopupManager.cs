using System.Collections.Generic;
using UnityEngine;

namespace Game_UI.Scripts.PopupManager
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

        [Header("ÆË¾÷")]
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
        }//½Ì±ÛÅæ
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
            pop.transform.SetAsLastSibling(); //ÆË¾÷ Á¦ÀÏ ¾ÕÀ¸·Î ¿Å°ÜÁÖ´Â ÄÚµå
            pop.SetActive(true);
            popupStack.Push(pop);
        }//ÆË¾÷ ¿­±â
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
        }//ÆË¾÷ ´Ý±â
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
        }//Áß°£ ÆË¾÷ »èÁ¦
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
        }//Á¦ÀÏ ¸¶Áö¸· ÆË¾÷ ´Ý±â

        public void OpenAbilityPop() { OpenPopup(abilityPop); } //¹öÆ° OnClick ¿¬°á¿ë ÇÔ¼ö
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
