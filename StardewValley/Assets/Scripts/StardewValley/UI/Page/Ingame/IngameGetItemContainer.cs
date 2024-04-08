using System;
using System.Collections.Generic;
using UnityEngine;

namespace WATP.UI
{
    public class IngameGetItemContainer : UIElement
    {
        private RectTransform content;
        private IngameGetItemPanel panel;

        private List<IngameGetItemPanel> list = new();


        public override void Initialize(RectTransform rect)
        {
            base.Initialize(rect);

            content = rectTransform.RecursiveFindChild("Content");
            panel = new();
            panel.Initialize(content.GetChild(0).GetComponent<RectTransform>());

            Bind();
        }

        public void Update()
        {
            foreach (var item in list)
                item.Update();

            for(int i = 0; i < list.Count; i++)
            {
                if (list[i].IsTimeOver)
                {
                    GameObject.Destroy(list[i].RectTransform.gameObject);
                    list.RemoveAt(i);
                    i--;
                }
            }
        }

        public override void Dispose()
        {
            UnBind();

            base.Dispose();
        }


        void Bind()
        {
            Root.State.inventory.onAddAction += OnAddItem;
        }

        void UnBind()
        {
            Root.State.inventory.onAddAction -= OnAddItem;
        }

        void OnAddItem(int id, int qty)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].itemId == id)
                {
                    list[i].Setting(id, qty);
                    return;
                }
            }


            var newPanel = new IngameGetItemPanel();
            newPanel.Initialize(GameObject.Instantiate(panel.RectTransform.gameObject, content).GetComponent<RectTransform>());
            list.Add(newPanel);
            newPanel.Setting(id, qty);
            newPanel.RectTransform.gameObject.SetActive(true);
            Root.SoundManager.PlaySound(SoundTrack.SFX, "getItem");
        }
    }
}
