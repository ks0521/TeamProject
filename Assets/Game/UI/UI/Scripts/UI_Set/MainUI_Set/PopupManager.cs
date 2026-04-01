using Base.Data;
using Base.Managers;
using Battle;
using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using UI.Equipment;
using UI.Scripts.Ability;
using UnityEngine;
using UnityEngine.UI;

namespace Game_UI.Scripts.PopupManager
{
    public class PopupManager : MonoBehaviour, IManager
    {
        private static PopupManager instance;

        [Header("프래핍 생성 위치")]
        [SerializeField] private Transform canvas;

        [Header("팝업 프리팹")]
        [SerializeField] private GameObject abilityPrefab;
        [SerializeField] private GameObject chapterPrefab;
        [SerializeField] private GameObject equipmentPrefab;

        [SerializeField] private GameObject skillPrefab;
        [SerializeField] private GameObject dungeonPrefab;
        [SerializeField] private GameObject settingPrefab;
        [SerializeField] private GameObject gameEndPrefab;

        [SerializeField] private GameObject clearPrefab;
        [SerializeField] private GameObject failPrefab;
        [SerializeField] private GameObject deadPrefab;

        [Header("팝업 버튼")]
        [SerializeField] private Button abilityBtn;
        [SerializeField] private Button chapterBtn;
        [SerializeField] private Button skillBtn;
        [SerializeField] private Button equipmentBtn;
        [SerializeField] private Button dungeonBtn;
        [SerializeField] private Button settingBtn;

        [Header("닫는 버튼")]
        [SerializeField] private Button abilityCloseBtn;


        private EventHub hub;
        private Stack<GameObject> popupStack = new();
        private StageManager stagemanager;

        private Ability abilityInstance;
        private AllChapter_Set chapterInstance;
        private EquipmentPresenter equipmentInstance;
        private GameObject skillInstance;
        private GameObject dungeonInstance;
        private GameObject settingInstance;
        private GameObject gameEndInstance;

        private GameObject clearInstance;
        private GameObject failInstance;
        private GameObject deadInstance;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
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
                    OpenGameEndPopup();
                }
                else
                {
                    CloseLastPopup();
                }
            }
        }

        public void Init()
        {
            hub = GameManager.Instance.GetGameSystem<EventHub>();
            stagemanager = GameManager.Instance.GetGameSystem<StageManager>(); //사망팝업과 스테이지 실패팝업 동시에 뜨는것 방지용

            clearPrefab.SetActive(false);
            failPrefab.SetActive(false);
            deadPrefab.SetActive(false);

            BindAllButton();
            popupStack.Clear();

            hub.OnClearStage += ClearEventChain;
            hub.OnFailStage += FailEventChain;
            hub.OnDeadPlayer += PlayerDeadEventChain;
        }

        public int GetOrder() => 201;

        void BindAllButton()
        {
            abilityBtn.onClick.RemoveAllListeners();
            chapterBtn.onClick.RemoveAllListeners();
            skillBtn.onClick.RemoveAllListeners();
            equipmentBtn.onClick.RemoveAllListeners();
            dungeonBtn.onClick.RemoveAllListeners();
            settingBtn.onClick.RemoveAllListeners();

            abilityBtn.onClick.AddListener(OpenAbilityPopup);
            chapterBtn.onClick.AddListener(OpenChapterPopup);
            skillBtn.onClick.AddListener(OpenSkillPopup);
            equipmentBtn.onClick.AddListener(OpenEquipmentPopup);
            dungeonBtn.onClick.AddListener(OpenDungeonPopup);
            settingBtn.onClick.AddListener(OpenSettingPopup);
        }//버튼에 함수 넣기



        void PlayerDeadEventChain(Character character)
        {
            if (stagemanager.CurStageSO == null) return;
            if (stagemanager.CurStageSO.type != StageType.Normal) return;

            OpenDeadPopup();

            Debug.Log("플레이어 사망. 페이드 아웃 시작");
            StartCoroutine(FadeOutPopup(deadPrefab, 3f));
        }//이벤트 연결용
        void ClearEventChain(StageSO stage)
        {
            OpenClearPopup();
            StartCoroutine(FadeOutPopup(clearPrefab, 4f));
        }
        void FailEventChain(StageSO stage)
        {
            OpenFailPopup();
            StartCoroutine(FadeOutPopup(failPrefab, 4f));
        }
        IEnumerator FadeOutPopup(GameObject popup, float time)
        {
            CanvasGroup popupCan = popup.GetComponent<CanvasGroup>();
            if (popupCan == null)
            {
                Debug.Log(popup.name + "에 CanvasGroup 이 없음");
                popup.SetActive(false);
                yield break;
            }

            popupCan.alpha = 1f;

            float endTime = 0f;
            Debug.Log("팝업 사라지기 시작");
            while (endTime < time)
            {
                endTime += Time.deltaTime;
                popupCan.alpha = Mathf.Lerp(1f, 0f, endTime / time);
                yield return null;
            }
            if (popup == clearInstance) clearInstance = null;
            if (popup == failInstance) failInstance = null;
            if (popup == deadInstance) deadInstance = null;

            Destroy(popup);
        }//클리어 , 실패 팝업 점점 사라지게 하는 코루틴

        void CloseLastPopup()
        {
            if (popupStack.Count == 0)
            {
                return;
            }

            GameObject lastPop = popupStack.Pop();

            if (abilityInstance != null && lastPop == abilityInstance.gameObject)
            {
                Destroy(abilityInstance.gameObject);
                abilityInstance = null;
                return;
            }
            if (chapterInstance != null && lastPop == chapterInstance.gameObject)
            {
                Destroy(chapterInstance.gameObject);
                chapterInstance = null;
                return;
            }
            if (equipmentInstance != null && lastPop == equipmentInstance.gameObject)
            {
                Destroy(equipmentInstance.gameObject);
                equipmentInstance = null;
                return;
            }
            if (skillInstance != null && lastPop == skillInstance)
            {
                Destroy(skillInstance);
                skillInstance = null;
                return;
            }
            if (dungeonInstance != null && lastPop == dungeonInstance)
            {
                Destroy(dungeonInstance);
                dungeonInstance = null;
                return;
            }
            if (settingInstance != null && lastPop == settingInstance)
            {
                Destroy(settingInstance);
                settingInstance = null;
                return;
            }
            if (gameEndInstance != null && lastPop == gameEndInstance)
            {
                Destroy(gameEndInstance);
                gameEndInstance = null;
                return;
            }
            Destroy(lastPop);

        }
        private void PushPopup(GameObject prefab)
        {
            prefab.SetActive(true);
            prefab.transform.SetAsLastSibling();
            popupStack.Push(prefab);
        }
        private void OpenAbilityPopup()
        {
            if (abilityInstance != null) return;
            if (abilityPrefab == null) return;

            GameObject prefab = Instantiate(abilityPrefab, canvas);
            abilityInstance = prefab.GetComponent<Ability>();

            if (abilityInstance == null)
            {
                Debug.Log("abilityPrefab 에 Ability 컴포넌트가 없음");
                Destroy(prefab);
                return;
            }

            abilityInstance.Init();
            PushPopup(prefab);
        }
        private void OpenChapterPopup()
        {
            if (chapterInstance != null) return;
            if (chapterPrefab == null) return;

            GameObject prefab = Instantiate(chapterPrefab, canvas);
            chapterInstance = prefab.GetComponent<AllChapter_Set>();

            if (chapterInstance == null)
            {
                Debug.Log("chapterPrefab 에 AllChapter_Set 컴포넌트가 없음");
                Destroy(prefab);
                return;
            }

            chapterInstance.Init();
            PushPopup(prefab);
        }
        private void OpenEquipmentPopup()
        {
            if (equipmentInstance != null) return;
            if (equipmentPrefab == null) return;

            GameObject prefab = Instantiate(equipmentPrefab, canvas);
            equipmentInstance = prefab.GetComponent<EquipmentPresenter>();

            if (equipmentInstance == null)
            {
                Debug.LogError("equipmentPrefab 에 EquipmentPresenter 컴포넌트가 없음");
                Destroy(prefab);
                return;
            }

            equipmentInstance.Init();
            PushPopup(prefab);
        }
        private void OpenSkillPopup()
        {
            if (skillInstance != null) return;
            if (skillPrefab == null) return;

            skillInstance = Instantiate(skillPrefab, canvas);
            PushPopup(skillInstance);
        }
        private void OpenDungeonPopup()
        {
            if (dungeonInstance != null) return;
            if (dungeonPrefab == null) return;

            dungeonInstance = Instantiate(dungeonPrefab, canvas);
            PushPopup(dungeonInstance);
        }
        private void OpenSettingPopup()
        {
            if (settingInstance != null) return;
            if (settingPrefab == null) return;

            settingInstance = Instantiate(settingPrefab, canvas);
            PushPopup(settingInstance);
        }
        private void OpenGameEndPopup()
        {
            if (gameEndInstance != null) return;
            if (gameEndPrefab == null) return;

            gameEndInstance = Instantiate(gameEndPrefab, canvas);
            PushPopup(gameEndInstance);
        }

        private void OpenClearPopup()
        {
            if (clearInstance != null) return;
            if (clearPrefab == null) return;

            clearInstance = Instantiate(clearPrefab, canvas);
        }
        private void OpenFailPopup()
        {
            if (failInstance != null) return;
            if (failPrefab == null) return;

            failInstance = Instantiate(failPrefab, canvas); 
        }
        private void OpenDeadPopup()
        {
            if (deadInstance != null) return;
            if (deadPrefab == null) return;

            deadInstance = Instantiate(deadPrefab, canvas);
        }
    }

}
