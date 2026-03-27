using Base.Data;
using Base.Managers;
using Battle;
using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using UI.Scripts.Ability;
using UnityEngine;
using UnityEngine.UI;

namespace Game_UI.Scripts.PopupManager
{
    public class PopupManager : MonoBehaviour, IManager
    {
        private static PopupManager instance;

        [Header("팝업")]
        [SerializeField] private Ability abilityPop;
        [SerializeField] private AllChapter_Set chapterPop;

        [SerializeField] private GameObject skillPop;
        [SerializeField] private GameObject equipmentPop;
        [SerializeField] private GameObject dungeonPop;
        [SerializeField] private GameObject gameEndPop;

        [SerializeField] private GameObject clearPop;
        [SerializeField] private GameObject failPop;
        [SerializeField] private GameObject deadPop;

        private Stack<GameObject> popupStack = new();

        [Header("팝업 버튼")]
        [SerializeField] private Button abilityBtn;
        [SerializeField] private Button chapterBtn;
        [SerializeField] private Button skillBtn;
        [SerializeField] private Button equipmentBtn;
        [SerializeField] private Button dungeonBtn;

        private EventHub hub;

        [Header("닫는 버튼")]
        [SerializeField] private Button abilityCloseBtn;

        private StageManager stagemanager;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                //DontDestroyOnLoad(gameObject);
                //3.23(규성) : PopUpManager스크립트가 있는 오브젝트가 루트 오브젝트가 아니라서 오류가 발생합니다 
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
            hub = GameManager.Instance.GetGameSystem<EventHub>();
            stagemanager = GameManager.Instance.GetGameSystem<StageManager>(); //사망팝업과 스테이지 실패팝업 동시에 뜨는것 방지용
            if (equipmentPop != null) // 해당 코드들은 아직 스크립트가 아직 없어서 이렇게 해뒀습니다.
            {
                equipmentPop.SetActive(false);
            }
            if (skillPop != null)
            {
                skillPop.SetActive(false);
            }

            if (dungeonPop != null)
            {
                dungeonPop.SetActive(false);
            }
            if (gameEndPop != null)
            {
                gameEndPop.SetActive(false);
            }

            BindAllButton();
            popupStack.Clear();

            abilityPop.Init();
            chapterPop.Init();

            hub.OnClearStage += ClearEventChain;
            hub.OnFailStage += FailEventChain;
            hub.OnDeadPlayer += PlayerDeadEventChain;
        }

        public int GetOrder() => 201;

        void PlayerDeadEventChain(Character character)
        {
            if (stagemanager.CurStageSO == null) return;
            if (stagemanager.CurStageSO.type != StageType.Normal) return;
            OpenPopup(deadPop);

            StartCoroutine(FadeOutPopup(deadPop, 3f));
        }
        void ClearEventChain(StageSO stage)
        {
            OpenPopup(clearPop);

            StartCoroutine(FadeOutPopup(clearPop , 4f));
        }//이벤트 연결용
        void FailEventChain(StageSO stage)
        {
            OpenPopup(failPop);

            StartCoroutine(FadeOutPopup(failPop , 4f));
        }

        IEnumerator FadeOutPopup(GameObject popup , float time)
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

            while (endTime < time)
            {
                endTime += Time.deltaTime;
                popupCan.alpha = Mathf.Lerp(1f, 0f ,endTime / time);
                yield return null;
            }
            popup.SetActive(false);
        }//팝업 점점 사라지게 하는 코루틴
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
        }//팝업 닫기(나중에 팝업에 닫기 버튼 구현 예정)
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
        void BindAllButton()
        {
            abilityBtn.onClick.AddListener(() => OpenPopup(abilityPop.gameObject));
            chapterBtn.onClick.AddListener(() => OpenPopup(chapterPop.gameObject));
            skillBtn.onClick.AddListener(() => OpenPopup(skillPop.gameObject));
            equipmentBtn.onClick.AddListener(() => OpenPopup(equipmentPop.gameObject));
            dungeonBtn.onClick.AddListener(() => OpenPopup(dungeonPop.gameObject));
            
            abilityCloseBtn.onClick.AddListener(() => ClosePopup(abilityPop.gameObject));
        }//버튼에 함수 넣기
    }

}
