using Base.Data;
using Base.Managers;
using Base.Save;
using Growth.Equipment;
using Shop.Gacha;
using System.Collections.Generic;

using UI.Scripts;

using UnityEngine;


public class GachaPresenter : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] private MainUItype_Set goldText;
    [SerializeField] private GachaView[] gachaView;
    [SerializeField] private ProbabilityTable problemTable;
    [SerializeField] private GameObject gachaResult;
    [SerializeField] private Transform transform;

    [Header("가챠 결과 화면")]
    [SerializeField] private GameObject gachaPanel;

    private GachaManager gachaManager;
    private ProgressManager progressManager;
    private PopupManager popupManager;
    private EventHub hub;

    private GameObject tableInstance;
    private GachaResult gachaResultInstance;
    private EquipType currentTableEquipType;
    private int currentTableLevel;

    private EquipType lastEquipType;
    private GachaDrawType lastDrawType;

    private void OnEnable()
    {
        gachaManager = GetComponentInParent<GachaManager>();
        progressManager = GameManager.Instance.GetGameSystem<ProgressManager>();
        popupManager = GameManager.Instance.GetGameSystem<PopupManager>();
        hub = GameManager.Instance.GetGameSystem<EventHub>();

        BindButton();
        RefreshShopUI();
        RefreshGoldUI();

        hub.OnCurrencyChange += EventChain;
    }
    private void OnDisable()
    {
        hub.OnCurrencyChange -= EventChain;
    }

    private void EventChain(CurrencyType type, int fake)
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
        if (tableInstance != null) return;
        currentTableEquipType = type;
        currentTableLevel = gachaManager.GetGachaLevel(type);
        

        tableInstance = Instantiate(problemTable.gameObject, transform);
        ProbabilityTable tableUI = tableInstance.GetComponent<ProbabilityTable>();
        tableUI.BindButtons(OnClickNextTableLevel, OnClickPrevTableLevel);

        RefreshProbabilityTable();

        popupManager.PopupStack.Push(tableInstance);
    }//버튼에 넣을 함수(확률표 열기)
    private void OnClickDraw(EquipType type, GachaDrawType drawType)
    {
        lastEquipType = type;
        lastDrawType = drawType;

        List<EquipmentSO> results = gachaManager.ExecuteGacha(type, drawType);

        if (results != null && results.Count > 0)
        {
            GameObject prefab = Instantiate(gachaPanel.gameObject, transform);

            gachaResultInstance = prefab.GetComponent<GachaResult>();
            gachaResultInstance.Show(results);

            popupManager.PopupStack.Push(gachaResultInstance.gameObject);
            popupManager.ClosePopup(gachaResultInstance.gameObject);
            gachaResultInstance.BindButton(OnClickRetry);
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

    private void OnClickRetry()
    {
        if (gachaManager == null) return;

        bool canDraw = gachaManager.GetCanDraw(lastEquipType, lastDrawType);

        if (!canDraw)
        {
            Debug.Log("재화 부족 or 가챠 불가");
            return;
        }

        List<EquipmentSO> results = gachaManager.ExecuteGacha(lastEquipType, lastDrawType);

        if (results != null && results.Count > 0)
        {
            gachaResultInstance.Show(results);
        }

        RefreshShopUI();
        RefreshGoldUI();
    }//다시 뽑기 체크
    private void RefreshProbabilityTable()
    {
        ProbabilityTable tableUI = tableInstance.GetComponent<ProbabilityTable>();

        string tableName = $"{currentTableEquipType} 확률표 Lv.{currentTableLevel}";
        string tableText = gachaManager.GetProbabilityTableText(currentTableEquipType, currentTableLevel);

        tableUI.SetTable(tableName, tableText);
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
        Debug.Log($"다음 레벨 클릭: {currentTableLevel}");
        currentTableLevel++;
        RefreshProbabilityTable();
    }//확률표 다음 레벨 보기

}
