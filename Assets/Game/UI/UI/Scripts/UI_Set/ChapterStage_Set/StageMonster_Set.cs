using Base.Managers;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StageMonster_Set : MonoBehaviour
{
    [Header("UI 이미지")]
    [SerializeField] Image [] monsterImg;
    
    public void SetMonster(StageEntry stageEntry)
    {
        for (int i = 0; i < monsterImg.Length; i++)
        {
            monsterImg[i].gameObject.SetActive(false);
        }

        int currentSlot = 0;
        var stageMonster = stageEntry.stageSO.preset;
        for (int i = 0; i < stageMonster.Count; i++)
        {
            var stageMon = stageMonster[i].monster;
            SetSlot(ref currentSlot);
        }

    }
    void SetSlot(ref int slot, Sprite icon = null)
    {
        if (slot < monsterImg.Length)
        {
            monsterImg[slot].sprite = icon;
            monsterImg[slot].gameObject.SetActive(true);
            slot++;
            Debug.Log("이미지 변경!");
        }
    }
}
