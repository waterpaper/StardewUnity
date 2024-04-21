using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WATP.Player;

namespace WATP.UI
{
    public class InventoryItemPanel : UIElement
    {
        private Button btn;
        private ClickComponent clickComponent;
        private Image onBack;
        private Image offBack;
        private Image itemIcon;
        private ItemPopup itemPopup;

        private Text qtyText;
        private Image hover;
        private int itemId;
        private int index = -1;
        private Action<int> onClickEvent;


        public override void Initialize(RectTransform rect)
        {
            base.Initialize(rect);

            btn = rectTransform.GetComponent<Button>();
            clickComponent = btn.gameObject.AddComponent<ClickComponent>();

            onBack = rectTransform.RecursiveFindChild("Img_OnBack").GetComponent<Image>();
            offBack = rectTransform.RecursiveFindChild("Img_OffBack").GetComponent<Image>();
            itemIcon = rectTransform.RecursiveFindChild("Img_Icon").GetComponent<Image>();
            qtyText = rectTransform.RecursiveFindChild("Txt_Value").GetComponent<Text>();
            hover = rectTransform.RecursiveFindChild("Img_Hovor").GetComponent<Image>();

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
            clickComponent.onClick += OnClickEvent;
            onClickEvent = null;

            itemPopup = null;
        }

        public void ActionSetting(int index, Action<int> action)
        {
            this.index = index;
            onClickEvent += action;
        }


        public void Setting(ItemInfo info, ItemPopup itemPopup)
        {
            this.itemPopup = itemPopup;
            if (info == null)
            {
                itemIcon.sprite = null;
                itemIcon.gameObject.SetActive(false);
                qtyText.text = string.Empty;
                return;
            }

            var itemTableData = Root.GameDataManager.TableData.GetItemTableData(info.itemId);
            if (itemTableData == null)
                return;

            itemIcon.sprite = Root.GameDataManager.AtlasContainer.GetSheetSprite(itemTableData.ImagePath, $"{itemTableData.ImageName}_{itemTableData.Index}");
            itemIcon.gameObject.SetActive(true);
            qtyText.text = info.itemQty == 1 ? "" : $"{info.itemQty}";
            itemId = info.itemId;
            UnHighlight();
        }

        public void OnHighlight()
        {
            hover.gameObject.SetActive(true);
        }

        public void UnHighlight()
        {
            hover.gameObject.SetActive(false);
        }

        public void SetInteraction(bool isInteraction)
        {
            if(!isInteraction)
                UnHighlight();

            onBack.gameObject.SetActive(isInteraction);
            offBack.gameObject.SetActive(!isInteraction);
            btn.interactable = isInteraction;
        }

        void OnClickEvent(PointerEventData e)
        {
            if (e.button != PointerEventData.InputButton.Right || !btn.interactable) return;

            onClickEvent?.Invoke(index);
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
