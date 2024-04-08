using UnityEngine;
using UnityEngine.UI;

namespace WATP.UI
{
    public class ShopPage : PageWidget
    {
        public static string DefaultPrefabPath { get => "ShopPage"; }

        protected Text money;
        protected ShopItemPanel shopItemPanel;
        protected ShopInventoryPanel shopInventoryPanel;


        protected override void OnLoad()
        {
            money = rectTransform.RecursiveFindChild("Txt_Money").GetComponent<Text>();
            closeButton = rectTransform.RecursiveFindChild("Bt_Close").GetComponent<Button>();

            var itemPopup = new ItemPopup();
            itemPopup.Initialize(rectTransform.RecursiveFindChild("ItemPopup"));

            shopItemPanel = new(itemPopup);
            shopItemPanel.Initialize(rectTransform.RecursiveFindChild("ShopItem"));

            shopInventoryPanel = new(itemPopup);
            shopInventoryPanel.Initialize(rectTransform.RecursiveFindChild("Inventory"));

            this.money.text = Root.State.player.money.Value.ToString();
            Bind();
        }

        protected override void OnDestroy()
        {
            shopItemPanel.Dispose();
            shopInventoryPanel.Dispose();
            UnBind();

            base.OnDestroy();
        }

        private void Bind()
        {
            Root.State.player.money.onChange += OnMoneyChange;
            closeButton.onClick.AddListener(BackPage);
        }

        private void UnBind()
        {
            Root.State.player.money.onChange -= OnMoneyChange;
            closeButton.onClick.RemoveAllListeners();
        }

        private void OnMoneyChange(int money)
        {
            this.money.text = money.ToString();
        }
    }
}