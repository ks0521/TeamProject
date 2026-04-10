using Growth.Equipment;
using System.Collections;
using System.Collections.Generic;
using UI.Equipment;
using UnityEngine;
using UnityEngine.UI;

public class ResultItem : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] Image icon;
    [SerializeField] Image frame;
    [SerializeField] Image background;

    public void SetData(EquipmentSO so)
    {
        if (so == null) return;
        if (icon != null)
        {
            icon.sprite = so.icon;
        }

        ApplyRarityTheme(frame, background, so.rarity);
    }
    private void ApplyRarityTheme(Image frame, Image background, EquipRarity rarity)
    {
        if (frame == null || background == null) return;

        switch (rarity)
        {
            case EquipRarity.Common:
                frame.color = UIRarityColors.Common_FRAME;
                background.color = UIRarityColors.Common_BG;
                break;

            case EquipRarity.UnCommon:
                frame.color = UIRarityColors.Uncommon_FRAME;
                background.color = UIRarityColors.Uncommon_BG;
                break;

            case EquipRarity.Rare:
                frame.color = UIRarityColors.Rare_FRAME;
                background.color = UIRarityColors.Rare_BG;
                break;

            case EquipRarity.Unique:
                frame.color = UIRarityColors.Unique_FRAME;
                background.color = UIRarityColors.Unique_BG;
                break;
        }
    }
}
