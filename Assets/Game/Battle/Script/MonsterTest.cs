using Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterTest : MonoBehaviour
{
    public MonsterSO monsterSO;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PrintRewards(); //µå¶ø È®ÀÎ¿ë
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DeleteMonster(); //½ÇÁ¦ Ã³Ä¡
        }
    }

    void DeleteMonster()
    {
        PrintRewards();
        gameObject.SetActive(false);
    }
    void PrintRewards()
    {
        int exp = monsterSO.rewardExp;
        int gold = monsterSO.rewardGold;
        int statStone = monsterSO.rewardStatStone;
        Debug.Log($"È¹µæ °æÇèÄ¡: {exp} / °ñµå: {gold} / °­È­¼®: {statStone}");

        if(monsterSO.dropTable != null && monsterSO.dropTable.dropList != null)
        {
            foreach(var dropInfo in monsterSO.dropTable.dropList)
            {
                float roll = Random.Range(0f, 1f);
                if(roll <= dropInfo.chance)
                {
                    int amount = Random.Range(dropInfo.minAmount, dropInfo.maxAmount + 1);
                    string itemName = dropInfo.item != null ? dropInfo.item.name : "Unknown Item";
                    Debug.Log($"{itemName} x {amount}°³ È¹µæ!");
                }
            }
        }
    }
}
