using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WATP.UI
{
    public class InventoryPanel : UIElement
    {
        private int hoverIndex = -1;

        private List<InventoryItemPanel> inventoryItemPanels = new();

        private Text nameText;
        private Text farmText;
        private Text moneyText;

        private ItemPopup itemPopup;
        private Image farmerBody, farmerHair, farmerPants, farmerShirts;


        public override void Initialize(RectTransform rect)
        {
            base.Initialize(rect);

            nameText = rectTransform.RecursiveFindChild("Txt_Name").GetComponent<Text>();
            farmText = rectTransform.RecursiveFindChild("Txt_Farm").GetComponent<Text>();
            moneyText = rectTransform.RecursiveFindChild("Txt_Money").GetComponent<Text>();

            farmerBody = rectTransform.RecursiveFindChild("Img_Body").GetComponent<Image>();
            farmerHair = rectTransform.RecursiveFindChild("Img_Hair").GetComponent<Image>();
            farmerPants = rectTransform.RecursiveFindChild("Img_Pants").GetComponent<Image>();
            farmerShirts = rectTransform.RecursiveFindChild("Img_Shirts").GetComponent<Image>();

            itemPopup = new();
            itemPopup.Initialize(rectTransform.RecursiveFindChild("ItemPopup"));

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
            CustomSetting();
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
        }

        void UnBind()
        {
        }

        void CustomSetting()
        {
            farmerHair.sprite = Root.GameDataManager.AtlasContainer.GetSheetSprite(Root.GameDataManager.AtlasContainer.FARMER_HAIR_PATH, $"FarmerHairs_{Root.State.player.hairIndex.Value - 1}");
            var hairColor = Root.State.player.hairColor;
            farmerHair.color = new Color(hairColor.Value.x / 255.0f, hairColor.Value.y / 255.0f, hairColor.Value.z / 255.0f);
            farmerShirts.sprite = Root.GameDataManager.AtlasContainer.GetSheetSprite(Root.GameDataManager.AtlasContainer.FARMER_SHIRTS_PATH, $"FarmerShirts_{Root.State.player.clothsIndex.Value - 1}");

            var pantsColor = Root.State.player.clothsColor;
            farmerPants.color = new Color(pantsColor.Value.x / 255.0f, pantsColor.Value.y / 255.0f, pantsColor.Value.z / 255.0f);

            nameText.text = $"ÀÌ¸§ : {Root.State.player.name.Value}";
            farmText.text = $"³óÀåÀÌ¸§ : {Root.State.player.farmName.Value}";
            moneyText.text = $"µ· : {Root.State.player.money.Value}";
        }

        void InventorySetting()
        {
            for (int i = 0; i < inventoryItemPanels.Count; i++)
            {
                var item = Root.State.inventory.GetItem_Index(i);
                inventoryItemPanels[i].Setting(item, itemPopup);
            }
        }

        void ClickEvent(int index)
        {
            if (hoverIndex != -1)
            {
                if (hoverIndex == index)
                {
                    inventoryItemPanels[index].UnHighlight();
                    hoverIndex = -1;
                    return;
                }

                Root.State.inventory.Swap(hoverIndex, index);
                inventoryItemPanels[hoverIndex].Setting(Root.State.inventory.GetItem_Index(hoverIndex), itemPopup);
                inventoryItemPanels[index].Setting(Root.State.inventory.GetItem_Index(index), itemPopup);
                inventoryItemPanels[hoverIndex].UnHighlight();
                hoverIndex = -1;
            }
            else
            {
                var item = Root.State.inventory.GetItem_Index(index);
                if (item != null)
                {
                    inventoryItemPanels[index].OnHighlight();
                    hoverIndex = index;
                }
            }
        }
    }
}
