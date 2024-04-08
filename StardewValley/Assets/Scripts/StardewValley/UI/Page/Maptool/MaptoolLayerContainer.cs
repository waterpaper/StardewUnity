using UnityEngine;
using UnityEngine.UI;
using WATP.Data;

namespace WATP.UI
{
    public class MaptoolLayerContainer : UIElement
    {
        private int layer = 1;

        private Text layerText;

        private Button upButton;
        private Button downButton;

        public int Layer { get => layer; }


        public override void Initialize(RectTransform rect)
        {
            base.Initialize(rect);

            layerText = rectTransform.RecursiveFindChild("Txt_Layer").GetComponent<Text>();

            upButton = rectTransform.RecursiveFindChild("Bt_Up").GetComponent<Button>();
            downButton = rectTransform.RecursiveFindChild("Bt_Down").GetComponent<Button>();

            Bind();
            layerText.text = $"{layer}";
        }

        public override void Dispose()
        {
            UnBind();

            base.Dispose();
        }

        void Bind()
        {
            upButton.onClick.AddListener(OnUpButton);
            downButton.onClick.AddListener(OnDownButton);
        }

        void UnBind()
        {
            upButton.onClick.RemoveAllListeners();
            downButton.onClick.RemoveAllListeners();
        }


        void OnUpButton()
        {
            if (layer >= Config.MAP_LAYER_MAX)
                return;

            layer++;
            layerText.text = $"{layer}";
        }

        void OnDownButton()
        {
            if (layer <= 1)
                return;

            layer--;
            layerText.text = $"{layer}";
        }
    }
}
