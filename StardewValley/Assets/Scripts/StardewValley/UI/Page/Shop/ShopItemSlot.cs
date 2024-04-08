using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WATP.UI
{
    public class ShopItemSlot : UIElement
    {
        private ClickComponent clickComponent;
        private Image itemIcon;
        private ItemPopup itemPopup;

        private Text nameText;
        private Text moneyText;
        private int itemId;
        private Action<int> onClickEvent;


        public override void Initialize(RectTransform rect)
        {
            base.Initialize(rect);
            clickComponent = rectTransform.AddComponent<ClickComponent>();

            itemIcon = rectTransform.RecursiveFindChild("Img_Icon").GetComponent<Image>();
            nameText = rectTransform.RecursiveFindChild("Txt_Name").GetComponent<Text>();
            moneyText = rectTransform.RecursiveFindChild("Txt_Money").GetComponent<Text>();

            Bind();
        }

        public override void Dispose()
        {
            UnBind();
            base.Dispose();
        }
        void Bind()
        {
            clickComponent.onEnter += OnEnterEvent;
            clickComponent.onMove += OnMoveEvent;
            clickComponent.onExit += OnExitEvent;
            clickComponent.onClick += OnClickEvent;
        }

        void UnBind()
        {
            clickComponent.onEnter -= OnEnterEvent;
            clickComponent.onMove -= OnMoveEvent;
            clickComponent.onExit -= OnExitEvent;
            clickComponent.onClick -= OnClickEvent;
            onClickEvent = null;

            itemPopup = null;
        }

        public void ActionSetting(Action<int> action, ItemPopup itemPopup)
        {
            onClickEvent += action;
            this.itemPopup = itemPopup;
        }


        public void Setting(int itemId)
        {
            if (itemId == 0)
            {
                itemIcon.sprite = null;
                itemIcon.gameObject.SetActive(false);
                moneyText.text = string.Empty;
                return;
            }

            var itemTableData = Root.GameDataManager.TableData.GetItemTableData(itemId);
            if (itemTableData == null)
                return;

            var shopItmeTableData = Root.GameDataManager.TableData.GetShopItemTableData(itemId);
            itemIcon.sprite = Root.GameDataManager.AtlasContainer.GetSheetSprite(itemTableData.ImagePath, $"{itemTableData.ImageName}_{itemTableData.Index}");
            itemIcon.gameObject.SetActive(true);
            nameText.text = itemTableData.Name;
            moneyText.text = shopItmeTableData == null ? "0" : $"{shopItmeTableData.Buy}";
            this.itemId = itemId;
        }

        public void MoneySetting(int money)
        {
            if (itemId == 0) return;

            var shopItmeTableData = Root.GameDataManager.TableData.GetShopItemTableData(itemId);

            if(shopItmeTableData.Buy > money)
                moneyText.color = new Color(1,0,0);
            else
                moneyText.color = new Color(0, 0, 0);
        }

        void OnClickEvent(PointerEventData e)
        {
            if (e.button != PointerEventData.InputButton.Right) return;

            onClickEvent?.Invoke(itemId);
        }

        void OnEnterEvent(PointerEventData e)
        {
            if (!itemIcon.gameObject.activeSelf) return;

            itemPopup.RectTransform.gameObject.SetActive(true);

            /*   var pos = UIManager.Camera.WorldToScreenPoint(rectTransform.position);
                Vector2 movePos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    UIManager.RectTransform,
                    pos, UIManager.Camera,
                    out movePos);*/

            itemPopup.SetItemId(itemId);
            itemPopup.SetPos(rectTransform.position.x, rectTransform.position.y);
        }

        void OnMoveEvent(PointerEventData e)
        {
            if (!itemIcon.gameObject.activeSelf) return;
            /* if (itemPopup != null)
             {
                 Vector2 movePos;
                 RectTransformUtility.ScreenPointToLocalPointInRectangle(
                     UIManager.RectTransform,
                     Input.mousePosition, UIManager.Camera,
                     out movePos);
                 itemPopup.SetPos(movePos.x, movePos.y);
             }*/
        }

        void OnExitEvent(PointerEventData e)
        {
            if (!itemIcon.gameObject.activeSelf) return;
            itemPopup.RectTransform.gameObject.SetActive(false);
        }
    }
}
