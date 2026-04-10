using UnityEngine;

//아이템 ID나 개수 등을 저장
[System.Serializable]
public class RewardData
{
    public int itemID;
    public string itemName;
    public int amount;
    public Sprite icon; //실제 아이콘 이미지
    public string description;  //일반 아이템용 설명
    public ScriptableObject originalSO; //원본 SO (EquipmentSO인지 확인용)
}