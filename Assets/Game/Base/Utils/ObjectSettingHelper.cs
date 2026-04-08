using UnityEngine;

namespace Base.Utils
{
    public static class ObjectSettingHelper
    {
        public static bool TryFindChild<T>(Transform parentTransform, out T[] childs)
        {
            childs = parentTransform.GetComponentsInChildren<T>();
            if (childs == null)
            {
                Debug.LogWarning($"{typeof(T)} 컴포넌트를 가진 자식 오브젝트 없음");
                return false;
            }
            return true;
        }
    }
}