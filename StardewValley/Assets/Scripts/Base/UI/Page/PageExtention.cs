using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WATP.UI
{
    public static class PageExtention
    {
        public static T CreatePage<T>(this PageWidget page, string prefabName, bool isOpen, RectTransform root) where T : PageWidget
        {
            //EventManager.Instance.SendEvent(new OpenCircleLoadingEvent());
            var path = page.PathDefault ? $"Address/Prefab/UI/Page/{prefabName}.prefab" : $"Address/Prefab/UI/Page/{prefabName}.prefab";

            var rect = page.Load(path, root);
            rect.gameObject.SetActive(false);
            if (rect == null)
            {
                Debugger.LogError($"No have {prefabName} Scene");
                throw new Exception("not prefab");
            }

            page.PageInfoSetting(); 
            page.SetCanvas(root);

            rect.gameObject.SetActive(false);
            page.LoadPage().Forget();
            //EventManager.Instance.SendEvent(new CloseCircleLoadingEvent());
            if (isOpen)
                page.Show();
            else
                page.Hide();

            return page as T;
        }

        public static async UniTask<T> CreatePageAsync<T>(this PageWidget page, string prefabName, bool isOpen, RectTransform root) where T : PageWidget
        {
            //EventManager.Instance.SendEvent(new OpenCircleLoadingEvent());
            var path = page.PathDefault ? $"Address/Prefab/UI/Page/{prefabName}.prefab" : $"Address/Prefab/UI/Page/{prefabName}.prefab";

            var rect = await page.LoadAsync(path, root);
            rect.gameObject.SetActive(false);
            if (rect == null)
            {
                Debugger.LogError($"No have {prefabName} Scene");
                throw new Exception("not prefab");
            }

            page.PageInfoSetting();
            page.SetCanvas(root);

            await page.LoadPage();
            rect.gameObject.SetActive(true);
            //EventManager.Instance.SendEvent(new CloseCircleLoadingEvent());
            if (isOpen)
                page.Show();
            else
                page.Hide();

            return page as T;
        }
    }
}

