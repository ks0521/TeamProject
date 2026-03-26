using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetViewer : MonoBehaviour
{
    [SerializeField] private Image img;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI text;

    public void SetImg(Sprite sprite)
    {
        if (img == null) return;
        img.sprite = sprite;
    }

    public void SetValue(float value)
    {
        if (slider == null) return;
        slider.value = Mathf.Clamp01(value);
    }

    public void SetText(string text)
    {
        if (text == null) return;
        this.text.text = text;
    }
}
