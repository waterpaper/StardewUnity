using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEngine.SceneManagement;

namespace WATP.UI
{
    /// <summary>
    /// UI Widget을 관리하는 container
    /// 각 widget에 uid를 통해 관리하며
    /// page와 popup, widget으로 나눠 관리한다.
    /// </summary>
    public class WidgetContainer
    {
        private UIManager uiManager;
        private RectTransform rect;

        private bool sceneClear = true;
        private int waitCount = 0;
        private int nowPageUID = 0;
        private int sceneIndex = 0;

        private readonly List<Widget> addList = new();
        private readonly List<Widget> removeList = new();
        private readonly Dictionary<int, Widget> widgetDic = new();

        private readonly Dictionary<int, List<int>> sceneWidgets = new();
        private readonly Dictionary<int, List<int>> pagePopups = new();
        private readonly List<Widget> blurList = new();

        public RectTransform RectTransform { get => rect; }
        public Widget GetWidget(int uid) => widgetDic.ContainsKey(uid) ? widgetDic[uid] : null;
        public int LastOrder => widgetDic.Count + addList.Count + waitCount;


        public void Initialize(UIManager manager, RectTransform rectTransform)
        {
            uiManager = manager;
            rect = rectTransform;

            sceneIndex = SceneManager.GetActiveScene().buildIndex;
            sceneWidgets.Add(sceneIndex, new());
            //EventController.ServiceEvents.On<SceneLoadStartEvent>(SceneChange);
        }

        public void Destroy()
        {
            rect = null;

            addList.Clear();
            removeList.Clear();
            blurList.Clear();

            foreach (var list in sceneWidgets.Values)
                list.Clear();
            sceneWidgets.Clear();

            foreach (var popups in pagePopups.Values)
                popups.Clear();
            pagePopups.Clear();

            foreach (var widget in widgetDic.Values)
                widget.Dispose();
            widgetDic.Clear();
        }

        public void Update()
        {
            RemoveProcess();
            AddProcess();

            foreach (var widget in widgetDic.Values)
            {
                if (widget.IsShow)
                    widget.Update();
            }
            RemoveProcess();
        }

        private void AddProcess()
        {
            foreach (var widget in addList)
            {
                widgetDic.Add(widget.UID, widget);

                if (widget is PageWidget && widget.IsShow)
                    ViewPage(widget.UID);
            }
            addList.Clear();
        }

        private void RemoveProcess()
        {
            foreach (var widget in removeList)
            {
                if (widgetDic.ContainsKey(widget.UID))
                    widgetDic.Remove(widget.UID);
                else if (addList.Contains(widget))
                    addList.Remove(widget);

                widget.Dispose();
            }
            removeList.Clear();
        }

        /// <summary>
        /// 새로운 위젯을 생성합니다.
        /// </summary>
        /// <param name="customPrefabPath"> 기본 프리팹 네임 </param>
        /// <param name="isEnable"> 생성시 열지 안 열지 </param>
        /// <typeparam name="T"></typeparam>
        /// <returns>생성된 위젯</returns>
        public T Create<T>(string customPrefabPath, RectTransform parent = null, bool isEnable = true, bool autoUnload = true)
            where T : Widget, new()
        {
            return Create(new T(), customPrefabPath, parent, isEnable, autoUnload);
        }

        /// <summary>
        /// 새로운 위젯을 생성합니다.
        /// </summary>
        /// <param name="customPrefabPath"> 기본 프리팹 네임 </param>
        /// <param name="isEnable"> 생성시 열지 안 열지 </param>
        /// <typeparam name="T"></typeparam>
        /// <returns>생성된 위젯</returns>
        public T Create<T>(T widget, string customPrefabPath, RectTransform parent = null, bool isEnable = true, bool autoUnload = true)
            where T : Widget
        {
            try
            {
                waitCount++;
                if (widget is PageWidget)
                {
                    var pageCanvasRoot = uiManager.CreateCanvas(customPrefabPath, parent != null ? parent : rect, LastOrder);
                    var page = widget as PageWidget;
                    page.CreatePage<PageWidget>(customPrefabPath, isEnable, pageCanvasRoot);
                    CreatePageOption(page);
                }
                else if (widget is PopupWidget)
                {
                    var popupCanvasRoot = uiManager.CreateCanvas(customPrefabPath, parent != null ? parent : rect, LastOrder);
                    var popup = widget as PopupWidget;
                    popup.CreatePopup<PopupWidget>(customPrefabPath, isEnable, popupCanvasRoot);
                    CreatePopupOption(popup);
                }
                else
                {
                    (widget as Widget).PrefabCreate(customPrefabPath, parent != null ? parent : rect, isEnable);
                }

                if (autoUnload)
                    sceneWidgets[sceneIndex].Add(widget.UID);
                addList.Add(widget);

                if (widget is PageWidget && isEnable)
                    ViewPage(widget.UID);

                waitCount--;
                return widget;
            }
            catch (Exception e)
            {
                UIController.ServiceEvents.Emit(new UIException() { exception = e });

                waitCount--;
                throw e;
            }
        }


        /// <summary>
        /// 새로운 위젯을 생성합니다.
        /// </summary>
        /// <param name="customPrefabPath"> 기본 프리팹 네임 </param>
        /// <param name="isEnable"> 생성시 열지 안 열지 </param>
        /// <typeparam name="T"></typeparam>
        /// <returns>생성된 위젯</returns>
        public async UniTask<T> CreateAsync<T>(string customPrefabPath, RectTransform parent = null, bool isEnable = true, bool autoUnload = true)
            where T : Widget, new()
        {
            var widget = new T();
            return await CreateAsync(widget, customPrefabPath, parent, isEnable, autoUnload);
        }

        /// <summary>
        /// 새로운 위젯을 생성합니다.
        /// </summary>
        /// <param name="customPrefabPath"> 기본 프리팹 네임 </param>
        /// <param name="isEnable"> 생성시 열지 안 열지 </param>
        /// <typeparam name="T"></typeparam>
        /// <returns>생성된 위젯</returns>
        public async UniTask<T> CreateAsync<T>(T widget, string customPrefabPath, RectTransform parent = null, bool isEnable = true, bool autoUnload = true)
            where T : Widget
        {
           /* try
            {*/
                waitCount++;
                if (widget is PageWidget)
                {
                    var pageCanvasRoot = uiManager.CreateCanvas(customPrefabPath, parent != null ? parent : rect, LastOrder);
                    var page = widget as PageWidget;
                    await page.CreatePageAsync<PageWidget>(customPrefabPath, isEnable, pageCanvasRoot);
                    CreatePageOption(page);
                }
                else if (widget is PopupWidget)
                {
                    var popupCanvasRoot = uiManager.CreateCanvas(customPrefabPath, parent != null ? parent : rect, LastOrder);
                    var popup = widget as PopupWidget;
                    await popup.CreatePopupAsync<PopupWidget>(customPrefabPath, isEnable, popupCanvasRoot);
                    CreatePopupOption(popup);
                }
                else
                {
                    await (widget as Widget).PrefabCreateAsync(customPrefabPath, parent != null ? parent : rect, isEnable);
                }

                if (autoUnload)
                    sceneWidgets[sceneIndex].Add(widget.UID);
                addList.Add(widget);

                waitCount--;
                return widget;
           /* }
            catch (Exception e)
            {
                UIController.ServiceEvents.Emit(new UIException() { exception = e });

                waitCount--;
                throw e;
            }*/
        }

        private void CreatePageOption(PageWidget page)
        {
            pagePopups.Add(page.UID, new());
            page.BackPageUID(nowPageUID);
        }

        private void CreatePopupOption(PopupWidget popup)
        {
            if (popup.IsBlur)
                BlurSetting(popup, true);

            popup.SetPageUID(nowPageUID);

            if (pagePopups.ContainsKey(nowPageUID) == false)
                pagePopups.Add(nowPageUID, new());

            var popupList = pagePopups[nowPageUID];

            if (popupList.Count > 0)
            {
                int uid = popupList[popupList.Count - 1];

                if (widgetDic.ContainsKey(uid))
                {
                    var beforePopup = widgetDic[uid] as PopupWidget;
                    beforePopup.UnderPopup(popup.IsBlockerAni);
                }
            }

            popupList.Add(popup.UID);
        }


        public void Remove(Widget widget)
        {
            if (widgetDic.ContainsKey(widget.UID) == false) return;

            if (widget is PageWidget)
            {
                if (pagePopups.ContainsKey(widget.UID))
                {
                    pagePopups[widget.UID].Clear();
                    pagePopups.Remove(widget.UID);
                }

                if (nowPageUID == widget.UID)
                    nowPageUID = 0;
            }
            else if (widget is PopupWidget)
            {
                var popup = widget as PopupWidget;
                if (pagePopups.ContainsKey(popup.PageUID))
                    pagePopups[popup.PageUID].Remove(widget.UID);
            }

            foreach (var key in sceneWidgets.Keys)
                sceneWidgets[key].Remove(widget.UID);
            
            removeList.Add(widget);
        }

        public void StartCloseAction(PopupWidget popup)
        {
            if (pagePopups.ContainsKey(popup.PageUID) == false) return;

            var popupList = pagePopups[popup.PageUID];

            if (popupList[popupList.Count - 1] == popup.UID)
            {
                if (popupList.Count > 1)
                    (widgetDic[popupList[popupList.Count - 2]] as PopupWidget).OverPopup(popup.IsBlockerAni);
            }
        }

        public void BackPage(PageWidget page)
        {
            Remove(page);
            if (widgetDic.ContainsKey(page.BackUID) == false) return;

            ViewPage(page.BackUID);
        }

        private void BlurSetting(Widget widget, bool isAdd)
        {
            if (isAdd)
            {
                int lastIndex = blurList.Count - 1;

                blurList.Add(widget);
                if (lastIndex == -1)
                    return;

                if (blurList[lastIndex] is PageWidget)
                {
                    (blurList[lastIndex] as PageWidget).BlurObject.SetActive(false);
                }
                else if (blurList[lastIndex] is PopupWidget)
                {
                    (blurList[lastIndex] as PopupWidget).BlurObject.SetActive(false);
                }
            }
            else
            {
                blurList.Remove(widget);

                if (blurList.Count == 0) return;

                int lastIndex = blurList.Count - 1;

                if (blurList[lastIndex] is PageWidget)
                {
                    (blurList[lastIndex] as PageWidget).BlurObject.SetActive(true);
                }
                else if (blurList[lastIndex] is PopupWidget)
                {
                    (blurList[lastIndex] as PopupWidget).BlurObject.SetActive(true);
                }
            }

        }


        public void ViewPage(int pageUID)
        {
            if (pagePopups.ContainsKey(pageUID) == false) return;

            var pagePopupList = pagePopups[pageUID];

            foreach (var popupUID in pagePopupList)
            {
                if (widgetDic.ContainsKey(popupUID) == false)
                {
                    Debugger.Log("popup error");
                    continue;
                }

                widgetDic[popupUID].Show();
            }

            if (nowPageUID != 0)
                HidePage(nowPageUID);

            nowPageUID = pageUID;
            widgetDic[pageUID].Show();
            if ((widgetDic[pageUID] as PageWidget).IsBlur)
                BlurSetting(widgetDic[pageUID], true);
        }

        private void HidePage(int pageUID)
        {
            if (pagePopups.ContainsKey(pageUID) == false) return;

            var pagePopupList = pagePopups[pageUID];

            foreach (var popupUID in pagePopupList)
            {
                if (widgetDic.ContainsKey(popupUID) == false)
                {
                    Debugger.Log("popup error");
                    continue;
                }

                widgetDic[popupUID].Hide();
            }

            widgetDic[pageUID].Hide();
            if (nowPageUID == pageUID)
                nowPageUID = 0;
        }

        public T FindWidget<T>() where T : Widget
        {
            var list = widgetDic.Values.ToList();

            foreach (var widget in list)
            {
                if (widget is T)
                    return widget as T;
            }

            return null;
        }

        
        public void SceneChange(int index)
        {
            nowPageUID = 0;
            sceneClear = true;

            if (sceneWidgets.ContainsKey(sceneIndex))
            {
                if (sceneClear)
                {
                    foreach (var sceneWidgetUID in sceneWidgets[sceneIndex].ToArray())
                    {
                        Remove(widgetDic[sceneWidgetUID]);
                    }
                    sceneWidgets.Remove(sceneIndex);
                }
                else
                {
                    foreach (var sceneWidgetUID in sceneWidgets[sceneIndex])
                    {
                        if (widgetDic[sceneWidgetUID] is not PageWidget) continue;
                        HidePage(sceneWidgetUID);
                    }
                }
            }

            sceneIndex = index;
            if (!sceneWidgets.ContainsKey(sceneIndex))
                sceneWidgets.Add(sceneIndex, new());
            else
            {
                for (int i = sceneWidgets[sceneIndex].Count - 1; i >= 0; i--)
                {
                    if (widgetDic[sceneWidgets[sceneIndex][i]] is not PageWidget) continue;
                    ViewPage(sceneWidgets[sceneIndex][i]);

                    break;
                }
            }
        }
    }
}

