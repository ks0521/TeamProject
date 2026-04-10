using UnityEngine;
using UnityEngine.UI;

namespace Base.Utils
{
    public static class UIHelper
    {
        // public static void SkillImgSetting(this Image img, Sprite sp, bool isHoming)
        public static void SkillImgSetting(this Image img, Sprite sp)
        {
            img.gameObject.SetActive(true);
            img.sprite = sp;
        }
        public static void SkillImgUnsetting(this Image img)
        {
            img.gameObject.SetActive(false);
        }
    }
}