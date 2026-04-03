using Base.Manager;
using Growth.Equipment;
using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Equipment
{
    /// <summary>
    /// 선택된 장비 1개의 상세 정보를 표시하는 View.
    /// 
    /// 화면 표시와 버튼 UI만 담당한다.
    /// 장착 / 강화 / 합성 가능 여부 계산은 하지 않고 Presenter가 계산한 결과를 받아 버튼 활성화 상태만 반영한다.
    /// </summary>
    public class EquipmentDetailView : MonoBehaviour
    {
        //현재 장비 없을때 / 없었다가 생겼을때 / 강화비용이 없다가 생겼을 때 / 합성에 필요한 장비가 없었다가 충족됐을 때 등
        //버튼 비활성->활성화 되는 경우는 거의 다 통과했는데 활성화->비활성화는 아직 테스트가 되지 않았습니다. 
        //이외의 버그테스트는 따로 많이 하지 못해서 버그가 많이 있을 수 있습니다. 
        public EquipmentCatalog CurrentCatalog => currentCatalog;
        [SerializeField] private EquipmentCatalog currentCatalog; //현재 상세정보창의 아이템 카탈로그
        
        [Header("UI 표시")] 
        [SerializeField] private SlotImages slotImages;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI ownedEffectText;
        [SerializeField] private TextMeshProUGUI equipEffectText;

        [Header("버튼")] 
        [SerializeField] private Button equipButton;
        [SerializeField] private Button enhanceButton;
        [SerializeField] private Button combineButton;

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        /// <summary>
        /// 선택된 장비 정보를 상세창 UI에 반영한다.
        /// 
        /// 여기서는 텍스트와 이미지 표시만 갱신한다.
        /// 버튼 활성화 여부는 별도 메서드 RefreshButton()에서 처리한다.
        /// catalog는 이후 버튼 상태 재계산 시 참조할 수 있도록 내부에 저장한다.
        /// </summary>
        public void ShowCatalog(EquipmentCatalog catalog, SlotImages inputSlotImages)
        {
            slotImages.icon.sprite = inputSlotImages.icon.sprite;
            slotImages.frame.color = inputSlotImages.frame.color;
            slotImages.backGround.color = inputSlotImages.backGround.color;

            RefreshCatalog(catalog);
        }
        public void RefreshCatalog(EquipmentCatalog catalog)
        {
            nameText.text = catalog.equipment.itemName;
            levelText.text = $"Lv : {catalog.state.enhancementLevel}";

            ownedEffectText.text = BuildOwnedEffectText(catalog.equipment, catalog.state.enhancementLevel);
            equipEffectText.text = BuildEquipEffectText(catalog.equipment);

            this.currentCatalog = catalog;
        }
        /// <summary>
        /// Presenter가 계산한 버튼 상태 플래그를 받아
        /// 장착 / 강화 / 합성 버튼의 interactable 값을 적용한다.
        /// 
        /// View는 이 값을 계산하지 않고, 전달받은 결과만 화면에 반영한다.
        /// </summary>
        public void ApplyButtonState(EquipmentButtonState state)
        {
            Debug.Log($"DetailView 갱신 결과 : {(int)state}");
            equipButton.interactable = (state & EquipmentButtonState.Equip) != 0;
            enhanceButton.interactable = (state & EquipmentButtonState.Enhance) != 0;
            combineButton.interactable = (state & EquipmentButtonState.Combine) != 0;
        }

        /// <summary> 장착/ 합석 / 강화버튼과 실제 프레젠터 메서드 연결 </summary>
        public void BindButtons(Action OnEquip, Action OnCombine, Action OnEnhance)
        {
            equipButton.onClick.RemoveAllListeners();
            combineButton.onClick.RemoveAllListeners();
            enhanceButton.onClick.RemoveAllListeners();

            equipButton.onClick.AddListener(() => OnEquip?.Invoke());
            combineButton.onClick.AddListener(() => OnCombine?.Invoke());
            enhanceButton.onClick.AddListener(() => OnEnhance?.Invoke());
        }

        /// <summary> 장비 보유효과 계산</summary>
        string BuildOwnedEffectText(EquipmentSO so, int level)
        {
            //예시로 OwnedEffect만 설정했습니다.
            //EquipEffect도 ownedBaseIncrease를 equipBaseIncrease로, ownedPerLevelIncrease를 equipPerLevelIncrease로
            //변경하기만 하면 됩니다
            
            StringBuilder sb = new StringBuilder(); //자동 줄 바꿈

            //AppendLine : 기존 내용 뒤에 내용추가하고 줄바꿈
            if (so.ownedBaseIncrease.atk > 0)
                sb.AppendLine($"공격력 +{so.ownedBaseIncrease.atk + so.ownedPerLevelIncrease.atk * level}");
            if (so.ownedBaseIncrease.atkRate > 0)
                sb.AppendLine($"피해량 +{so.ownedBaseIncrease.atkRate + so.ownedPerLevelIncrease.atkRate * level}%");

            if (so.ownedBaseIncrease.maxHp > 0)
                sb.AppendLine($"체력 +{so.ownedBaseIncrease.maxHp + so.ownedPerLevelIncrease.maxHp * level}");
            if (so.ownedBaseIncrease.maxHpRate > 0)
                sb.AppendLine(
                    $"최대 체력 배율 +{so.ownedBaseIncrease.maxHpRate + so.ownedPerLevelIncrease.maxHpRate * level}%");
            if (so.ownedBaseIncrease.damageReduction > 0)
                sb.AppendLine(
                    $"받는 피해 비율 감소 +{so.ownedBaseIncrease.damageReduction + so.ownedPerLevelIncrease.damageReduction * level}");

            if (so.ownedBaseIncrease.itemDropRate > 0)
                sb.AppendLine(
                    $"아이템 드랍률 +{so.ownedBaseIncrease.itemDropRate + so.ownedPerLevelIncrease.itemDropRate * level}%");
            if (so.ownedBaseIncrease.goldGain > 0)
                sb.AppendLine(
                    $"골드 획득량 +{so.ownedBaseIncrease.goldGain + so.ownedPerLevelIncrease.goldGain * level}%");
            if (so.ownedBaseIncrease.expGain > 0)
                sb.AppendLine(
                    $"경험치 획득량 +{so.ownedBaseIncrease.expGain + so.ownedPerLevelIncrease.expGain * level}%");

            if (so.ownedBaseIncrease.moveSpeed > 0)
                sb.AppendLine(
                    $"이동속도 증가 + {so.ownedBaseIncrease.moveSpeed + so.ownedPerLevelIncrease.moveSpeed * level}%");
            if (so.ownedBaseIncrease.atkSpeed > 0)
                sb.AppendLine(
                    $"공속 증가 + {so.ownedBaseIncrease.atkSpeed + so.ownedPerLevelIncrease.atkSpeed * level}%");

            return sb.Length > 0 ? sb.ToString().TrimEnd() : "보유 효과 없음";
        } 

        string BuildEquipEffectText(EquipmentSO so)
        {
            StringBuilder sb = new StringBuilder();

            return sb.Length > 0 ? sb.ToString().TrimEnd() : "장착 효과 없음";
        } // 장착 효과 표시 // RetentionEffect 에 있는것들중에 장착 효과로 할 스텟들 여기 넣으시면 됩니다.
    }
}
