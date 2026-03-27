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
    private int deadLine;

    private void Awake()
    {
        slider = GetComponentInChildren<Slider>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetTime(float inputDeadLine, float inputCurTime)
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
    public void UpdateKillText(int current, int target)
    {
        if (lastKill == current && lastTarget == target)
            return;

        lastKill = current;
        lastTarget = target;
        slider.value = Mathf.Clamp01((float)current / target);
        text.text = $"{current} / {target}";
    }
}
