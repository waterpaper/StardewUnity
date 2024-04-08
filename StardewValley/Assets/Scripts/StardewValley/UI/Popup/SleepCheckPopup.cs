using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace WATP.UI
{
    public class SleepCheckPopup : PopupWidget
    {
        public static string DefaultPrefabPath { get => "SleepCheckPopup"; }

        public Button yesBtn;
        public Button noBtn;

        protected override void OnInit()
        {
            yesBtn = rectTransform.RecursiveFindChild("Bt_Okay").GetComponent<Button>();
            noBtn = rectTransform.RecursiveFindChild("Bt_Not").GetComponent<Button>();
            closeButton = rectTransform.RecursiveFindChild("Bt_Close").GetComponent<Button>();

            Root.State.logicState.Value = LogicState.Parse;
            Bind();
        }

        protected override void OnDestroy()
        {
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
            yesBtn.onClick.AddListener(OnYesButton);
            noBtn.onClick.AddListener(OnCloseButton);
            closeButton.onClick.AddListener(OnCloseButton);
        }

        private void UnBind()
        {
            yesBtn.onClick.RemoveAllListeners();
            noBtn.onClick.RemoveAllListeners();
            closeButton.onClick.RemoveAllListeners();
        }

        public void OnCloseButton()
        {
            ClosePopup();
            Root.State.logicState.Value = LogicState.Normal;
        }

        public void OnYesButton()
        {
            Root.State.TodayUpdateSetting();
            ClosePopup();
            OnLoadPage().Forget();
        }

        private async UniTaskVoid OnLoadPage()
        {
            var inGameSaveLoadPage = new InGameSaveLoadPage();
            inGameSaveLoadPage = await UIManager.Widgets.CreateAsync<InGameSaveLoadPage>(inGameSaveLoadPage, InGameSaveLoadPage.DefaultPrefabPath);
            inGameSaveLoadPage.SetType(1);
        }

    }
}
