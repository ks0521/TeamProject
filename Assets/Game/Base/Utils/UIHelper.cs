using UnityEngine;
using UnityEngine.UI;

namespace Base.Utils
{
    public static class UIHelper
    {
        public static void SkillImgSetting(this Image img, Sprite sp, bool isHoming)
        {
            img.gameObject.SetActive(true);
            img.sprite = sp;
            Vector3 imgRot = isHoming ? new Vector3(0, 0, 135f) : Vector3.zero;
            img.rectTransform.localEulerAngles = imgRot;
        }
        public static void SkillImgUnsetting(this Image img)
        {
            img.gameObject.SetActive(false);
        }
    }
}