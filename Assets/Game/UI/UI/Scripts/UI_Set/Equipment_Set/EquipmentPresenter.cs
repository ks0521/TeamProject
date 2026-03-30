using Base.Managers;
using Base.Save;
using Growth.Equipment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EquipmentPresenter : MonoBehaviour, IManager
{

    [SerializeField] EquipmentDictionarySO equipment;

    [Header("장비 버튼")]
    [SerializeField] private Button weaponBtn;
    [SerializeField] private Button armorBtn;
    [SerializeField] private Button accBtn;

    [Header("장비 팝업")]
    [SerializeField] private GameObject weaponPop;
    [SerializeField] private GameObject armorPop;
    [SerializeField] private GameObject accPop;
    int currentTab = 0;
    private void OnEnable()
    {
        ShowPopup(currentTab);
    }
    public void Init()
    {
       
        weaponBtn.onClick.AddListener(() => ShowPopup(0));
        armorBtn.onClick.AddListener(() => ShowPopup(1));
        accBtn.onClick.AddListener(() => ShowPopup(2));
    }
    public int GetOrder() => 220;
    void ShowPopup(int index)
    {
        weaponPop.SetActive(false);
        armorPop.SetActive(false);
        accPop.SetActive(false);

        currentTab = index;

        switch (index)
        {
            case 0:
                weaponPop.SetActive(true);
                break;

            case 1:
                armorPop.SetActive(true);
                break;

            case 2:
                accPop.SetActive(true);
                break;
        }
    }


}
