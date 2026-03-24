using Base.Data;
using System;
using UnityEngine;
using UnityEngine.UI;
using Base.Managers;
using Base.Save;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Serialization;

namespace Growth.Currency
{
    //드랍된 아이템의 타입(장비 / 재화)
    
    
    public class DroppedItem : MonoBehaviour
    {
        public bool dropDelay; //드랍 유휴시간
        public bool isDropped; //중복 드랍 제거
        public SpriteRenderer img;
        public Transform target;
        public DropReward reward;
        public ItemDropManager dropManager;
        public ItemPoolManager itemPool;
        public CancellationTokenSource cts;

        private void Awake()
        {
            img = GetComponent<SpriteRenderer>();
        }
        
        public void Init(DropReward inputReward, Transform inputTarget, ItemPoolManager inputItempool)
        {
            if (dropManager == null)
                dropManager = GameManager.Instance.GetGameSystem<ItemDropManager>();
            dropDelay = true;
            isDropped = false;
            reward = inputReward;
            target = inputTarget;
            itemPool = inputItempool;
            switch (reward.rewardType)
            {
                case DropRewardType.Currency:
                    img.sprite = reward.currencySO.img;
                    break;
                case DropRewardType.Item:
                    img.sprite = reward.itemSO.icon;
                    break;
            }

            cts = new CancellationTokenSource();
            DropDelay(cts.Token);
        }
        //필드에 떨어진 후 1초후 드랍가능
        async UniTaskVoid DropDelay(CancellationToken cts)
        {
            await Task.Delay(TimeSpan.FromSeconds(1),cancellationToken: cts);
            dropDelay = false;
        }
        private void Update()
        {
            if (target == null || dropDelay) return;
            transform.position = Vector3.MoveTowards(transform.position, target.position, 10f * Time.deltaTime);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Player") || dropDelay || isDropped) return;
            isDropped = true;
            dropManager.GetReward(reward);
            itemPool.ReturnPool(gameObject);
        }

        private void OnDisable()
        {
            cts?.Cancel();
            cts?.Dispose();
        }
    }
}