using System;
using UnityEngine;
using UnityEngine.UI;

namespace WATP.UI
{
    public class IngameGetItemPanel : UIElement
    {
        public int itemId;
        public int count;

        private Image icon;
        private Text nameTxt;
        private Text countTxt;
        private float timer;

        public bool IsTimeOver { get => timer > 5; }


        public override void Initialize(RectTransform rect)
        {
            base.Initialize(rect);

            icon = rectTransform.RecursiveFindChild("Img_Icon").GetComponent<Image>();
            nameTxt = rectTransform.RecursiveFindChild("Txt_Name").GetComponent<Text>();
            countTxt = rectTransform.RecursiveFindChild("Txt_Count").GetComponent<Text>();

        }

        public void Update()
        {
            timer += Time.deltaTime;
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public void Setting(int id, int qty)
        {
            this.itemId = id;
            this.count += qty;
            var itemTableData =Root.GameDataManager.TableData.GetItemTableData(id);
            if (itemTableData == null) return;

            icon.sprite = Root.GameDataManager.AtlasContainer.GetSheetSprite(itemTableData.ImagePath, $"{itemTableData.ImageName}_{itemTableData.Index}");
            nameTxt.text = itemTableData.Name;
            countTxt.text = count == 1 ? "" : $"{count}";
            timer = 0;
        }


    }
}
