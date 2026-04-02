using Base.Data;
using Base.Managers;
using Battle;
using DamageNumbersPro;
using UnityEngine;
using UnityEngine.Serialization;

public class DamageText : MonoBehaviour {

    //Assign prefab in inspector.
    public DamageNumber playerHitText;
    public DamageNumber normalHitText;
    public DamageNumber criticalHitText;
    private EventHub eventHub;
    private void Start()
    {
        eventHub = GameManager.Instance.GetGameSystem<EventHub>();
        eventHub.OnRequestDamageText += RequestDamageText;
    }

    void RequestDamageText(Vector3 position, int resultDamage, HitType type, bool isMonster = true )
    {
        if (!isMonster)
        {
            playerHitText.Spawn(position, resultDamage);
            return;
        }

        switch (type)
        {
            case HitType.Normal:
                normalHitText.Spawn(position, resultDamage);
                break;
            case HitType.Critical:
                criticalHitText.Spawn(position, resultDamage);
                break;
        }
    }
    void Update()
    {
    }
}
