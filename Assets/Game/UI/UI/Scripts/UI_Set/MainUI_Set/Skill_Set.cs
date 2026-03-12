using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skill_Set : MonoBehaviour
{
    [Header("½ºÅ³ ½½·Ô")]
    [SerializeField] Image[] skill;
    // Start is called before the first frame update

    
    public IEnumerator SetSkillCool(int slot, float cool)
    {
        float coolmax = cool;
        while (cool > 0.0f)
        {
            cool -= Time.deltaTime;
            skill[slot].fillAmount = cool / coolmax;
            yield return null;
        }
        skill[slot].fillAmount = 1f;
    }
}
