using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace WATP.UI
{
    /// <summary>
    /// popup 기반 확장 코드
    /// </summary>
    public static class PopupExtention
    {
        public static T CreatePopup<T>(this PopupWidget popup, string popupName, bool isOpen, RectTransform popupRoot) where T : PopupWidget
        {
            //EventManager.Instance.SendEvent(new OpenCircleLoadingEvent());

            var path = popup.PathDefault ? $"Address/Prefab/UI/Popup/{popupName}.prefab" : $"Address/Prefab/UI/Popup/{popupName}.prefab";

            var rect = popup.Load(path, popupRoot);
            rect.gameObject.SetActive(false);
            if (rect == null)
            {
                Debugger.LogError($"No have {popupName} Scene");
                throw new Exception("not prefab");
            }

            popup.SetCanvas(popupRoot);
            popup.RectTransform.gameObject.SetActive(true);

            popup.LoadPopup().Forget();

            if (isOpen)
                popup.OpenPopup();
            else
                popup.Hide();

            //EventManager.Instance.SendEvent(new CloseCircleLoadingEvent());

            return popup as T;
        }

        public static async UniTask<T> CreatePopupAsync<T>(this PopupWidget popup, string popupName, bool isOpen, RectTransform popupRoot) where T : PopupWidget
        {
            //EventManager.Instance.SendEvent(new OpenCircleLoadingEvent());

            var path = popup.PathDefault ? $"Address/Prefab/UI/Popup/{popupName}.prefab" : $"Address/Prefab/UI/Popup/{popupName}.prefab";

            var rect = await popup.LoadAsync(path, popupRoot);
            rect.gameObject.SetActive(false);
            if (rect == null)
            {
                Debugger.LogError($"No have {popupName} Scene");
                throw new Exception("not prefab");
            }

            popup.SetCanvas(popupRoot);

            await popup.LoadPopup();
            popup.RectTransform.gameObject.SetActive(true);

            if (isOpen)
                popup.OpenPopup();
            else
                popup.Hide();

            //EventManager.Instance.SendEvent(new CloseCircleLoadingEvent());

            return popup as T;
        }
    }
}
