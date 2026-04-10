using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UI.Popup
{
    [CreateAssetMenu(menuName = "Game/UI/Popup SO")]
    public class PopupSO : ScriptableObject
    {
        [SerializeField] private List<PopupData> popupList = new();
        public List<PopupData> PopupList => popupList;

        public enum popupType
        {
            None, ability, equipment, skill, stage, shop
        }


        [Serializable]
        public class PopupData
        {
            public popupType popupType;
            public GameObject popupPrefab;
        }

        
    }

}
