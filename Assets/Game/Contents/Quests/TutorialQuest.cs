using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace QuestSystem.TutorialSteps
{
    //튜토리얼 중 무언가를 클릭할 것을 요구하는 경우
    public class Tutorial_Click : IQuestStep
    {
        public string Description { get; }
        private Button targetButton;
        private GameObject guideArrow;
        private Action onCompleteCallback;

        public Tutorial_Click(string desc, Button btn, GameObject arrow, Action reward = null)
        {
            Description = desc;
            targetButton = btn;
            guideArrow = arrow;
            onCompleteCallback = reward;
        }

        public void OnStartQuest() => guideArrow?.SetActive(true);
        public void OnCompleteQuest()
        {
            guideArrow?.SetActive(false);
            onCompleteCallback?.Invoke();
        }

        public async Task ExecuteStepAsync()
        {
            var tcs = new TaskCompletionSource<bool>();
            targetButton.onClick.AddListener(() => tcs.TrySetResult(true));
            await tcs.Task;
            targetButton.onClick.RemoveAllListeners(); //이벤트 정리용
        }

        //튜토리얼 중 사냥을 요구하는 경우
        public class Tutorial_Hunt : IQuestStep
        {
            public string Description { get; private set; }
            private int targetCount;
            private int currentCount;
            private string monsterId; // 필요시 특정 몬스터 구분
            private Action onCompleteCallback;

            public Tutorial_Hunt(string desc, int count, Action reward = null, string mobId = "")
            {
                Description = desc;
                targetCount = count;
                onCompleteCallback = reward;
                monsterId = mobId;
            }

            public void OnStartQuest() { /* 필요시 타겟 표시 */ }
            public void OnCompleteQuest() => onCompleteCallback?.Invoke();

            public async Task ExecuteStepAsync()
            {
                var tcs = new TaskCompletionSource<bool>();

                // 게임 내 이벤트 시스템에 구독 (가상의 이벤트 예시)
                // 실제로는 GameManager.OnMobKilled += Handler; 형태가 됩니다.
                Action<string> huntHandler = null;
                huntHandler = (id) =>
                {
                    if (!string.IsNullOrEmpty(monsterId) && id != monsterId) return;

                    currentCount++;
                    if (currentCount >= targetCount)
                    {
                        //EventHub.OnMobKilled -= huntHandler;
                        tcs.TrySetResult(true);
                    }
                };
                //EventHub.OnMobKilled += huntHandler;
                await tcs.Task;
            }
        }

        public class Tutorial_Action : IQuestStep
        {
            public string Description { get; }
            private string actionTag; //"Equip", "Enhance", "LevelUp" 등
            private Action onCompleteCallback;
            public Tutorial_Action(string desc, string tag, Action reward = null)
            {
                Description = desc;
                actionTag = tag;
                onCompleteCallback = reward;
            }

            public void OnStartQuest() { }
            public void OnCompleteQuest() => onCompleteCallback?.Invoke();

            public async Task ExecuteStepAsync()
            {
                var tcs = new TaskCompletionSource<bool>();

                //가상의 전역 이벤트 매니저로부터 특정 액션이 완료되었음을 수신
                //TutorialEventManager.OnActionSuccess += (tag) => { if(tag == _actionTag) tcs.SetResult(true); };
                await tcs.Task;
            }
        }
    }
}
