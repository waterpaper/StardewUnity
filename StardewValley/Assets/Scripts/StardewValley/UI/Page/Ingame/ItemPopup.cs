using System;
using UnityEngine;
using UnityEngine.UI;

namespace WATP.UI
{
    public class ItemPopup : UIElement
    {
        public int itemId;
        public float posX;
        public float posY;

        public Text nameText;
        public Text categoryText;

        public Text descText;
        public RectTransform hpArea;
        public Text hpText;
        public RectTransform powerArea;
        public Text powerText;

        public override void Initialize(RectTransform rect)
        {
            base.Initialize(rect);
            nameText = rectTransform.RecursiveFindChild("Txt_Name").GetComponent<Text>();
            categoryText = rectTransform.RecursiveFindChild("Txt_Category").GetComponent<Text>();
            descText = rectTransform.RecursiveFindChild("Txt_Desc").GetComponent<Text>();
            hpArea = rectTransform.RecursiveFindChild("HpArea");
            hpText = rectTransform.RecursiveFindChild("Txt_AddHp").GetComponent<Text>();
            powerArea = rectTransform.RecursiveFindChild("PowerArea");
            powerText = rectTransform.RecursiveFindChild("Txt_AddPower").GetComponent<Text>();

            rectTransform.position = new Vector3(posX, posY, 0);
            Bind();
            SetItemInfo();
        }
        public override void Dispose()
        {
            UnBind();

            base.Dispose();
        }

        private void Bind()
        {
        }

        private void UnBind()
        {
        }

        public void SetPos(float x, float y)
        {
            posX = x;
            posY = y;

            if (rectTransform != null)
            {
                if (posX + rectTransform.rect.width > 800)
                {
                    posX = posX - (rectTransform.rect.width + 50);
                }

                if (posY + rectTransform.rect.height > 1200)
                {
                    posY = posY + rectTransform.rect.height - ((posY + rectTransform.rect.height) - 1200);
                }

                rectTransform.position = new Vector3(posX, posY, 0);
                rectTransform.anchoredPosition3D = new Vector3(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y, 0);
            }
        }

        public void SetItemId(int itemId)
        {
            this.itemId = itemId;
            SetItemInfo();
        }

        public void SetItemInfo()
        {
            var itemTableData = Root.GameDataManager.TableData.GetItemTableData(itemId);
            if (itemTableData == null)
                return;

            nameText.text = itemTableData.Name;
            categoryText.text = Root.GameDataManager.TableData.CategoryStr((ECategory)itemTableData.Type);
            descText.text = itemTableData.Desc;


            var eatTableData = Root.GameDataManager.TableData.GetEatTableData(itemId);
            if (eatTableData == null)
            {
                hpArea.gameObject.SetActive(false);
                powerArea.gameObject.SetActive(false);
                return;
            }

            hpArea.gameObject.SetActive(true);
            powerArea.gameObject.SetActive(true);

            hpText.text = $"체력 + {eatTableData.AddHp}";
            powerText.text = $"행동력 + {eatTableData.AddPower}";
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
    }
}
