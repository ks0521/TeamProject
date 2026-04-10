using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetViewer : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI text;
    private int lastKill;
    private int lastTarget;
    private int curTime;
    private float deadLine;

    private float bossMaxhp;
    private string bossName;
    private void Awake()
    {
        slider = GetComponentInChildren<Slider>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void SetTime(float inputCurTime)
    {
        SetTime(inputCurTime , deadLine);
    }
    public void SetTime(float inputCurTime , float inputDeadLine)
    {
        if (inputDeadLine <= 0)
        {
            slider.value = 1;
            return;
        }
        slider.value = Mathf.Clamp01(inputCurTime / inputDeadLine);
        if (Mathf.CeilToInt(curTime) == inputCurTime && Mathf.CeilToInt(deadLine) == inputDeadLine)
            return;
        curTime = Mathf.CeilToInt(inputCurTime);
        deadLine = Mathf.CeilToInt(inputDeadLine);
        text.text = $"{curTime} / {deadLine}";
    }
    public void UpdateKillText(int current)
    {
        UpdateKillText(current , lastTarget);
    }
    public void UpdateKillText(int current, int target)
    {
        if (lastKill == current && lastTarget == target)
            return;

        lastKill = current;
        lastTarget = target;
        slider.value = Mathf.Clamp01((float)current / target);
        text.text = $"{current} / {target}";
    }

    public void SetBoss(float hp)
    {
        SetBoss(hp, bossMaxhp);
    }
    public void SetBoss(float hp , float maxhp , string name = null)
    {
        if (maxhp > 0f)
        {
            bossMaxhp = maxhp;
        }
        if (name != null)
        {
            bossName = name;
            text.text = bossName.Replace("(Clone)","").Trim();
        }
        if(bossMaxhp <= 0f) return;

        slider.value = hp / bossMaxhp;
    }
}
