using Base.Data;
using Base.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour,IManager
{
    private SkillDictionarySO skillTable;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetOrder() => 20;

    public void Init()
    {
        skillTable = GameManager.Instance.GetGameSystem<GameDataProvider>().SkillTable;
    }
}
