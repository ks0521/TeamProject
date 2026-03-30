using Base.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Base.Data
{
    [CreateAssetMenu(menuName = "Game/Growth/Currency")]
    public class CurrencySO : ScriptableObject
    {
        public Sprite icon;
        public string currencyName;
        public string explain;
        public CurrencyType type;
    }
}