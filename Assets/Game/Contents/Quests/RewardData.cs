using UnityEngine;

//아이템 ID나 개수 등을 저장
[System.Serializable]
public class RewardData
{
    public int itemID;
    public string itemName;
    public int amount;
    public Sprite icon; //실제 아이콘 이미지
}
