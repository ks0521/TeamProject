using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Auto_Set : MonoBehaviour
{
    [Header("자동 전투 버튼")]
    [SerializeField] GameObject onButton;
    [SerializeField] GameObject offButton;


    public void SetAutoBattle(bool isAuto)
    {
        if (isAuto)
        {
            onButton.SetActive(isAuto);
            offButton.SetActive(!isAuto);

        }
        else
        {
            onButton.SetActive(isAuto);
            offButton.SetActive(!isAuto);
        }
    }


}
