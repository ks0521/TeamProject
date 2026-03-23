using Battle;
using Personal.GyuSeong;
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
        hpBar.RefreshHp(monster.Hp, monster.CurrentBattleStat.maxHp);
        monster.OnMonsterHpChanged += hpBar.RefreshHp;
    }
}
