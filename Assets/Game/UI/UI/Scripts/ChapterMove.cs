using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Scripts.ChapterMove
{
    public class ChapterMove : MonoBehaviour
    {
        [Header("챕터 이름")]
        [SerializeField] TextMeshProUGUI chapterName;

        [Header("챕터 팝업")]
        [SerializeField] GameObject[] chapterPop;

        private int currentChapter;

        [Header("스테이지 이동 버튼")]
        [SerializeField] Button before;
        [SerializeField] Button after;

        private void OnEnable()
        {
            ShowChapter();
        }
        private void Start()
        {
            currentChapter = 0;
        }

        public void OnClickBefore()
        {
            if (currentChapter == 0)
            {
                return;
            }

            currentChapter--;
            ShowChapter();

        }
        public void OnClickAfter()
        {
            if (currentChapter == chapterPop.Length - 1)
            {
                return;
            }

            currentChapter++;
            ShowChapter();

        }
        private void ShowChapter()
        {
            chapterName.text = currentChapter.ToString();

            if (chapterPop == null || chapterPop.Length == 0)//배열 확인용
            {
                return;
            }
            for (int i = 0; i < chapterPop.Length; i++)
            {
                chapterPop[i].SetActive(i == currentChapter);//currentChapter 아닌것들은 false 하는 용도
            }

            if (after != null)
            {
                after.interactable = currentChapter < chapterPop.Length - 1;
            }
            if (before != null)
            {
                before.interactable = currentChapter > 0;
            }
        }
    }

}
