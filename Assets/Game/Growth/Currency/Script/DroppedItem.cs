using Base.Data;
using System;
using UnityEngine;
using UnityEngine.UI;
using Base.Managers;
using Base.Save;

namespace Growth.Currency
{
    //드랍된 아이템의 타입(장비 / 재화)
    
    
    public class DroppedItem : MonoBehaviour
    {
        private DropReward reward;
        public DropRewardType type;
        public CurrencySO currency;
        public Transform target;
        public SpriteRenderer img;
        public ItemDropManager dropManager;
        private void OnEnable()
        {
            img = GetComponent<SpriteRenderer>();
            img.sprite = currency.img;
        }
        
        public void Init(DropReward inputReward, Transform inputTarget)
        {
            if (inputTarget == null) return;
            if (dropManager == null)
            {
                dropManager = GameManager.Instance.GetGameSystem<ItemDropManager>();
            }

            reward = inputReward;
            target = inputTarget;
            switch (reward.rewardType)
            {
                case DropRewardType.Currency:
                    img.sprite = reward.currencySO.img;
                    break;
                case DropRewardType.Item:
                    img.sprite = reward.itemSO.icon;
                    break;
            }
        }

        private void Update()
        {
            if (target == null) return;
            transform.position = Vector3.MoveTowards(transform.position, target.position, 3.5f * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;
            dropManager.GetReward(reward);
            Destroy(gameObject);
        }
    }
}