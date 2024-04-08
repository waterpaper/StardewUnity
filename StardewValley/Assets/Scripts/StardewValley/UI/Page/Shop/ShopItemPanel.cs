using System.Collections.Generic;
using UnityEngine;

namespace WATP.UI
{
    public class ShopItemPanel : UIElement
    {
        private List<ShopItemSlot> slotItemPanels = new();

        private ItemPopup itemPopup;
        private RectTransform contents;

        public ShopItemPanel(ItemPopup itemPopup)
        {
            this.itemPopup = itemPopup;
        }


        public override void Initialize(RectTransform rect)
        {
            base.Initialize(rect);

            contents = rectTransform.RecursiveFindChild("Content");

            var list = Root.GameDataManager.TableData.GetShopItemTableData_Month(Root.State.month.Value);

            list.Sort((a, b) =>
            {
                if (a.Month < b.Month)
                    return 1;
                else
                    return -1;
            });

            for (int i = 0; i < list.Count; i++)
            {
                ShopItemSlot newSlot = new();
                if(i != 0)
                    GameObject.Instantiate(contents.GetChild(0), contents);

                newSlot.Initialize(contents.GetChild(i).GetComponent<RectTransform>());
                slotItemPanels.Add(newSlot);

                newSlot.ActionSetting(ClickEvent, itemPopup);
                newSlot.Setting(list[i].Id);
            }

            OnMoneyChange(Root.State.player.money.Value);
            Bind();
        }

        public override void Dispose()
        {
            UnBind();
            for (int i = 0; i < slotItemPanels.Count; i++)
                slotItemPanels[i].Dispose();

            base.Dispose();
        }

        void Bind()
        {
            Root.State.player.money.onChange += OnMoneyChange;
        }

        void UnBind()
        {
            Root.State.player.money.onChange -= OnMoneyChange;
        }

        void ClickEvent(int id)
        {
            var tableData = Root.GameDataManager.TableData.GetShopItemTableData(id);

            if (tableData == null || Root.State.player.money.Value < tableData.Buy)
                return;

            if (Root.State.inventory.IsFull && Root.State.inventory.GetItem_Id(id) == null)
                return;

            Root.State.inventory.AddInventory(id, 1);
            Root.State.player.money.Value -= tableData.Buy;
        }


        private void OnMoneyChange(int money)
        {
            for (int i = 0; i < slotItemPanels.Count; i++)
            {
                slotItemPanels[i].MoneySetting(money);
            }
        }
    }
}
