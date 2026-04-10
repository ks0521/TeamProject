using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Ability_Set
{
    public class Button_Set : MonoBehaviour
    {
        [SerializeField] public Button[] Xbtn;

        private Color normal = Color.white;
        private Color press = Color.yellow;

        private int current = 0;

        public void SelectButton(int bt)
        {
            current = bt;

            for (int i = 0; i < Xbtn.Length; i++)
            {
                Xbtn[i].image.color = normal;
            }
            Xbtn[current].image.color = press;
        }
    }

}

