using UnityEngine;
using UnityEngine.UI;

public class MonsterHpBar : MonoBehaviour
{
    [SerializeField] private Image img;

    public void Init()
    {
        img = GetComponent<Image>();
    }

    public void RefreshHp(float hp, float maxHp)
    {
        if (maxHp == 0)
        {
            img.fillAmount = 1;
            return;
        }
        img.fillAmount = hp / maxHp;
    }
}
