using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WATP.UI
{
    public static class UIExtension
    {
        /// <summary>
        /// 해당 위치에 ui object가 있는지 확인
        /// </summary>
        /// <param name="touchPos"></param>
        /// <returns></returns>
        public static bool IsPointerOverUIObject(this UIManager uiManager, Vector3 touchPos)
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = touchPos;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

            return results.Count > 0;
        }

        /// <summary>
        /// 해당 위치에 해당 ui가 있는지 확인
        /// </summary>
        /// <param name="touchPos"></param>
        /// <returns></returns>
        static public bool IsPointerOverWidget(this UIManager uiManager, Vector3 touchPos, Widget widget)
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = touchPos;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

            for (int i = 0; i < results.Count; i++)
            {
                if (results[i].gameObject == widget.RectTransform.gameObject)
                    return true;
            }

            return false;
        }

        static public RectTransform CreateCanvas(this UIManager uiManager, string name, RectTransform parent, int order)
        {
            GameObject canvasObject = new GameObject(name);
            canvasObject.layer = 5;
            RectTransform canvasRect = canvasObject.AddComponent<RectTransform>();
            canvasObject.transform.SetParent(parent, false);
            canvasRect.anchorMin = Vector3.zero;
            canvasRect.anchorMax = Vector3.one;
            canvasRect.sizeDelta = Vector2.zero;

            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = order;
            canvasObject.AddComponent<CanvasGroup>();
            canvasObject.AddComponent<GraphicRaycaster>();

            return canvasRect;
        }


        /// <summary>
        /// 하위 object 중 해당 name을 찾아주는 함수
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="childName"></param>
        /// <returns></returns>
        public static RectTransform RecursiveFindChild(this RectTransform widgetTrs, string childName, string parentName = null)
        {
            try
            {
                foreach (RectTransform child in widgetTrs)
                {
                    if (child.name == childName)
                    {
                        if (string.IsNullOrEmpty(parentName) || child.parent.name == parentName)
                            return child;
                    }
                    else
                    {
                        RectTransform found = RecursiveFindChild(child, childName, parentName);
                        if (found != null)
                        {
                            return found;
                        }
                    }
                }
            }
            catch {
            }
            return null;
        }

        /// <summary>
        /// 하위 object 중 해당 name을 찾아주는 함수
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="childName"></param>
        /// <returns></returns>
        public static Transform RecursiveFindChild(this Transform widgetTrs, string childName, string parentName = null)
        {
            foreach (Transform child in widgetTrs)
            {
                if (child.name == childName)
                {
                    if (string.IsNullOrEmpty(parentName) || child.parent.name == parentName)
                        return child;
                }
                else
                {
                    Transform found = RecursiveFindChild(child, childName, parentName);
                    if (found != null)
                    {
                        return found;
                    }
                }
            }
            return null;
        }
    }

    public static class WidgetExtension
    {
        /// <summary>
        /// 새로운 위젯을 생성합니다.
        /// </summary>
        /// <param name="customPrefabPath"> 기본 프리팹 네임 </param>
        /// <param name="isEnable"> 생성시 열지 안 열지 </param>
        /// <typeparam name="T"></typeparam>
        /// <returns>생성된 위젯</returns>
        static public T PrefabCreate<T>(this T widget, string customPrefabPath, RectTransform parent, bool isEnable = true)
            where T : Widget
        {
            var path = widget.PathDefault ? $"Address/Prefab/UI/{customPrefabPath}.prefab" : $"Address/Prefab/UI/{customPrefabPath}.prefab";

            widget.LoadAsync(path, parent).Forget();

            if (isEnable)
                widget.Show();
            else
                widget.Hide();

            return widget;
        }

        /// <summary>
        /// 새로운 위젯을 생성합니다.
        /// </summary>
        /// <param name="customPrefabPath"> 기본 프리팹 네임 </param>
        /// <param name="isEnable"> 생성시 열지 안 열지 </param>
        /// <typeparam name="T"></typeparam>
        /// <returns>생성된 위젯</returns>
        static public async UniTask<T> PrefabCreateAsync<T>(this T widget, string customPrefabPath, RectTransform parent, bool isEnable = true)
            where T : Widget
        {
            var path = widget.PathDefault ? $"Address/Prefab/UI/{customPrefabPath}.prefab" : $"Address/Prefab/UI/{customPrefabPath}.prefab";

            await widget.LoadAsync(path, parent);

            if (isEnable)
                widget.Show();
            else
                widget.Hide();

            return widget;
        }

        /// <summary>
        /// world 좌표를 canvas 좌표로 바꿔주는 함수
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="camera"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Vector2 WorldToCanvasPosition(this Widget widget, RectTransform canvas, Camera camera, Vector2 position)
        {
            Vector3 vPos = new Vector3(position.x, 0, position.y);
            return widget.WorldToCanvasPosition(canvas, camera, vPos);
        }

        /// <summary>
        /// world 좌표를 canvas 좌표로 바꿔주는 함수
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="camera"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Vector2 WorldToCanvasPosition(this Widget widget, RectTransform canvas, Camera camera, Vector3 position)
        {
            Vector3 temp = camera.WorldToViewportPoint(position);

            temp.x *= canvas.sizeDelta.x;
            temp.y *= canvas.sizeDelta.y;

            temp.x -= canvas.sizeDelta.x * canvas.pivot.x;
            temp.y -= canvas.sizeDelta.y * canvas.pivot.y;

            return temp;
        }
    }
}