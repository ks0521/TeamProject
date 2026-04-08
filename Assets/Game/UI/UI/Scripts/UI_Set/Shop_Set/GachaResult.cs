using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Shop.Gacha
{
    public class GachaResult : MonoBehaviour
    {
        [Header("확인 닫기 버튼")]
        [SerializeField] Button checkButton;
        [SerializeField] Button retryButton;

        [Header("장비 프리팹")]
        [SerializeField] ResultItem prefab;

        [Header("프리팹 생성 위치")]
        [SerializeField] private Transform canvas;

        public void Show()
        {

        }


    }

}
