using Base.Manager;
using Base.Managers;
using Base.Save;
using Growth.Equipment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EquipmentPresenter : MonoBehaviour, IManager
{

    private EquipmentManager equipment;

    [Header("상세창")]
    [SerializeField] private EquipmentDetailView detailView;

    [Header("장비 버튼")]
    [SerializeField] private Button weaponBtn;
    [SerializeField] private Button armorBtn;
    [SerializeField] private Button accBtn;

    [Header("장비 팝업")]
    [SerializeField] private GameObject weaponPop;
    [SerializeField] private GameObject armorPop;
    [SerializeField] private GameObject accPop;
    int currentTab = 0;

    private EquipmentSlotView[] weaponSlots;
    private EquipmentSlotView[] armorSlots;
    private EquipmentSlotView[] accSlots;

    private void OnEnable()
    {
        detailView.SetActive(false);
        ShowPopup(currentTab);
    }
    public void Init()
    {
        equipment = GameManager.Instance.GetGameSystem<EquipmentManager>();

        weaponSlots = weaponPop.GetComponentsInChildren<EquipmentSlotView>(true);
        //armorSlots = armorPop.GetComponentsInChildren<EquipmentSlotView>(true);
        //accSlots = accPop.GetComponentsInChildren<EquipmentSlotView>(true);

        weaponBtn.onClick.AddListener(() => ShowPopup(0));
        armorBtn.onClick.AddListener(() => ShowPopup(1));
        accBtn.onClick.AddListener(() => ShowPopup(2));

        gameObject.SetActive(false);
    }
    public int GetOrder() => 230;
    void ShowPopup(int index)
    {
        weaponPop.SetActive(index == 0);
        armorPop.SetActive(index == 1);
        accPop.SetActive(index == 2);

        currentTab = index;
        detailView.SetActive(false);

        RefreshPopup();
    }

    void RefreshPopup()
    {
        switch (currentTab)
        {
            case 0:
                BindSlots(EquipType.Weapon, weaponSlots);
                break;
                /*case 1:
                    BindSlots(EquipType.Armor, armorSlots);
                    break;
                case 2:
                    BindSlots(EquipType.Accessory, accSlots);
                    break;*/
        }
    }//새로고침 할 팝업
    void BindSlots(EquipType equipType, EquipmentSlotView[] slots)
    {



    }
}
