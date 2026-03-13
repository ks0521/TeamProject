using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button_Set : MonoBehaviour
{
    [SerializeField] Image [] Xbtn;

    private Color normal = Color.white;
    private Color press = Color.yellow;

    private int current = 0;

    public void SelectButton(int bt)
    {
        current = bt;

        for (int i = 0; i < Xbtn.Length; i++)
        {
            Xbtn[i].color = normal;
        }
        Xbtn[current].color = press;
    }
}
