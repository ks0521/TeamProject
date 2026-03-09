using System.Collections;
using System.Collections.Generic;
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
                if (Instance == null)
                {
                    return null;
                }
                return Instance;
            }
        }

        [Header("검은화면")]
        [SerializeField] private GameObject blackScreen; //팝업 뒤에 검은화면

        [Header("팝업")]
        [SerializeField] private GameObject abilityPopup; //능력치
        [SerializeField] private GameObject equipmentPopup; //장비
        [SerializeField] private GameObject skillPopup; //스킬
        [SerializeField] private GameObject stagePopup; //스테이지
        [SerializeField] private GameObject dungeonPopup; //던전
        [SerializeField] private GameObject gameEndPopup; //게임 종료

        [SerializeField]private GameObject currentPopup;
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
        }//싱글톤
        // Start is called before the first frame update
        void Start()
        {
            CloseAllPopup();
        }
       public void CloseAllPopup()
        {
            if (blackScreen != null)
            {
                blackScreen.SetActive(false);
            }
            if (abilityPopup != null)
            {
                abilityPopup.SetActive(false);
            }
            if (equipmentPopup != null)
            {
                equipmentPopup.SetActive(false);
            }
            if (skillPopup != null)
            {
                skillPopup.SetActive(false);
            }
            if (stagePopup != null)
            {
                stagePopup.SetActive(false);
            }
            if (dungeonPopup != null)
            {
                dungeonPopup.SetActive(false);
            }
            if (gameEndPopup != null)
            {
                gameEndPopup.SetActive(false);
            }
        }//시작할때 모든 팝업 닫는 함수

        public void OpenAbilityPopup()
        {
            blackScreen.SetActive(true);
            abilityPopup.SetActive(true);
            currentPopup = abilityPopup;
        }//능력치 팝업 띄우는 함수
        public void OpenEquipmentPopup()
        {
            blackScreen.SetActive(true);
            equipmentPopup.SetActive(true);
            currentPopup = equipmentPopup;
        }//장비 팝업 띄우는 함수
        public void OpenSkillPopup()
        {
            blackScreen.SetActive(true);
            skillPopup.SetActive(true);
            currentPopup = skillPopup;
        }//스킬 팝업 띄우는 함수
        public void OpenStagePopup()
        {
            blackScreen.SetActive(true);
            stagePopup.SetActive(true);
            currentPopup = stagePopup;
        }//스테이지 팝업 띄우는 함수
        public void OpenDungeonPopup()
        {
            blackScreen.SetActive(true);
            dungeonPopup.SetActive(true);
            currentPopup = dungeonPopup;
        }//던전 팝업 띄우는 함수

        void CloseCurrentPopup()
        {
            if (currentPopup != null)
            {
                currentPopup.SetActive(false);
                blackScreen.SetActive(false);
                currentPopup = null;
            }
            else
            {
                currentPopup = gameEndPopup;
                gameEndPopup.SetActive(true);
                blackScreen.SetActive(true);
            }
        }//현재 열려있는 팝업 닫기(컴퓨터 ESC , 모바일 뒤로가기)

        // Update is called once per frame
        void Update()
        { 
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseCurrentPopup();
            }//현재 열려있는 팝업 닫기(컴퓨터 ESC , 모바일 뒤로가기)
        }
    }

}
