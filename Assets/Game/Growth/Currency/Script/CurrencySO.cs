using Base.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Base.Data
{
    [CreateAssetMenu(menuName = "Game/Growth/Currency")]
    public class CurrencySO : ScriptableObject
    {
        public Sprite img;
        public string name;
        public string explain;
        public CurrencyType type;
    }
}