using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shop.Gacha
{
    public class ProbabilityTable : MonoBehaviour
    {
        [Header("Text 참조")]
        [SerializeField] TextMeshProUGUI Name;
        [SerializeField] TextMeshProUGUI Table;

        [Header("UI 참조")]
        [SerializeField] private Button afterButton;
        [SerializeField] private Button beforeButton;

        public void SetTable(string name , string table)
        {
            Name.text = name;
            Table.text = table;
        }
        public void BindButtons(Action after, Action before)
        {
            /*afterButton.onClick.RemoveAllListeners();
            beforeButton.onClick.RemoveAllListeners();

            afterButton.onClick.AddListener(() => after());
            beforeButton.onClick.AddListener(() => before());*/
        }
    }
}
