using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Ability : MonoBehaviour
{
    public enum StatusType
    {
        Atk, MaxHp, Def, AtkSpeed, CritChance, CritDmg, MoveSpeed, GoldRate, ExpRate, ItemDropRate
    }//나중에 참고
    [Header("능력치 버튼")]
    [SerializeField] Button[] Upbtn;

    [Header("곱하기 버튼")]
    [SerializeField] Button_Set btnX;
    private enum XState 
    {
        X1 , X10 , X100
    }
    private XState X_state;
    private float multiValue;

    [Header("UI 참조")]
    [SerializeField] Atk_Set atk;
    // Start is called before the first frame update
    public void Start()
    {
        ChangeState(XState.X1);
    }
    public void RefreshUI()
    {

    }//능력치팝업창 UI 갱신용 함수(능력치 팝업창 안에있는 모든 함수 추가 예정)
    void ChangeState(XState newState)
    {
        X_state = newState;

        switch (X_state)
        {
            case XState.X1:
                multiValue = 1;
                btnX.SelectButton(0);
                break;

            case XState.X10:
                multiValue = 10;
                btnX.SelectButton(1);
                break;

            case XState.X100:
                multiValue = 100;
                btnX.SelectButton(2);
                break;
        }
        RefreshUI();
    }//상태 전환 함수
    public void OnClickX1()
    {
        ChangeState(XState.X1);
    }//버튼 연결용 함수
    public void OnClickX10()
    {
        ChangeState(XState.X10);
    }
    public void OnClickX100()
    {
        ChangeState(XState.X100);
    }

    private void UnlockStats()
    {
        /*bool isLock = playerLv < unlockLv;   
          if(isLock){return;}

        */
    }
    // Update is called once per frame
    void Update()
    {

    }
}
