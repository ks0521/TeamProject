using Base.Managers;
using Battle;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;


namespace UI.ChapterStage_Set
{
    public class Reward_Set : MonoBehaviour
    {
        [SerializeField] Image[] rewardImg;
        [SerializeField] TextMeshProUGUI[] rewardText;

        public void SetReward()
        {
            foreach (var s in rewardImg)
            {
                s.gameObject.SetActive(false);
            }
            foreach (var s in rewardText)
            {
                s.gameObject.SetActive(false);
            }
        }
        public void SetSlot(int slot, Sprite icon, string amount, string exp = null)
        {
            rewardImg[slot].gameObject.SetActive(true);
            rewardText[slot].gameObject.SetActive(true);
            rewardImg[slot].sprite = icon;
            rewardText[slot].text = amount ?? exp;
        }
    }

}
