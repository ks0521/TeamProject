using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UI.Popup
{
    [CreateAssetMenu(menuName = "Game/UI/Popup SO")]
    public class PopupSO : ScriptableObject
    {
        [Header("팝업")]
        [SerializeField] public List<PopupData> popupList = new();

        [Header("이벤트 팝업")]
        [SerializeField] public List<EventPopupData> eventPopupList = new();
        
        [Header("스테이지 팝업")]
        [SerializeField] public List<StagePopupData> stagePopupList = new();

        public enum PopupType
        {
            None, ability, equipment, skill, stage, shop, dungeon,
            quest, setting, end, info
        }
        public enum EventPopupType
        {
            clear , fail , dead , clearReward
        }
        public enum StagePopupType
        {
            timer , monKill , Boss
        }
        
        [Serializable] public class PopupData
        {
            public PopupType popupType;
            public GameObject popupPrefab;
        }
        
        [Serializable] public class EventPopupData
        {
            public EventPopupType eventPopupType;
            public GameObject popupPrefab;
        }
        [Serializable] public class StagePopupData
        {
            public StagePopupType stagePopupType;
            public GameObject popupPrefab;
        }
        
    }

}
