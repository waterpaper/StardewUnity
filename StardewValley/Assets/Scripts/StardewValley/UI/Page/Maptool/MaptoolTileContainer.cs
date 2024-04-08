using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WATP.UI
{
    public class MaptoolTileContainer : UIElement
    {
        private Image tilesheet;
        private Button loadButton;
        private InputField fileInput;
        private ClickComponent clickComponent;

        private GameObject hoverObject;

        private string filePath;
        private int index;

        private int totalXIndex, totalYIndex;

        public override void Initialize(RectTransform rect)
        {
            base.Initialize(rect);

            tilesheet = rectTransform.RecursiveFindChild("Image").GetComponent<Image>();
            loadButton = rectTransform.RecursiveFindChild("Bt_ImageLoad").GetComponent<Button>();
            fileInput = rectTransform.RecursiveFindChild("TxtInput_TileSheet").GetComponent<InputField>();
            clickComponent = rectTransform.RecursiveFindChild("Image").GetComponent<ClickComponent>();

            hoverObject = rectTransform.RecursiveFindChild("Hover").gameObject;
            hoverObject.SetActive(false);

            clickComponent.onDown += OnDownEvent;
            totalXIndex = 0;
            totalYIndex = 0;
            Bind();
        }

        public override void Dispose()
        {
            UnBind();
            base.Dispose();
        }

        public void OnHide()
        {
            index = -1;
            hoverObject.SetActive(false);
        }

        public void TileClickEvent(int layer, int x, int y)
        {
            if (index == -1) return;
            if (string.IsNullOrEmpty(filePath)) return;

            if (filePath.Contains('/'))
            {
                var spilts = filePath.Split('/');

                Root.SceneLoader.TileMapManager.TileImageSetting(layer, x, y, spilts[0], spilts[1], index);
            }
            else
            {
                Root.SceneLoader.TileMapManager.TileImageSetting(layer, x, y, null, filePath, index);
            }
        }

        void Bind()
        {
            loadButton.onClick.AddListener(OnLoadButton);
        }

        void UnBind()
        {
            loadButton.onClick.RemoveAllListeners();
        }

        private void OnLoadButton()
        {
            if (string.IsNullOrEmpty(fileInput.text)) return;

            try
            {
                var sprite = AssetLoader.Load<Sprite>($"Address/Sprite/TileSheets/Maptool/{fileInput.text}.png");
                    tilesheet.GetComponent<RectTransform>().sizeDelta = new Vector2(sprite.texture.width, sprite.texture.height);
                    tilesheet.sprite = sprite;

                totalXIndex = sprite.texture.width / 16;
                totalYIndex = sprite.texture.height / 16;
                hoverObject.SetActive(false);

                filePath = fileInput.text;
                index = -1;
            }
            catch
            {
                Debug.Log("not file");
                return;
            }
        }

        private void OnDownEvent(PointerEventData data)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(tilesheet.GetComponent<RectTransform>(),
                data.position, data.pressEventCamera, out Vector2 localCursor))
                return;

            localCursor *= new Vector2(1, -1);
            Debug.Log("LocalCursor:" + localCursor);

            int x = (int)(localCursor.x / 16);
            int y = (int)(localCursor.y / 16);

            hoverObject.GetComponent<RectTransform>().localPosition = new Vector2(x * 16, -y * 16);
            hoverObject.SetActive(true);
            index = y * totalXIndex + x;
        }
    }
}
