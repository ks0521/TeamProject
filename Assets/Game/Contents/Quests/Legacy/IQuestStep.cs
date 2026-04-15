using System.Threading.Tasks;

public interface IQuestStep
{
    string Description { get; } //퀘스트 설명문

    // 실제 퀘스트 완료 조건이 충족될 때까지 기다리는 핵심 비동기 메서드
    Task ExecuteStepAsync();
    void OnStartQuest();
    void OnCompleteQuest();
}
