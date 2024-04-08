using System;
using UnityEngine;
using UnityEngine.UI;

namespace WATP.UI
{
    public class MaptoolAttributeContainer : UIElement
    {
        private Text nowText;
        private Button normalButton;
        private Button farmButton;
        private Button blockButton;
        private Button waterButton;

        private Button warpButton;
        private InputField connectMap;
        private InputField warpX;
        private InputField warpY;

        private int type;


        public override void Initialize(RectTransform rect)
        {
            base.Initialize(rect);

            nowText = rectTransform.RecursiveFindChild("Txt_Now").GetComponent<Text>();
            normalButton = rectTransform.RecursiveFindChild("Bt_Normal").GetComponent<Button>();
            farmButton = rectTransform.RecursiveFindChild("Bt_Farm").GetComponent<Button>();
            blockButton = rectTransform.RecursiveFindChild("Bt_Block").GetComponent<Button>();
            waterButton = rectTransform.RecursiveFindChild("Bt_Water").GetComponent<Button>();

            warpButton = rectTransform.RecursiveFindChild("Bt_Warp").GetComponent<Button>();
            connectMap = rectTransform.RecursiveFindChild("TxtInput_WarpMap").GetComponent<InputField>();
            warpX = rectTransform.RecursiveFindChild("TxtInput_WarpX").GetComponent<InputField>();
            warpY = rectTransform.RecursiveFindChild("TxtInput_WarpY").GetComponent<InputField>();

            Bind();
        }

        public override void Dispose()
        {
            UnBind();
            base.Dispose();
        }

        public void OnHide()
        {
            type = 0;
            TypeText();
        }

        void Bind()
        {
            normalButton.onClick.AddListener(() => { type = 1; TypeText(); });
            farmButton.onClick.AddListener(() => { type = 2; TypeText(); });
            waterButton.onClick.AddListener(() => { type = 3; TypeText(); });
            blockButton.onClick.AddListener(() => { type = 4; TypeText(); });
            warpButton.onClick.AddListener(() => { type = 5; TypeText(); });
        }

        void UnBind()
        {
            normalButton.onClick.RemoveAllListeners();
            farmButton.onClick.RemoveAllListeners();
            blockButton.onClick.RemoveAllListeners();
            waterButton.onClick.RemoveAllListeners();
            warpButton.onClick.RemoveAllListeners();
        }

        public void TileClickEvent(int layer, int x, int y)
        {
            char kind = 'C';
            if (type == 0)
                kind = 'C';
            else if (type == 1)
                kind = 'C';
            else if (type == 2)
                kind = 'F';
            else if (type == 3)
                kind = 'W';
            else if (type == 4)
                kind = 'B';
            else if (type == 5)
                kind = 'P';


            if (type == 5)
            {
                if (string.IsNullOrEmpty(connectMap.text) || string.IsNullOrEmpty(warpX.text) || string.IsNullOrEmpty(warpY.text))
                    return;

                Root.SceneLoader.TileMapManager.TileAttributeSetting(x, y, kind, connectMap.text, float.Parse(warpX.text), float.Parse(warpY.text));
            }
            else
                Root.SceneLoader.TileMapManager.TileAttributeSetting(x, y, kind);
        }

        private void TypeText()
        {
            if (type == 0)
            {
                nowText.text = "-";
            }
            else if (type == 1)
            {
                nowText.text = "일반";
            }
            else if (type == 2)
            {
                nowText.text = "농사타일";
            }
            else if(type == 3)
            {
                nowText.text = "물";
            }
            else if (type == 4)
            {
                nowText.text = "이동불가";
            }
            else if (type == 5)
            {
                nowText.text = "워프";
            }
        }

    }
}
