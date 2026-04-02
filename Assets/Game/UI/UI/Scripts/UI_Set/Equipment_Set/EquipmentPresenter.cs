using Base.Data;
using Base.Manager;
using Base.Managers;
using Base.Save;
using Growth.Equipment;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.Equipment
{


    /// <summary> DetailView의 버튼 활성화를 위한 Enum flag</summary>
    [Flags]
    public enum EquipmentButtonState
    {
        None = 0,
        Equip = 1 << 0,
        Enhance = 1 << 1,
        Combine = 1 << 2
    }

    /// <summary>
    /// 장비 팝업 전체 흐름을 연결하는 프레젠터.
    /// 
    /// 이 클래스가 담당하는 일:
    /// 1. 현재 선택된 탭(무기 / 방어구 / 악세서리)에 맞는 슬롯 목록을 갱신한다.(RefreshPopUp)
    /// 2. 슬롯 클릭 시 선택된 장비를 기억하고, DetailView를 열어준다.
    /// 3. 장비/재화 변화 이벤트를 받아 DetailView 버튼 상태를 다시 계산한다.(UpdateDetailViewButton)
    /// 4. DetailView에서 발생한 장착 / 강화 / 합성 버튼 클릭을 EquipmentManager로 전달한다.
    /// 
    /// 결과적으로 실제 상태 판단과 매니저 연결을 맡음
    /// </summary>
    public class EquipmentPresenter : MonoBehaviour
    {
        //관규님이 만들어주신 메서드 기반으로 최대한 구현하고자 했고, 추가되어야 할 것 같은 부분들은 임의로 추가했습니다. 
        [Header("상세창")][SerializeField] private EquipmentDetailView detailView; //장비 아이콘 클릭 시 열리는 상세페이지

        [Header("장비 버튼")][SerializeField] private Button openWeaponTabButton; //무기 인벤토리 여는 버튼
        [SerializeField] private Button openArmorTabButton; //방어구 인벤토리 여는 버튼
        [SerializeField] private Button openAccessoryTabButton; //악세서리 인벤토리 여는 버튼

        [Header("장비 팝업")]
        [SerializeField] private GameObject weaponPopup;
        [SerializeField] private GameObject armorPopup;
        [SerializeField] private GameObject accessoryPopup;

        [SerializeField] private GameObject currentPopUp; //현재 열려있는 팝업
        [SerializeField] EquipType currentTabType = EquipType.Weapon; //현재 열려있는 팝업 타입

        private EquipmentManager equipmentManager;
        private EventHub eventHub;
        private List<EquipmentCatalog> equipmentCatalogs; //장비 카탈로그 배열
        private EquipmentCatalog selectedCatalog; //현재 상세보기중인 장비의 카탈로그 
        private EquipmentSlotView[] weaponSlots; //무기 아이콘 슬롯배열
        private EquipmentSlotView[] armorSlots; //방어구 아이콘 슬롯배열
        private EquipmentSlotView[] accSlots; //장신구 아이콘 슬롯배역

        private void Awake()
        {
            openWeaponTabButton.onClick.AddListener(() => ShowPopup(EquipType.Weapon));
            openArmorTabButton.onClick.AddListener(() => ShowPopup(EquipType.Armor));
            openAccessoryTabButton.onClick.AddListener(() => ShowPopup(EquipType.Accessory));
            
        }
        private void OnEnable()
        {
            equipmentManager = GameManager.Instance.GetGameSystem<EquipmentManager>();
            eventHub = GameManager.Instance.GetGameSystem<EventHub>();
            weaponSlots = weaponPopup.GetComponentsInChildren<EquipmentSlotView>(true);
            //armorSlots = armorPop.GetComponentsInChildren<EquipmentSlotView>(true);
            //accSlots = accPop.GetComponentsInChildren<EquipmentSlotView>(true);

            detailView.SetActive(false);
            currentPopUp = weaponPopup; //맨 처음은 무기 인벤토리를 열기
            //GameManager의 Start전 Enable때는 매니저들이 초기화되어있지 않기 때문에 실행하지 많음
            if (equipmentManager == null || eventHub == null) return;
            eventHub.OnGetEquipments += RefreshCurrentTab;
            eventHub.OnGetEquipments += RefreshDetailViewButtonState; //장비 획득은 합성 / 장착버튼
            eventHub.OnCurrencyChange += RefreshCurrency; //재화 획득은 강화버튼 활성화 판정에 필요
            ShowPopup(currentTabType);
        }

        private void OnDisable()
        {
            if (eventHub == null) return;
            eventHub.OnGetEquipments -= RefreshCurrentTab;
            eventHub.OnGetEquipments -= RefreshDetailViewButtonState;
            eventHub.OnCurrencyChange -= RefreshCurrency;
        }

        /// <summary> 장비 타입에 해당하는 팝업창 열기</summary>
        /// <param name="type">열고싶은 장비창 종류(무기 / 방어구 / 악세서리)</param>
        void ShowPopup(EquipType type)
        {
            //수정 내역 : 정수형 고정 인덱스로 하니까 가독성이 살짝 아쉬워서 enum타입으로 조정했습니다
            if (currentPopUp != null)
                currentPopUp.SetActive(false); //새 장비창 열기 전 기존 장비창 닫기
            currentTabType = type;
            switch (type)
            {
                case EquipType.Weapon:
                    currentPopUp = weaponPopup;
                    break;
                    /*case EquipType.Armor:
                        curPopUp = armorPopUp;
                        break;
                    case EquipType.Accessory:
                        curPopUp = accessoryPopUp;
                        break;*/
            }

            detailView.SetActive(false);
            currentPopUp?.SetActive(true);
            RefreshCurrentTab();

            // weaponPop.SetActive(index == 0);
            // armorPop.SetActive(index == 1);
            // accPop.SetActive(index == 2);
            //currentTab = type;
        }

        /// <summary> EquipmentPopup 타입별 팝업창 갱신</summary>
        void RefreshCurrentTab()
        {
            switch (currentTabType)
            {
                case EquipType.Weapon:
                    BindSlotViews(EquipType.Weapon, weaponSlots);
                    break;
                    /*case 1:
                        BindSlots(EquipType.Armor, armorSlots);
                        break;
                    case 2:
                        BindSlots(EquipType.Accessory, accSlots);
                        break;*/
            }
        }

        /// <summary>
        /// 현재 탭에 해당하는 장비 카탈로그를 슬롯 배열에 연결한다.
        /// 
        /// 호출 타이밍:
        /// - 팝업을 처음 열 때
        /// - 탭을 전환할 때
        /// - 장비 획득 이벤트로 슬롯 표시를 다시 그려야 할 때
        /// 
        /// 주의:
        /// 슬롯 클릭 액션 안에서 사용할 catalog / slot은
        /// foreach/for 캡쳐 문제를 피하기 위해 지역변수로 다시 저장해서 사용한다.
        /// </summary>
        void BindSlotViews(EquipType equipType, EquipmentSlotView[] slots)
        {
            //지금은 일단 이렇게 고정된 크기로 만들어놓았지만 차후에는 무한스크롤 구현을 위해 slots을 동적으로 만들고
            //bind할때만 canvasGroup? 같은걸로 실시간 정렬해가면서 만드는 방식도 생각해보면 좋을 것 같습니다. 

            Debug.Log("bindslot 실행");
            equipmentCatalogs = equipmentManager.GetEquipmentCatalogs(equipType);
            if (equipmentCatalogs.Count != slots.Length) //도감 내 장비와 실제 장비가 다를경우 실행하지 않음
            {
                Debug.Log($"도감 내 장비개수({equipmentCatalogs.Count})와 실제 슬롯 개수({slots.Length}) 불일치");
                return;
            }
            //int minLength = equipmentCatalogs.Count < slots.Length ? equipmentCatalogs.Count : slots.Length

            for (int i = 0; i < equipmentCatalogs.Count; i++)
            {
                EquipmentCatalog curCatalog = equipmentCatalogs[i]; //람다 캡쳐문제가 발생해서 매번 변수선언 해줬습니다
                EquipmentSlotView curSlot = slots[i]; //상동

                slots[i].SetSlot(curCatalog, () =>
                {
                    selectedCatalog = curCatalog;
                    detailView.SetActive(true);
                    detailView.ShowCatalog(curCatalog, curSlot.SlotImages);
                    detailView.BindButtons(OnEquip: OnEquipClicked, OnCombine: OnCombineClicked, OnEnhance: OnEnhanceClicked);
                    RefreshDetailViewButtonState();
                });
            }
        }
        /// <summary> OnGetCurrency 이벤트와 RefreshDetailViewButtonState 연결용 메서드 </summary>
        void RefreshCurrency(CurrencyType type, int amount)
        {
            if (type != CurrencyType.GOLD) return;
            RefreshDetailViewButtonState();
        }
        /// <summary>
        /// 현재 DetailView에 떠 있는 장비를 기준으로 버튼 활성화 상태를 다시 계산한다.
        /// 
        /// 호출 타이밍:
        /// - 상세창을 처음 열었을 때
        /// - 장비를 획득했을 때
        /// - 재화가 변해서 강화 가능 여부가 달라졌을 때
        /// 
        /// 중요:
        /// DetailView가 들고 있는 Catalog는 예전 상태일 수 있으므로,
        /// 버튼 계산 전에는 EquipmentManager에서 최신 Catalog를 다시 조회해서 사용한다.
        /// </summary>
        void RefreshDetailViewButtonState()
        {
            //기존 detailView.Catalog사용하면 장비 획득 후 카탈로그가 갱신되지 않은 상태에서 실행되기 때문에
            //제대로 연산이 안됩니다.(장비 최초획득시 상세창 장착버튼 활성화가 바로 안됐었음)
            //재연하고 싶으시면 lastCatalog -> detailView.Catalog로 바꾸신 후 미획득한 장비 클릭 -> test에서 장비 획득하시면
            //재연가능합니다. 재연 안되면 말씀해주세용 

            if (!detailView.isActiveAndEnabled || detailView.CurrentCatalog == null) return;
            EquipmentButtonState state = EquipmentButtonState.None;
            equipmentManager.TryGetEquipmentCatalog(detailView.CurrentCatalog.key, out var lastCatalog);
            if (lastCatalog == null) return;

            selectedCatalog = lastCatalog;
            detailView.RefreshCatalog(lastCatalog);

            //장착가능은 1<<0, 강화가능은 1<<1, 합성가능은 1<<2이며, 이 값든은 state에 OR연산되어 비트연산자로 작동합니다. 
            if (lastCatalog.state.isDiscovered)
                state |= EquipmentButtonState.Equip;
            if (equipmentManager.CanEnhanceEquipment(lastCatalog.equipment))
                state |= EquipmentButtonState.Enhance;
            if (equipmentManager.CanEquipmentCombine(lastCatalog.key))
                state |= EquipmentButtonState.Combine;
            detailView.ApplyButtonState(state);
        }

        //아래 3개의 메서드는 EquipmentPresenter -> equipmentManager을 직접 참조하는 메서드입니다. 
        //현재 OnCombineClicked과 OnEnhanceClicked는 실제 equipmentManager를 참조하나 equipmentManager쪽에서 실제로 값을 바꾸지는 않아서 
        //실제 게임부분에서의 반영은 내일 진행할 예정입니다. 
        /// <summary> 현재 선택된 장비 장착시도 </summary>
        void OnEquipClicked()
        {
            equipmentManager.Equip(selectedCatalog.equipment);
        }

        /// <summary> 현재 선택된 장비 합성시도</summary>
        void OnCombineClicked()
        {
            equipmentManager.TryEquipmentCombine(selectedCatalog.key);
        }

        /// <summary> 현재 선택된 장비 강화시도</summary>
        void OnEnhanceClicked()
        {
            bool result = equipmentManager.TryEnhanceEquipment(selectedCatalog.equipment);

            if (!result) return;
            RefreshCurrentTab();
            RefreshDetailViewButtonState();
        }
    }
}
