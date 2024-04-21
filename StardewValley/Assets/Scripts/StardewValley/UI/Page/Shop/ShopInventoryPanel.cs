using System.Collections.Generic;
using UnityEngine;

namespace WATP.UI
{
    public class ShopInventoryPanel : UIElement
    {
        private List<InventoryItemPanel> inventoryItemPanels = new();

        private ItemPopup itemPopup;

        public ShopInventoryPanel(ItemPopup itemPopup)
        {
            this.itemPopup = itemPopup;
        }

        public override void Initialize(RectTransform rect)
        {
            base.Initialize(rect);

            var contents = rectTransform.RecursiveFindChild("Content");

            for (int i = 0; i < contents.childCount; i++)
            {
                InventoryItemPanel newPanel = new();
                newPanel.Initialize(contents.GetChild(i).GetComponent<RectTransform>());
                inventoryItemPanels.Add(newPanel);

                newPanel.ActionSetting(i, ClickEvent);
                newPanel.SetInteraction(i < Root.State.inventory.FullCount);
            }

            Bind();
            InventorySetting();
        }

        public override void Dispose()
        {
            UnBind();
            for (int i = 0; i < inventoryItemPanels.Count; i++)
                inventoryItemPanels[i].Dispose();

            base.Dispose();
        }

        void Bind()
        {
            Root.State.inventory.onChangeAction += OnInvenChangeEvent;
        }

        void UnBind()
        {
            Root.State.inventory.onChangeAction -= OnInvenChangeEvent;
        }

        void InventorySetting()
        {
            for (int i = 0; i < inventoryItemPanels.Count; i++)
            {
                var item = Root.State.inventory.GetItem_Index(i);
                inventoryItemPanels[i].Setting(item, itemPopup);

                if (item == null || item.itemId == 0)
                {
                    inventoryItemPanels[i].SetInteraction(false);
                    continue;
                }

                var tableData = Root.GameDataManager.TableData.GetShopItemTableData(item.itemId);
                if (tableData == null || tableData.IsSell == 0)
                    inventoryItemPanels[i].SetInteraction(false);
                else
                    inventoryItemPanels[i].SetInteraction(true);
            }
        }

        void ClickEvent(int index)
        {
            var item = Root.State.inventory.GetItem_Index(index);
            var tableData = Root.GameDataManager.TableData.GetShopItemTableData(item.itemId);

            if (tableData == null || tableData.IsSell == 0)
                return;

            Root.State.inventory.RemoveInventory(item.itemId, 1);
            Root.State.player.money.Value += tableData.Sell;
        }

        void OnInvenChangeEvent(int index)
        {
            var item = Root.State.inventory.GetItem_Index(index);
            inventoryItemPanels[index].Setting(item, itemPopup);
        }
    }
}
