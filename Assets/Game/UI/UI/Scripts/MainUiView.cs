using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Game_UI_Scripts_MainUiView
{
    public class MainUiView : MonoBehaviour
    {
        [Header("HP")]
        [SerializeField] Image hp;

        [Header("EXP")]
        [SerializeField] Image expBar;
        [SerializeField] TextMeshProUGUI expPercent;

        [Header("전투력 Text")]
        [SerializeField] TextMeshProUGUI powerText;

        [Header("골드 Text")]
        [SerializeField] TextMeshProUGUI goldText;

        [Header("성장석 Text")]
        [SerializeField] TextMeshProUGUI growthStoneText;

        [Header("스테이지 Text")]
        [SerializeField] TextMeshProUGUI stageText;

        [Header("자동 전투 버튼")]
        [SerializeField] GameObject onButton;
        [SerializeField] GameObject offButton;

        [Header("스킬 아이콘")]
        [SerializeField] Image[] skillIcons;

        public void SetHp(int curhp, int maxhp)
        {
            if (maxhp > 0)
            {
                hp.fillAmount = (float)curhp / maxhp;
            }
            else
            {
                hp.fillAmount = 0f;
            }
        }
        public void SetExp(float exp, float maxExp)
        {
            if (maxExp > 0)
            {
                expBar.fillAmount = exp / maxExp;
            }
            else
            {
                expBar.fillAmount = 0f;
            }
            expPercent.text = $"{exp}/{maxExp}";
        }

        public void SetPower(int power)
        {
            powerText.text = power.ToString();
        }

        public void SetGold(int gold)
        {
            goldText.text = gold.ToString();
        }
        public void SetGrowthStone(int stone)
        {
            growthStoneText.text = stone.ToString();
        }

        public void SetStage(int chapter, int stage)
        {
            stageText.text = $"{chapter}-{stage}";
        }

        public void SetAutoBattle(bool isAuto)
        {
            onButton.SetActive(isAuto);
            offButton.SetActive(!isAuto);
        }
       
    }

}
