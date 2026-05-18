using Base.Data;
using Base.Managers;
using Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using UI.ChapterStage_Set;
using UI.Popup;
using UnityEngine;
using UnityEngine.UI;
using static UI.Popup.PopupSO;

namespace UI.Scripts
{
    public enum ExplanationPopup
    {

    }

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

        [Header("팝업 버튼")]
        [SerializeField] private Button abilityBtn;
        [SerializeField] private Button chapterBtn;
        [SerializeField] private Button skillBtn;
        [SerializeField] private Button equipmentBtn;
        [SerializeField] private Button dungeonBtn;
        [SerializeField] private Button settingBtn;
        [SerializeField] private Button shopBtn;
        [SerializeField] private Button questBtn;
        
        
        private EventHub hub;
        private Stack<GameObject> popupStack = new();
        public Stack<GameObject> PopupStack => popupStack;
        private StageManager stagemanager;

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
                    OpenPopup(PopupType.end);
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
            hub.OnGetClearRewards += OpenClearRewardPopup; //나중에 수정될 예정
            //hub.SkillAutoToggleInput += autoBtn.SetAutoBattle;
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
            questBtn.onClick.RemoveAllListeners();

            abilityBtn.onClick.AddListener(() => OpenPopup(PopupType.ability));
            equipmentBtn.onClick.AddListener(() => OpenPopup(PopupType.equipment));
            skillBtn.onClick.AddListener(() => OpenPopup(PopupType.skill));
            chapterBtn.onClick.AddListener(() => OpenPopup(PopupType.stage));
            shopBtn.onClick.AddListener(() => OpenPopup(PopupType.shop));
            settingBtn.onClick.AddListener(() => OpenPopup(PopupType.setting));
            questBtn.onClick.AddListener(() => OpenPopup(PopupType.quest));
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
        public void OpenEventPopup(EventPopupType type)
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

            if (type == EventPopupType.clearReward)return;
            
            StartCoroutine(FadeOutPopup(popup , 4f));
        }
        public void OpenStagePopup(StagePopupType type)
        {
            if (!stagePopupDic.TryGetValue(type, out var prefab))
            {
                Debug.Log($"팝업 없음 : {type}");
                return;
            }
            if (openStagePopupDic.TryGetValue(type, out var open))
            {
                if (open != null)
                {
                    return;
                }
                openStagePopupDic.Remove(type);
            }
            GameObject popup = Instantiate(prefab, canvas);
            popup.transform.SetAsLastSibling();
            openStagePopupDic[type] = popup;
        }

        public void InfoPopup(Sprite icon, string text)
        {
            PopupType type = PopupType.info;

            if (!popupDic.TryGetValue(type, out var prefab))
            {
                Debug.Log("Info 팝업 없음");
                return;
            }

            if (openPopupDic.TryGetValue(type, out var open))
            {
                if (open != null)
                {
                    var view = open.GetComponent<Reward_Set>();
                    view.SetData(icon, text);
                    return;
                }
                openPopupDic.Remove(type);
            }

            GameObject popup = Instantiate(prefab, canvas);
            popup.transform.SetAsLastSibling();
            var popupView = popup.GetComponent<Reward_Set>();
            popupView.SetData(icon, text);
            popupStack.Push(popup);
            openPopupDic[type] = popup;

            ClosePopup(popup);
        }//아이템,몬스터 등등 설명창(미완성)
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

            OpenEventPopup(EventPopupType.dead);
            Debug.Log("플레이어 사망. 페이드 아웃 시작");
        }//이벤트 연결용
        void ClearEventChain(StageSO stage)
        {
            OpenEventPopup(EventPopupType.clear);

            CloseStagePopup(StagePopupType.timer);
            CloseStagePopup(StagePopupType.monKill);
            CloseStagePopup(StagePopupType.Boss);
        }
        void FailEventChain(StageSO stage)
        {
            OpenEventPopup(EventPopupType.fail);

            CloseStagePopup(StagePopupType.timer);
            CloseStagePopup(StagePopupType.monKill);
            CloseStagePopup(StagePopupType.Boss);
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
        public void OpenClearRewardPopup(List<DropReward> rewardList, string titleText)
        {
            OpenEventPopup(EventPopupType.clearReward);

            if (!TryGetEventPopup(EventPopupType.clearReward, out var popup))
            {
                Debug.Log("클리어 보상 팝업을 못 찾음");
                return;
            }
            popup.SetReward(rewardList, titleText);
            ClosePopup(popup.gameObject);
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
        public void CloseStagePopup(StagePopupType type)
        {
            if (!openStagePopupDic.TryGetValue(type, out var open)) return;

            if (open != null)
            {
                Destroy(open);
            }
            openStagePopupDic.Remove(type);
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
        } //open 딕셔너리 관리용(버튼용)

        public bool TryGetEventPopup(EventPopupType type, out ClearReward prefab)
        {
            prefab = null;

            if(!openEventPopupDic.TryGetValue(type, out var open)) return false;

            if(open == null)
            {
                openEventPopupDic.Remove(type);
                return false;
            }
            prefab = open.GetComponent<ClearReward>();
            return true;
        }
        public bool TryGetStagePopup(StagePopupType type, out SetViewer viewer)
        {
            viewer = null; 

            if(!openStagePopupDic.TryGetValue(type, out var open)) return false;

            if(open == null)
            {
                openStagePopupDic.Remove(type);
                return false;
            }

            viewer = open.GetComponent<SetViewer>();
            return true;
        }
    }

}
