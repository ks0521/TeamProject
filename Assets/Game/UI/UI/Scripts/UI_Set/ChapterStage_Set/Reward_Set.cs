using Base.Managers;
using UnityEngine;
using UnityEngine.UI;


namespace UI.ChapterStage_Set
{
    public class Reward_Set : MonoBehaviour
    {
        [SerializeField] Image[] rewardImg;

        [SerializeField] Sprite gold;
        [SerializeField] Sprite stone;
        [SerializeField] Sprite exp;


        public void SetReward(StageEntry stageEntry)
        {
            for (int i = 0; i < rewardImg.Length; i++)
            {
                rewardImg[i].gameObject.SetActive(false);
            }

            var dropTable = stageEntry.stageSO.dropTable;

            if (dropTable == null)
            {
                return;
            }

            int currentSlot = 0;

            if (dropTable.rewardExp > 0)
            {
                SetSlot(ref currentSlot, exp);
            }

            foreach (var reward in dropTable.dropList)
            {
                switch (reward.rewardType)
                {
                    case DropRewardType.Currency:
                        SetSlot(ref currentSlot, reward.currencySO.icon);
                        break;

                    case DropRewardType.Item:
                        SetSlot(ref currentSlot, reward.itemSO.icon);
                        break;
                }
            }
        }
        private void SetSlot(ref int slot, Sprite icon)
        {
            if (slot < rewardImg.Length)
            {
                rewardImg[slot].sprite = icon;
                rewardImg[slot].gameObject.SetActive(true);
                slot++;
            }
        }
    }

}
