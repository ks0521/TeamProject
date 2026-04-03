using Base.Data;
using System;
using UnityEngine;
using UnityEngine.UI;
using Base.Managers;
using Base.Save;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine.Serialization;

namespace Growth.Currency
{
    //드랍된 아이템의 타입(장비 / 재화)
    
    
    public class DroppedItem : MonoBehaviour
    {
        private float targetSize;
        private bool dropDelay; //드랍 유휴시간 플래그
        private bool isDropped; //중복 드랍 감지용 플래그
        private SpriteRenderer img;
        private Transform target;
        private DropReward reward;
        private ItemDropManager dropManager;
        private ItemPoolManager itemPool;
        private CancellationTokenSource cts;

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
                    img.sprite = reward.currencySO.icon;
                    targetSize = 0.6f;
                    break;
                case DropRewardType.Item:
                    img.sprite = reward.itemSO.icon;
                    targetSize = 1f;
                    break;
            }
            FixSize(img, targetSize);
            cts = new CancellationTokenSource();
            DropDelay(cts.Token).Forget();
        }

        void FixSize(SpriteRenderer spriteRenderer, float targetWorldSize)
        {
            if (spriteRenderer.sprite == null) return;
            Vector2 size = spriteRenderer.sprite.bounds.size;
            float maxSize = Mathf.Max(size.x, size.y);

            if (maxSize <= 0f) return;
            float scale = targetWorldSize / maxSize;
            spriteRenderer.transform.localScale = Vector3.one * scale;
        }
        //필드에 떨어진 후 1초후 드랍가능
        async UniTaskVoid DropDelay(CancellationToken cts)
        {
            await Task.Delay(TimeSpan.FromSeconds(1),cancellationToken: cts);
            dropDelay = false;
        }
 
        private void FixedUpdate()
        {
            if (target == null || dropDelay) return;
            transform.position = Vector3.MoveTowards(transform.position, target.position, 10f * Time.deltaTime);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.layer != 7 || dropDelay || isDropped) return;
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