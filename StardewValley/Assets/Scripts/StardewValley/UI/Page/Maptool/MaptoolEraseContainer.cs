using UnityEngine;
using UnityEngine.UI;

namespace WATP.UI
{
    public class MaptoolEraseContainer : UIElement
    {
        private Text nowText;
        private Button tileButton;
        private Button objectButton;
        private Button attributeButton;

        private int type;

        public override void Initialize(RectTransform rect)
        {
            base.Initialize(rect);

            nowText = rectTransform.RecursiveFindChild("Txt_Now").GetComponent<Text>();
            tileButton = rectTransform.RecursiveFindChild("Bt_Tile").GetComponent<Button>();
            objectButton = rectTransform.RecursiveFindChild("Bt_Object").GetComponent<Button>();
            attributeButton = rectTransform.RecursiveFindChild("Bt_Attribute").GetComponent<Button>();
            Bind();

            OnHide();
        }

        public override void Dispose()
        {
            UnBind();
            base.Dispose();
        }

        public void OnHide()
        {
            type = 1;
            TypeText();
        }

        public void TileClickEvent(int layer, int x, int y)
        {
            if (type == 1)
            {
                Root.SceneLoader.TileMapManager.TileImageSetting(1, x, y, string.Empty, string.Empty, 0);
                Root.SceneLoader.TileMapManager.TileImageSetting(2, x, y, string.Empty, string.Empty, 0);
            }
            else if (type == 2)
            {
                var cell = Root.SceneLoader.TileMapManager.GetCell(x, y);
                if (cell == null) return;
                Root.SceneLoader.TileMapManager.TileObjectRemove(x, y);
                WATP.ECS.ECSController.ServiceEvents.Emit(new WATP.ECS.EventDeleteRoutine() { isRemove = true, posX = cell.Position.x, posY = cell.Position.y });
            }
            else if (type == 3)
            {
                Root.SceneLoader.TileMapManager.TileAttributeSetting(x, y, 'C');
            }
        }

        void Bind()
        {
            tileButton.onClick.AddListener(() => { type = 1; TypeText(); });
            objectButton.onClick.AddListener(() => { type = 2; TypeText(); });
            attributeButton.onClick.AddListener(() => { type = 3; TypeText(); });
        }

        void UnBind()
        {
            tileButton.onClick.RemoveAllListeners();
            objectButton.onClick.RemoveAllListeners();
            attributeButton.onClick.RemoveAllListeners();
        }

        private void TypeText()
        {
            if (type == 0)
            {
                nowText.text = "-";
            }
            else if (type == 1)
            {
                nowText.text = "타일";
            }
            else if (type == 2)
            {
                nowText.text = "오브젝트";
            }
            else if (type == 3)
            {
                nowText.text = "속성";
            }
        }
    }
}
