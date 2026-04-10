using Base.Save;
using UnityEngine;

[System.Serializable]
public class RewardData
{
    public int itemID;
    public string itemName;
    public Sprite icon;
    public int amount;
    public string description;  //팀원 SO엔 없지만, 우리가 UI용으로 쓸 변수
    public ScriptableObject originalSO; //장비, 재화 판별용 원본 데이터

    //[추가] 재화 합산 시 키값으로 사용하기 위함
    public CurrencyType currencyType;
}
