using System;
using UnityEngine;
using UnityEngine.UI;

namespace WATP.UI
{
    public class MenuPopup : PopupWidget
    {
        public static string DefaultPrefabPath { get => "MenuPopup"; }

        public TabMenu tabMenu;
        public Action onCloseEvent;

        public InventoryPanel inventoryPanel;
        public LikeabilityPanel likeabilityPanel;
        public MapPanel mapPanel;
        public CreatePanel createPanel;
        public InfoPanel infoPanel;
        public PreferencePanel preferencePanel;

        protected override void OnInit()
        {
            inventoryPanel = new();
            likeabilityPanel = new();
            mapPanel = new();
            createPanel = new();
            infoPanel = new();
            preferencePanel = new();

            inventoryPanel.Initialize(rectTransform.RecursiveFindChild("InventoryPanel"));
            likeabilityPanel.Initialize(rectTransform.RecursiveFindChild("LikeabilityPanel"));
            mapPanel.Initialize(rectTransform.RecursiveFindChild("MapPanel"));
            createPanel.Initialize(rectTransform.RecursiveFindChild("CreatePanel"));
            infoPanel.Initialize(rectTransform.RecursiveFindChild("InfoPanel"));
            preferencePanel.Initialize(rectTransform.RecursiveFindChild("PreferencePanel"));

            tabMenu = rectTransform.RecursiveFindChild("TabMenu").GetComponent<TabMenu>();
            closeButton = rectTransform.RecursiveFindChild("Bt_Close").GetComponent<Button>();

            Root.State.logicState.Value = LogicState.Parse;
            Bind();
        }

        protected override void OnDestroy()
        {
            onCloseEvent?.Invoke();
            inventoryPanel.Dispose();
            likeabilityPanel.Dispose();
            mapPanel.Dispose();
            createPanel.Dispose();
            infoPanel.Dispose();
            preferencePanel.Dispose();

            UnBind();
            base.OnDestroy();
        }

        protected override void PopupOptionSetting(PopupOption popupOption)
        {
            popupOption.isBlur = false;
            popupOption.isBlockerAni = false;
            popupOption.blockerColor = new Color(0, 0, 0, 0);
            popupOption.isOutClickClose = false;
        }


        private void Bind()
        {
            closeButton.onClick.AddListener(OnCloseAction);
        }

        private void UnBind()
        {
            closeButton.onClick.RemoveAllListeners();
        }

        private void OnCloseAction()
        {
            Root.State.logicState.Value = LogicState.Normal;
            ClosePopup();
        }
    }
}
