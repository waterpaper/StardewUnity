using System;
using UnityEngine;
using UnityEngine.UI;
using WATP.Map;

namespace WATP.UI
{
    public class MaptoolSizeContainer : UIElement
    {
        private int x = 0;
        private int y = 0;

        private Text sizeText;
        private Button sizeButton;
        private InputField xSize;
        private InputField ySize;


        public Action<int, int> onChangeEvent;
        public int XSize { get => x; }
        public int YSize { get => y; }


        public override void Initialize(RectTransform rect)
        {
            base.Initialize(rect);

            sizeText = rectTransform.RecursiveFindChild("Txt_Size").GetComponent<Text>();
            sizeButton = rectTransform.RecursiveFindChild("Bt_Size").GetComponent<Button>();
            xSize = rectTransform.RecursiveFindChild("TxtInput_SizeX").GetComponent<InputField>();
            ySize = rectTransform.RecursiveFindChild("TxtInput_SizeY").GetComponent<InputField>();

            sizeText.text = $"{x} x {y}";
            Bind();
        }

        public override void Dispose()
        {
            UnBind();

            base.Dispose();
        }

        void Bind()
        {
            sizeButton.onClick.AddListener(OnChangeButton);
        }

        void UnBind()
        {
            sizeButton.onClick.RemoveAllListeners();
            onChangeEvent = null;
        }


        void OnChangeButton()
        {
            if (string.IsNullOrEmpty(xSize.text) || string.IsNullOrEmpty(ySize.text))
                return;

            x = int.Parse(xSize.text);
            y = int.Parse(ySize.text);

            Root.SceneLoader.TileMapManager.SizeSetting(x, y);
            sizeText.text = $"{x} x {y}";
            WATP.ECS.ECSController.ServiceEvents.Emit(new WATP.ECS.EventDeleteRoutine() { isAll = true });
            onChangeEvent?.Invoke(x, y);
        }


        public void SetText(int x, int y)
        {
            sizeText.text = $"{x} x {y}";
        }
    }
}
