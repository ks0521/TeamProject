using Base.Data;
using Base.Managers;
using Base.Save;
using Growth.Equipment;
using Shop.Gacha;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UI.Scripts;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.EventSystems;
using static Shop.Gacha.GachaConfigSO;
using static UnityEngine.Rendering.DebugUI;

public class GachaPresenter : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] private MainUItype_Set goldText;
    [SerializeField] private GachaView[] gachaView;
    [SerializeField] private ProbabilityTable problemTable;
    [SerializeField] private GachaResult gachaResult;
 
    [Header("가챠 결과 화면")]
    [SerializeField] private GameObject gachaPanel;

    private GachaManager gachaManager;
    private ProgressManager progressManager;
    private PopupManager popupManager;
    private EventHub hub;

    private EquipType currentTableEquipType;
    private int currentTableLevel;

    private void OnEnable()
    {
        gachaManager = GameManager.Instance.GetGameSystem<GachaManager>();
        progressManager = GameManager.Instance.GetGameSystem<ProgressManager>();
        popupManager = GameManager.Instance.GetGameSystem<PopupManager>();


        BindButton();
        RefreshShopUI();
        RefreshGoldUI();

        hub.OnCurrencyChange += EventChain;
    }
    private void OnDisable()
    {
        hub.OnCurrencyChange -= EventChain;
    }

    private void EventChain(CurrencyType type , int fake)
    {
        RefreshShopUI();
        RefreshGoldUI();
    }

    private void BindButton()
    {
        foreach (var view in gachaView)
        {
            EquipType equipType = view.EquipType;

            view.BindButton(
                () => OnClickDraw(equipType, GachaDrawType.One),
                () => OnClickDraw(equipType, GachaDrawType.Ten),
                () => OnClickDraw(equipType, GachaDrawType.Hundred),
                () => OnClickTable(equipType));
        }
    }//View 에 버튼 넣어주기
    private void OnClickTable(EquipType type)
    {
        currentTableEquipType = type;
        currentTableLevel = gachaManager.GetGachaLevel(type);

        problemTable.BindButtons(OnClickNextTableLevel, OnClickPrevTableLevel);

        RefreshProbabilityTable();


        problemTable.gameObject.SetActive(true);
        popupManager.PushPopup(problemTable.gameObject);
    }//버튼에 넣을 함수(확률표 열기)
    private void OnClickDraw(EquipType type, GachaDrawType drawType)
    {

        List<EquipmentSO> results = gachaManager.ExecuteGacha(type, drawType);

        if (results != null && results.Count > 0)
        {
            gachaPanel.SetActive(true);
            gachaResult.Show(results);
        }

        RefreshShopUI();
        RefreshGoldUI();

    }//버튼에 넣을 함수(가챠 + 가챠 화면ON)
    private void RefreshShopUI()
    {
        foreach (var view in gachaView)
        {
            EquipType equipType = view.EquipType;

            int currentLevel = gachaManager.GetGachaLevel(equipType);
            int currentCount = gachaManager.GetCurrentGachaCount(equipType);
            int maxCount = gachaManager.GetNextLevelUpCount(equipType);

            int oneCost = gachaManager.GetDrawCost(equipType, GachaDrawType.One);
            int tenCost = gachaManager.GetDrawCost(equipType, GachaDrawType.Ten);
            int hundredCost = gachaManager.GetDrawCost(equipType, GachaDrawType.Hundred);

            int currentCurrency = progressManager.Currency.gold;

            bool canOne = currentCurrency >= oneCost;
            bool canTen = currentCurrency >= tenCost;
            bool canHundred = currentCurrency >= hundredCost;

            view.SetGachaLevel(currentLevel);
            view.SetGachaCount(currentCount, maxCount);
            view.SetDrawCosts(oneCost, tenCost, hundredCost);
            view.SetButtonStates(canOne, canTen, canHundred);
        }
    }//상점 UI 갱신
    private void RefreshProbabilityTable()
    {
        string tableName = $"{currentTableEquipType} 확률표 Lv.{currentTableLevel}";
        string tableText = gachaManager.GetProbabilityTableText(currentTableEquipType, currentTableLevel);

        problemTable.SetTable(tableName, tableText);
    }//확률표 UI 갱신
    private void RefreshGoldUI()
    {
        if (goldText == null) return;
        if (progressManager == null) return;

        goldText.SetUI(progressManager.Currency.gold);
    }//골드 UI 갱신
    private void OnClickPrevTableLevel()
    {
        GachaConfigSO configSO = gachaManager.GetGachaSO(currentTableEquipType);
        if (configSO == null) return;
        if (currentTableLevel <= configSO.defaultLevel) return;

        currentTableLevel--;
        RefreshProbabilityTable();
    }//확률표 이전 레벨 보기
    private void OnClickNextTableLevel()
    {
        GachaConfigSO configSO = gachaManager.GetGachaSO(currentTableEquipType);
        if (configSO == null) return;
        if (currentTableLevel >= configSO.maxLevel) return;

        currentTableLevel++;
        RefreshProbabilityTable();
    }//확률표 다음 레벨 보기

}
