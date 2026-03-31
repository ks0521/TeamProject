using Base.Manager;
using Base.Save;
using Growth.Equipment;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Equipment
{
    /// <summary> 장비의 등급별 테마색입니다. _BG는 배경, _FRAME은 테두리 색깔입니다 </summary>
    public static class UIRarityColors
    {
        //배경 색조합 - 커먼 : 회색(100,100,100, alpha : 255), 언커먼 : 하늘색(100,230,255, alpha : 125)
        //, 레어 : 보라색(165,0,255, alpha : 125), 유니크 : 금색(255,200,0, alpha : 125)
        //프레임은 색은 동일 ,alpha는 255고정
        public static readonly Color32 Common_BG = new Color32(100, 100, 100, 255);
        public static readonly Color32 Uncommon_BG = new Color32(100, 230, 255, 125);
        public static readonly Color32 Rare_BG = new Color32(165, 0, 255, 125);
        public static readonly Color32 Unique_BG = new Color32(255, 200, 0, 125);

        public static readonly Color32 Common_FRAME = new Color32(100, 100, 100, 255);
        public static readonly Color32 Uncommon_FRAME = new Color32(100, 230, 255, 255);
        public static readonly Color32 Rare_FRAME = new Color32(165, 0, 255, 255);
        public static readonly Color32 Unique_FRAME = new Color32(255, 200, 0, 255);
    }

    /// <summary> EquipmentDetailView에 필요한 장비 / 프레임 / 배경이미지 3개를 묶은 구조체</summary>
    [Serializable]
    public struct SlotImages
    {
        [Header("기본 UI")] public Image icon; //장비 아이콘
        [Header("등급 표시")] public Image frame; //장비 프레임
        public Image backGround; //장비 배경
    }
    /// <summary>
    /// 장비 인벤토리 슬롯 1칸을 담당하는 View.
    /// 
    /// 역할:
    /// - 장비 아이콘, 등급색, 강화 레벨, 보유 개수 표시
    /// - 잠금 상태 표시
    /// - 슬롯 클릭 시 Presenter가 넘긴 액션 실행
    /// 
    /// 실제 장비 선택/상세창 열기 로직은 Presenter가 담당한다.
    /// </summary>
    public class EquipmentSlotView : MonoBehaviour
    {
        public EquipType equipType;
        public SlotImages SlotImages => slotImages; //참조용 프로퍼티
        
        [SerializeField] private SlotImages slotImages;
        [SerializeField] private Button slotButton;
        [Header("강화정보 표시")]
        [SerializeField] private TextMeshProUGUI enhanceText; //강화정보
        [Header("개수 표시")] 
        [SerializeField] private Slider ownedCountFill; //장비 개수 슬라이더
        [SerializeField] private TextMeshProUGUI ownedCountText; //장비 개수 텍스트
        [Header("잠금표시")] 
        [SerializeField] private GameObject lockIcon; //장비 잠금 오브젝트
    
        /// <summary>
        /// 입력받은 장비 카탈로그를 슬롯 UI에 표시한다.
        /// 
        /// 처리 내용:
        /// - 아이콘 적용
        /// - 등급에 맞는 프레임/배경색 적용
        /// - 보유 개수 및 잠금 상태 표시
        /// - 슬롯 클릭 시 실행할 액션 등록
        /// </summary>
        public void SetSlot(EquipmentCatalog catalog, Action action)
        {
            slotImages.icon.sprite = catalog.equipment.icon;
            
            enhanceText.text = catalog.state.enhancementLevel.ToString();

            ApplyRarityTheme(catalog.equipment.rarity);
            RefreshOwnedCountUI(catalog.equipment, catalog.state);
            
            slotButton.onClick.RemoveAllListeners();
            slotButton.onClick.AddListener(() => action?.Invoke());
        }
        
        /// <summary> 입력받은 희귀도로 슬롯 색상 변경</summary>
        void ApplyRarityTheme(EquipRarity rarity)
        {
            if (slotImages.frame == null) return;

            switch (rarity)
            {
                case EquipRarity.Common:
                    slotImages.frame.color = UIRarityColors.Common_FRAME;
                    slotImages.backGround.color = UIRarityColors.Common_BG;
                    break;

                case EquipRarity.UnCommon:
                    slotImages.frame.color = UIRarityColors.Uncommon_FRAME;
                    slotImages.backGround.color = UIRarityColors.Uncommon_BG;
                    break;

                case EquipRarity.Rare:
                    slotImages.frame.color = UIRarityColors.Rare_FRAME;
                    slotImages.backGround.color = UIRarityColors.Rare_BG;
                    break;

                case EquipRarity.Unique:
                    slotImages.frame.color = UIRarityColors.Unique_FRAME;
                    slotImages.backGround.color = UIRarityColors.Unique_BG;
                    break;
            }
        } 

        /// <summary> 자기 슬롯의 장비 개수 출력, 없으면 잡금표시 (버튼을 잠그지는 않음)</summary>
        public void RefreshOwnedCountUI(EquipmentSO equipment, EquipmentEntryState state)
        {
            if (!state.isDiscovered)
            {
                ownedCountText.text = $"0 / {equipment.combineNeedAmount}";
                ownedCountFill.value = 0;
                enhanceText.text = "";
                
                lockIcon.SetActive(true);
                return;
            }

            ownedCountText.text = $"{state.ownedCount} / {equipment.combineNeedAmount}";
            ownedCountFill.value = Mathf.Clamp01((float)state.ownedCount / equipment.combineNeedAmount);
            enhanceText.text = $"LV {state.enhancementLevel.ToString()}";
            
            lockIcon.SetActive(false);
        }
    }
}
