using System.Collections;
using System.Collections.Generic;
using Base.Data;
using Battle;
using Growth.Equipment;
using Growth.StatUpgrade;
using Reward;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Game/Data/SOHub")]
public class ScriptableObjectHub : ScriptableObject
{
    public DropTableDictionarySO dropTable;
    public StageDictionarySO stageTable;
    public StatusSO statusTable;
    public SkillDictionarySO SkillTable;
    public CurrencyDataBaseSO currencyTable;
    public EquipmentDictionarySO equipmentTable;
    public ItemDictionarySO itemTable;
}
