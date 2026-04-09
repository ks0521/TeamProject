using UnityEngine;

namespace Base.Utils
{
    public static class GetCompChildByName
    {
        /// <summary>
        /// 인수로 지정한 Transform의 자식 오브젝트 중 지정한 이름의 object에 해당하는 컴포넌트가 있을 경우
        /// 해당 컴포넌트를 return하는 함수
        /// </summary>
        /// <param name="targetTransform">가져올 컴포넌트의 부모 object</param>
        /// <param name="objectName">컴포넌트를 가져올 대상 object의 이름</param>
        /// <param name="getComponent">가져올 컴포넌트</param>
        /// <param name="isThisCheck">현재 Transform에 있는 컴포넌트도 대상이라면 true, 아니라면 false</param>
        /// <returns>발견되었는지 여부</returns>
        public static bool TryGetChildrenByName<T>(this Transform targetTransform, string objectName,
        out T getComponent, bool isThisCheck = true) where T : Component
        {
            getComponent = null;
            var allChildren = targetTransform.GetComponentsInChildren<T>(true);
            foreach (T checkComponent in allChildren)
            {
                if (!isThisCheck && checkComponent.transform == targetTransform) continue;
                else if (checkComponent.name.StartsWith(objectName))
                {
                    getComponent = checkComponent;
                    return true;
                }
            }
            return false;
        }
    }
}