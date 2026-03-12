using System.Collections;
using System.Collections.Generic;
using Base.Data;
using Battle;
using Growth.StatUpgrade;
using Reward;
using UnityEngine;
[CreateAssetMenu(menuName = "Game/Data/SOHub")]
public class ScriptableObjectHub : ScriptableObject
{
    public DropTableDictionarySO dropTable;
    public StageDictionarySO stageTable;
    public StatusSO statusTable;
    public SkillDictionarySO SkillDictionarySo;
}
