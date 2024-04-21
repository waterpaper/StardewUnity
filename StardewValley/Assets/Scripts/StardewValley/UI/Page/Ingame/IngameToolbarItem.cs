using UnityEngine;
using UnityEngine.UI;
using WATP.Player;

namespace WATP.UI
{
    public class IngameToolbarItem : UIElement
    {
        private Image iconImg;
        private Text valueTxt;
        private Image hoverImg;

        public override void Initialize(RectTransform rect)
        {
            base.Initialize(rect);

            iconImg = rectTransform.RecursiveFindChild("Img_Icon").GetComponent<Image>();
            valueTxt = rectTransform.RecursiveFindChild("Txt_Value").GetComponent<Text>();
            hoverImg = rectTransform.RecursiveFindChild("Img_Hovor").GetComponent<Image>();

            UnHighlight();
        }

        public void Setting(ItemInfo info)
        {
            if(info == null)
            {
                iconImg.sprite = null;
                iconImg.gameObject.SetActive(false);
                valueTxt.text = string.Empty;
                return;
            }

            var itemTableData = Root.GameDataManager.TableData.GetItemTableData(info.itemId);
            if (itemTableData == null)
                return;

            iconImg.sprite = Root.GameDataManager.AtlasContainer.GetSheetSprite(itemTableData.ImagePath, $"{itemTableData.ImageName}_{itemTableData.Index}");
            iconImg.gameObject.SetActive(true);
            valueTxt.text = info.itemQty == 1 ? "" : $"{info.itemQty}";
        }

        public void OnHighlight()
        {
            hoverImg.gameObject.SetActive(true);
        }

        public void UnHighlight()
        {
            hoverImg.gameObject.SetActive(false);
        }
    }
}
