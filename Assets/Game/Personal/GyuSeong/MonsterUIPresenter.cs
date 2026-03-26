using Battle;
using Personal.GyuSeong;
using System;
using UnityEngine;

public class MonsterUIPresenter : MonoBehaviour
{
    [SerializeField] private MonsterHpBar hpBar;
    [SerializeField] private Monster monster;
    private void Awake()
    {
        hpBar = GetComponentInChildren<MonsterHpBar>();
        hpBar.Init();
        monster = GetComponentInParent<Monster>();
    }

    private void OnEnable()
    {
        if (monster.monsterSO == null)
            return;
        hpBar.RefreshHp(monster.Hp, monster.CurrentBattleStat.maxHp);
        monster.OnMonsterHpChanged += hpBar.RefreshHp;
    }

    private void OnDisable()
    {
        monster.OnMonsterHpChanged -= hpBar.RefreshHp;
    }
}
