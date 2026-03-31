using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using QuestSystem;

public class NormalQuest : MonoBehaviour
{
    [SerializeField] private List<QuestData> questTemplates; //순환할 퀘스트 목록

    private int playerLevel;

    private int currentIndex = 0;
    private int completedCountToday = 0;
    private const int MAX_QUESTS_PER_DAY = 20; //과도한 보상 방지용 제한

    public async void StartRecurringQuests()
    {
        Debug.Log("튜토리얼 완료 후 순환형 퀘스트로 진입합니다");
        await RunCycleLoop();
    }
    async Task RunCycleLoop()
    {
        while(true) //퀘스트는 항상 존재하므로 항상 만족
        {
            if(completedCountToday >= MAX_QUESTS_PER_DAY)
            {
                await WaitForNextDay(); //오늘치 퀘스트 완료
                completedCountToday = 0;
                continue;
            }

            var data = questTemplates[currentIndex]; //지금 순서의 데이터
            IQuestStep currentStep = CreateStepFromData(data); //새 퀘스트 단계 생성
            QuestManager.Instance.UpdateQuestUI(currentStep.Description);
            currentStep.OnStartQuest();
            await currentStep.ExecuteStepAsync();

            /*
            //여기서는 플레이어의 레벨 * 10만큼 골드가 지급되는 것을 가정함
            int questReward = data.GetScaledReward(playerLevel);
            GiveReward(questReward);
            */

            currentStep.OnCompleteQuest();
            //나눗셈의 나머지에 해당하는 순서대로 퀘스트가 등장
            currentIndex = (currentIndex + 1) % questTemplates.Count;
            completedCountToday++;

            await Task.Delay(100);
        }
    }

    IQuestStep CreateStepFromData(QuestData data)
    {
        switch (data.type)
        {
            case QuestData.GoalType.Hunt:
                return null;
                /* 예시
                return new Tutorial_HuntStep(
                    string.Format(data.descriptionFormat, data.baseTargetValue),
                    data.baseTargetValue
                */
            default:
                return null;
        }
    }

    async Task WaitForNextDay()
    {
        Debug.Log("오늘 수행할 수 있는 퀘스트를 모두 완료하였습니다!");
        await Task.Delay(2000); //실제로는 날짜를 감지하는 로직 필요
    }

    void GiveReward(int amount)
    {
        Debug.Log($"{amount} 골드 획득!");
        //실제 재화 인벤토리 연동
    }
}
