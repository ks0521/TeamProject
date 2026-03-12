using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityPresenter : MonoBehaviour
{
    public enum StatusType
    {
        Atk, MaxHp, Def, AtkSpeed, CritChance, CritDmg, MoveSpeed, GoldRate, ExpRate, ItemDropRate
    }
    [Header("능력치 버튼")]
    [SerializeField] Button[] Upbtn;

    [Header("곱하기 버튼")]
    [SerializeField] Button [] btnX;
    private enum XState 
    {
        X1 = 1 , X10 = 10 , X100 = 100
    }
    private XState X_state;
    private float multiValue;


    [Header("UI 참조")]
    [SerializeField] Atk_Set atk;
    // Start is called before the first frame update
    public void Start()
    {
        X_state = XState.X1;
    }
    public void OnClickX1()
    {
        GameObject obj = EventSystem.current.currentSelectedGameObject;
        if (obj == btnX[0])
        {
            X_state=XState.X1;
        }
        else if (obj == btnX[1])
        {
            X_state = XState.X10;
        }
        else if (obj == btnX[2])
        {
            X_state = XState.X100;
        }
        
    }
    // Update is called once per frame
    void Update()
    {

    }
}
