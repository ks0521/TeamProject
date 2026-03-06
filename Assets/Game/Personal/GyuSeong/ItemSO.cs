using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Personal.GyuSeong
{
    [CreateAssetMenu]
    public class ItemSO : ScriptableObject
    {
        public int key; //구분용 키
        public Sprite icon; //아이템 아이콘
        public string itemName; //아이템 이름
        public string context; //아이템 설명(로어)
    }
}