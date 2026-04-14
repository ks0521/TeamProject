using Base.Data;
using Base.Managers;
using Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using UI.Popup;
using UnityEngine;
using UnityEngine.UI;
using static UI.Popup.PopupSO;

namespace UI.Scripts
{
    public class PopupManager : MonoBehaviour, IManager
    {
        public static PopupManager instance;

        [Header("팝업 프리팹 SO")]
        [SerializeField] private PopupSO popupSO;
        Dictionary<PopupType, GameObject> popupDic = new();
        Dictionary<PopupType, GameObject> openPopupDic = new();

        Dictionary<EventPopupType, GameObject> eventPopupDic = new();
        Dictionary<EventPopupType, GameObject> openEventPopupDic = new();

        Dictionary<StagePopupType, GameObject> stagePopupDic = new();
        Dictionary<StagePopupType, GameObject> openStagePopupDic = new();

        [Header("프래핍 생성 위치")]
        [SerializeField] private Transform canvas;


        [Header("이벤트 팝업 프리팹")]
        [SerializeField] private GameObject clearPrefab;
        [SerializeField] private GameObject failPrefab;
        [SerializeField] private GameObject deadPrefab;
        [SerializeField] private ClearReward clearRewardPrefab;

        [Header("챌린지 팝업 프리팹")]
        [SerializeField] SetViewer timer;
        [SerializeField] SetViewer monsterKill;

        [Header("보스 체력 프리팹")]
        [SerializeField] SetViewer BossHp;

        [Header("팝업 버튼")]
        [SerializeField] private Button abilityBtn;
        [SerializeField] private Button chapterBtn;
        [SerializeField] private Button skillBtn;
        [SerializeField] private Button equipmentBtn;
        [SerializeField] private Button dungeonBtn;
        [SerializeField] private Button settingBtn;
        [SerializeField] private Button shopBtn;

        private EventHub hub;
        private Stack<GameObject> popupStack = new();
        public Stack<GameObject> PopupStack => popupStack;
        private StageManager stagemanager;



        private GameObject clearInstance;
        private GameObject failInstance;
        private GameObject deadInstance;

        private SetViewer timerInstance;
        private SetViewer monsterKillInstance;
        private SetViewer bossHpInstance;
        private ClearReward clearRewardInstance;


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
                    OpenPopup(PopupType.End);
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

            InitPopupDic(); //테스트중

            BindAllButton();
            popupStack.Clear();

            hub.OnClearStage += ClearEventChain;
            hub.OnFailStage += FailEventChain;
            hub.OnDeadPlayer += PlayerDeadEventChain;
            //hub.OnGetClearRewards += OpenClearRewardPopup; //나중에 수정될 예정


            Debug.Log(timer);
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
            shopBtn.onClick.RemoveAllListeners();

            /*abilityBtn.onClick.AddListener(OpenAbilityPopup);
            chapterBtn.onClick.AddListener(OpenChapterPopup);
            skillBtn.onClick.AddListener(OpenSkillPopup);
            equipmentBtn.onClick.AddListener(OpenEquipmentPopup);
            dungeonBtn.onClick.AddListener(OpenDungeonPopup);
            settingBtn.onClick.AddListener(OpenSettingPopup);
            shopBtn.onClick.AddListener(OpenShopPopup); */ // 이것들은 지울 예정

            abilityBtn.onClick.AddListener(() => OpenPopup(PopupType.ability));
            equipmentBtn.onClick.AddListener(() => OpenPopup(PopupType.equipment));
            skillBtn.onClick.AddListener(() => OpenPopup(PopupType.skill));
            chapterBtn.onClick.AddListener(() => OpenPopup(PopupType.stage));
            shopBtn.onClick.AddListener(() => OpenPopup(PopupType.shop));
            settingBtn.onClick.AddListener(() => OpenPopup(PopupType.setting));


        }//버튼에 함수 넣기


        private void InitPopupDic()
        {
            if (popupSO == null) return;
            if (popupSO.popupList == null) return;

            for (int i = 0; i < popupSO.popupList.Count; i++)
            {
                var data = popupSO.popupList[i];

                if (data == null) continue;
                if (popupDic.ContainsKey(data.popupType)) continue;

                popupDic.Add(data.popupType, data.popupPrefab);
            }
            for (int i = 0; i < popupSO.eventPopupList.Count; i++)
            {
                var data = popupSO.eventPopupList[i];

                if (data == null) continue;
                if (eventPopupDic.ContainsKey(data.eventPopupType)) continue;

                eventPopupDic.Add(data.eventPopupType, data.popupPrefab);
            }
            for (int i = 0; i < popupSO.stagePopupList.Count; i++)
            {
                var data = popupSO.stagePopupList[i];

                if (data == null) continue;
                if (stagePopupDic.ContainsKey(data.stagePopupType)) continue;

                stagePopupDic.Add(data.stagePopupType, data.popupPrefab);
            }
        }//작업중...
        private void OpenPopup(PopupType type)
        {
            if (!popupDic.TryGetValue(type, out var prefab))
            {
                Debug.Log($"팝업 없음 : {type}");
                return;
            }

            if (openPopupDic.TryGetValue(type, out var open))
            {
                if (open != null)
                {
                    return;
                }
                openPopupDic.Remove(type);
            }//중복생성 방지
            GameObject popup = Instantiate(prefab, canvas);
            popup.transform.SetAsLastSibling();
            popupStack.Push(popup);
            openPopupDic[type] = popup;

            ClosePopup(popup);
        }

        private void OpenEventPopup(EventPopupType type)
        {
            if (!eventPopupDic.TryGetValue(type, out var prefab))
            {
                Debug.Log($"팝업 없음 : {type}");
                return;
            }

            if (openEventPopupDic.TryGetValue(type, out var open))
            {
                if (open != null)
                {
                    return;
                }
                openEventPopupDic.Remove(type);
            }

            GameObject popup = Instantiate(prefab, canvas);
            popup.transform.SetAsLastSibling();
            openEventPopupDic[type] = popup;

            if (type == EventPopupType.clearReward)
            {
                ClosePopup(popup);
                return;
            }
            StartCoroutine(FadeOutPopup(popup , 4f));
        }

        private void OpenStagePopup(StagePopupType type)
        {

        }
        void CloseLastPopup()
        {
            if (popupStack.Count == 0)
            {
                return;
            }

            GameObject lastPop = popupStack.Pop();
            RemovePopupDic(lastPop, openPopupDic);
            RemovePopupDic(lastPop, openEventPopupDic);
            RemovePopupDic(lastPop, openStagePopupDic);
            Destroy(lastPop);

        }




        void PlayerDeadEventChain(Character character)
        {
            if (stagemanager.CurrentStageSo == null) return;
            if (stagemanager.CurrentStageSo.stageType != StageType.Normal) return;

            OpenDeadPopup();

            Debug.Log("플레이어 사망. 페이드 아웃 시작");
        }//이벤트 연결용
        void ClearEventChain(StageSO stage)
        {
            OpenClearPopup();
            CloseTimer();
            CloseMonsterKill();
            CloseBossUI();
        }
        void FailEventChain(StageSO stage)
        {
            OpenFailPopup();
            CloseTimer();
            CloseMonsterKill();
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
            yield return new WaitForSeconds(2f);

            while (endTime < time)
            {
                endTime += Time.deltaTime;
                popupCan.alpha = Mathf.Lerp(1f, 0f, endTime / time);
                yield return null;
            }

            RemovePopupDic(popup, openEventPopupDic);
            Destroy(popup);


        }//클리어 , 실패 팝업 점점 사라지게 하는 코루틴



        private void OpenClearPopup()
        {
            if (clearInstance != null) return;
            if (clearPrefab == null) return;

            clearInstance = Instantiate(clearPrefab, canvas);
            StartCoroutine(FadeOutPopup(clearInstance, 4f));
        }
        private void OpenFailPopup()
        {
            if (failInstance != null) return;
            if (failPrefab == null) return;

            failInstance = Instantiate(failPrefab, canvas);
            StartCoroutine(FadeOutPopup(failInstance, 4f));
        }
        private void OpenDeadPopup()
        {
            if (deadInstance != null) return;
            if (deadPrefab == null) return;

            deadInstance = Instantiate(deadPrefab, canvas);
            StartCoroutine(FadeOutPopup(deadInstance, 3f));
        }
        public void OpenClearRewardPopup(List<DropReward> rewardList, string titleText)
        {
            if (clearRewardInstance != null) return;
            if (clearRewardPrefab == null) return;
            Debug.Log("보상 함수 실행");
            clearRewardInstance = Instantiate(clearRewardPrefab, canvas);
            clearRewardInstance.SetReward(rewardList, titleText);
            ClosePopup(clearRewardInstance.gameObject);
        }


        public void OpenMonsterKill()
        {
            if (monsterKill != null && monsterKillInstance == null)
            {
                Debug.Log("몬스터 킬 생성");
                monsterKillInstance = Instantiate(monsterKill, canvas);
            }

        }
        public void OpenTimer()
        {
            if (timer != null && timerInstance == null)
            {
                Debug.Log("타이머 생성");
                timerInstance = Instantiate(timer, canvas);
            }
        }
        public void OpenBossUI()
        {
            if (BossHp != null && bossHpInstance == null)
            {
                bossHpInstance = Instantiate(BossHp, canvas);
            }
        }
        public void CloseTimer()
        {
            if (timerInstance != null)
            {
                Destroy(timerInstance.gameObject);
                timerInstance = null;
            }
        }
        public void CloseMonsterKill()
        {
            if (monsterKillInstance != null)
            {
                Destroy(monsterKillInstance.gameObject);
                monsterKillInstance = null;
            }
        }
        public void CloseBossUI()
        {
            if (bossHpInstance != null)
            {
                Destroy(bossHpInstance.gameObject);
                bossHpInstance = null;
            }
        }


        public void ClosePopup(GameObject gameObject)
        {
            Close_Button_Set closeBtn = gameObject.GetComponentInChildren<Close_Button_Set>();

            if (closeBtn != null)
            {
                Action[] actions =
                {
                    ()=> RemovePopupFromStack(gameObject),
                    ()=> RemovePopupDic(gameObject, openPopupDic),
                    ()=> RemovePopupDic(gameObject, openEventPopupDic),
                    ()=> RemovePopupDic(gameObject, openStagePopupDic),
                    ()=> Destroy(gameObject)
                };

                closeBtn.BindButton(actions);
            }
        }
        private void RemovePopupFromStack(GameObject target)
        {
            if (target == null || popupStack.Count == 0) return;

            Stack<GameObject> temp = new Stack<GameObject>();

            while (popupStack.Count > 0)
            {
                GameObject pop = popupStack.Pop();

                if (pop != target)
                {
                    temp.Push(pop);
                }
            }
            while (temp.Count > 0)
            {
                popupStack.Push(temp.Pop());
            }

        }//ClosePopup 으로 닫는 팝업을 popupstack 에서 제거용 + popupstack 정렬용
        private void RemovePopupDic<T>(GameObject target, Dictionary<T, GameObject> popupDic)
        {
            if (target == null) return;

            T removeType = default;
            bool found = false;

            foreach (var popup in popupDic)
            {
                if (popup.Value == target)
                {
                    removeType = popup.Key;
                    found = true;
                    break;
                }
            }

            if (found)
            {
                popupDic.Remove(removeType);
            }
        } //open 딕셔너리 관리용

        public bool TryGetTimer(out SetViewer timer)
        {
            timer = timerInstance;
            return timer != null;
        }
        public bool TryGetMonster(out SetViewer kill)
        {
            kill = monsterKillInstance;
            return kill != null;
        }
        public bool TryGetBossHpBar(out SetViewer bossHp)
        {
            bossHp = bossHpInstance;
            return bossHp != null;
        }
    }

}
