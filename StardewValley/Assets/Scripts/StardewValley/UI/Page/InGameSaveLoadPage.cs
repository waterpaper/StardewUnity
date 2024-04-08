using UnityEngine;
using UnityEngine.UI;

namespace WATP.UI
{
    public class InGameSaveLoadPage : PageWidget
    {
        //1- save, 2- load
        int type = 0;
        SaveLoadContainer file1 = new();
        SaveLoadContainer file2 = new();
        SaveLoadContainer file3 = new();

        public static string DefaultPrefabPath { get => "InGameSaveLoadPage"; }

        protected override void OnInit()
        {
            closeButton = rectTransform.RecursiveFindChild("Bt_Close").GetComponent<Button>();
            var content = rectTransform.RecursiveFindChild("Content");

            file1.Initialize(content.GetChild(0).GetComponent<RectTransform>());
            file2.Initialize(content.GetChild(1).GetComponent<RectTransform>());
            file3.Initialize(content.GetChild(2).GetComponent<RectTransform>());

            file1.Setting(type, 1);
            file2.Setting(type, 2);
            file3.Setting(type, 3);

            Bind();
            Root.State.logicState.Value = LogicState.Parse;
        }

        protected override void OnDestroy()
        {
            UnBind();

            if(type == 1)
                Root.State.logicState.Value = LogicState.Normal;
        }

        public void SetType(int type)
        {
            this.type = type;
            file1.Setting(type, 1, OnBackButton);
            file2.Setting(type, 2, OnBackButton);
            file3.Setting(type, 3, OnBackButton);

            if(type == 1)
                closeButton.gameObject.SetActive(false);
        }


        #region event

        private void Bind()
        {
            closeButton.onClick.AddListener(OnBackButton);
        }

        private void UnBind()
        {
            closeButton.onClick.RemoveAllListeners();
        }

        private void OnBackButton()
        {
            BackPage();
        }


        #endregion

    }
}
