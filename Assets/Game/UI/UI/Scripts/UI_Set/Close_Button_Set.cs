using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Close_Button_Set : MonoBehaviour
{
    public void BindButton(Action[] actions)
    {   
        Button button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();

        foreach (var s in actions)
        {
            button.onClick.AddListener(() => s?.Invoke());
        }
    }
}
