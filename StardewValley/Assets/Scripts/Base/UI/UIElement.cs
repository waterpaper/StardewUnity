using System;
using System.Collections;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace WATP.UI
{
    interface IUIElement
    {
        RectTransform RectTransform { get; }


        void Initialize(RectTransform rect);

        void Dispose();
    }

    public abstract class UIElement : IUIElement
    {
        protected static UIManager UIManager;
        public static void SetManager(UIManager manager)
        {
            UIManager = manager;
        }

        protected RectTransform rectTransform;
        protected int uid;
        public int UID { get => uid; }

        public RectTransform RectTransform { get => rectTransform; }

        public virtual void Initialize(RectTransform rect)
        {
            rectTransform = rect;
            uid = uid == 0 ? GenID.Get<UIElement>() : uid;
        }

        public virtual void Dispose()
        {
            rectTransform = null;

            FieldInfo[] info = GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (FieldInfo field in info)
            {
                Type fieldType = field.FieldType;

                if (typeof(IList).IsAssignableFrom(fieldType))
                {
                    IList list = field.GetValue(this) as IList;
                    if (list != null)
                    {
                        list.Clear();
                    }
                }

                if (typeof(IDictionary).IsAssignableFrom(fieldType))
                {
                    IDictionary dictionary = field.GetValue(this) as IDictionary;
                    if (dictionary != null)
                    {
                        dictionary.Clear();
                    }
                }

                if (!fieldType.IsPrimitive)
                {
                    field.SetValue(this, null);
                }
            }
        }

    }

    public static class UIElementExtension
    {
        static private WidgetContainer widgetContainer;
        public static void SetManager(WidgetContainer container)
        {
            widgetContainer = container;
        }

        /// <summary>
        /// ���ο� ������ �����մϴ�.
        /// </summary>
        /// <param name="customPrefabPath"> �⺻ ������ ���� </param>
        /// <param name="isEnable"> ������ ���� �� ���� </param>
        /// <typeparam name="T"></typeparam>
        /// <returns>������ ����</returns>
        static public T WidgetCreate<T>(this UIElement element, T widget, string customPrefabPath, RectTransform parent, bool isEnable = true)
            where T : Widget
        {
            var path = widget.PathDefault ? $"Address/Default/Prefab/{customPrefabPath}.prefab" : $"Address/UI/Prefab/{customPrefabPath}.prefab";

            widget.LoadAsync(path, parent).Forget();

            if (isEnable)
                widget.Show();
            else
                widget.Hide();

            return widget;
        }
    }
}

