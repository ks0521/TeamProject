using System;
using Base.Managers;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Base.Save
{
    [Serializable]
    public struct StageProgress
    {
        public int selectedNormalChapter;
        public int selectedNormalStage;
        public int nextChallengeChapter;
        public int nextChallengeStage;
    }
    /// <summary> 실제 런타임 데이터를 보유 / 저장 / 로드하는 데이터 매니저  </summary>
    public class PlayerProgressManager : MonoBehaviour, IManager
    {
        public static PlayerProgressManager Instance;
        [SerializeField] private StatusCalculator playerStatCalculator;
        public RuntimeProgressState progress; //현재 플레이어의 정보를 전부 저장하고 있는 데이터
        public RuntimeProgressState Progress => progress;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void Init()
        {
            LoadProgress();
            AutoSave(this.GetCancellationTokenOnDestroy(), 3f).Forget();
        }

        public int GetOrder()=> 1; //일단 진행사항이 로딩되어야 다른 매니저가 참고 가능

        async UniTaskVoid AutoSave(CancellationToken token, float period)
        {
            while (true)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(period), cancellationToken: token);
                Debug.Log($"자동저장 실행 ({period}초 주기)");
                SaveProgress();
            }
        }
        /// <summary> 런타임 데이터 기기에 저장</summary>
        public void SaveProgress()
        {
            Debug.Log("GameDataManager : 진행 상황을 저장합니다. ");
            SaveManager.Save(DataConverter.RuntimeToSave(progress));
        }
        /// <summary> 저장된 데이터 런타임 데이터형식으로 불러오기</summary>
        public void LoadProgress()
        {
            progress = DataConverter.SaveToRuntime(SaveManager.Load());
        }
    }
}