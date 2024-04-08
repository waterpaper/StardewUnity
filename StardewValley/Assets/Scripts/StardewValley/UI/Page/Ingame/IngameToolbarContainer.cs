using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WATP.UI
{
    public class IngameToolbarContainer : UIElement
    {
        private List<IngameToolbarItem> toolbarItems = new();


        public override void Initialize(RectTransform rect)
        {
            base.Initialize(rect);

            for (int i = 0; i < rect.GetChild(0).childCount; i ++)
            {
                var addToolbar = new IngameToolbarItem();
                addToolbar.Initialize(rect.GetChild(0).GetChild(i).GetComponent<RectTransform>());
                addToolbar.Setting(Root.State.inventory.GetItem_Index(i));

                toolbarItems.Add(addToolbar);
            }

            Bind();
        }

        public override void Dispose()
        {
            UnBind();

            base.Dispose();
        }

        void Bind()
        {
            Root.State.inventory.selectIndex.onChange += ItemSelect;
            Root.State.inventory.onChangeAction += ItemChange;
        }

        void UnBind()
        {
            Root.State.inventory.selectIndex.onChange -= ItemSelect;
            Root.State.inventory.onChangeAction -= ItemChange;
        }

        void ItemSelect(int index)
        {
            for (int i = 0; i < toolbarItems.Count; i++)
            {
                if(index == i)
                    toolbarItems[i].OnHighlight();
                else
                    toolbarItems[i].UnHighlight();
            }
        }

        void ItemChange(int index)
        {
            if (index >= toolbarItems.Count) return;

            toolbarItems[index].Setting(Root.State.inventory.GetItem_Index(index));
        }
    }
}
