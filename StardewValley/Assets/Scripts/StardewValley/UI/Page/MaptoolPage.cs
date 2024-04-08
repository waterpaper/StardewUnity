using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WATP.UI
{
    public class MaptoolPage : PageWidget
    {
        private Image backImage;
        private ClickComponent backClick;
        private MaptoolSizeContainer sizeContainer;
        private MaptoolLayerContainer layerContainer;
        private TabMenu tabMenu;

        private MaptoolTileContainer tileContainer;
        private MaptoolObjectContainer objectContainer;
        private MaptoolAttributeContainer attributeContainer;
        private MaptoolEraseContainer eraseContainer;

        private Button backButton;
        private Button initButton;
        private Button saveButton;
        private Button loadButton;

        private InputField fileNameInput;
        private int selectIndex;

        private MaptoolCameraBounds cameraBounds;

        private Action OnHideEvent;
        private Action<int, int, int> OnClickEvent;


        public static string DefaultPrefabPath { get => "MaptoolPage"; }

        protected override void OnInit()
        {
            backImage = rectTransform.RecursiveFindChild("Back").GetComponent<Image>();
            backClick = rectTransform.RecursiveFindChild("Back").GetComponent<ClickComponent>();

            sizeContainer = new MaptoolSizeContainer();
            sizeContainer.Initialize(rectTransform.RecursiveFindChild("Top"));

            layerContainer = new MaptoolLayerContainer();
            layerContainer.Initialize(rectTransform.RecursiveFindChild("Top"));

            tileContainer = new MaptoolTileContainer();
            tileContainer.Initialize(rectTransform.RecursiveFindChild("TabMenu").RecursiveFindChild("TilePanel"));

            objectContainer = new MaptoolObjectContainer();
            objectContainer.Initialize(rectTransform.RecursiveFindChild("TabMenu").RecursiveFindChild("ObjectPanel"));

            attributeContainer = new MaptoolAttributeContainer();
            attributeContainer.Initialize(rectTransform.RecursiveFindChild("TabMenu").RecursiveFindChild("AttributePanel"));

            eraseContainer = new MaptoolEraseContainer();
            eraseContainer.Initialize(rectTransform.RecursiveFindChild("TabMenu").RecursiveFindChild("ErasePanel"));

            backButton = rectTransform.RecursiveFindChild("Bt_Back").GetComponent<Button>();
            initButton = rectTransform.RecursiveFindChild("Bt_Init").GetComponent<Button>();
            saveButton = rectTransform.RecursiveFindChild("Bt_Save").GetComponent<Button>();
            loadButton = rectTransform.RecursiveFindChild("Bt_Load").GetComponent<Button>();

            fileNameInput = rectTransform.RecursiveFindChild("TxtInput_MapFile").GetComponent<InputField>();
            tabMenu = rectTransform.RecursiveFindChild("TabMenu").GetComponent<TabMenu>();

            cameraBounds = new();

            Bind();
        }

        protected override void OnDestroy()
        {
            sizeContainer.Dispose();
            layerContainer.Dispose();
            tileContainer.Dispose();
            objectContainer.Dispose();
            attributeContainer.Dispose();
            eraseContainer.Dispose();
            UnBind();

            WATP.ECS.ECSController.ServiceEvents.Emit(new WATP.ECS.EventDeleteRoutine() { isAll = true });
            Root.SceneLoader.TileMapManager.Clear();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            cameraBounds.Update();
        }

        #region event

        private void Bind()
        {
            backButton.onClick.AddListener(OnCloseButton);
            initButton.onClick.AddListener(OnInitButton);
            saveButton.onClick.AddListener(OnSaveButton);
            loadButton.onClick.AddListener(OnLoadButton);

            tabMenu.OnSelectTab.AddListener(OnTabMenu);

            backClick.onClick += OnBackClickEvent;

            sizeContainer.onChangeEvent += OnSizeEvent;
            sizeContainer.onChangeEvent += cameraBounds.Setting;
        }

        private void UnBind()
        {
            backButton.onClick.RemoveAllListeners();
            initButton.onClick.RemoveAllListeners();
            saveButton.onClick.RemoveAllListeners();
            loadButton.onClick.RemoveAllListeners();

            tabMenu.OnSelectTab.RemoveAllListeners();
        }

        private void OnCloseButton()
        {
            BackPage();
        }

        private void OnInitButton()
        {
            Root.SceneLoader.TileMapManager.Clear();
            Root.SceneLoader.TileMapManager.SizeSetting(sizeContainer.XSize, sizeContainer.YSize);
            WATP.ECS.ECSController.ServiceEvents.Emit(new WATP.ECS.EventDeleteRoutine() { isAll = true });
        }

        private void OnSaveButton()
        {
            var saveData = Root.SceneLoader.TileMapManager.GetSaveForm();
            if (saveData.tilemaps[0].tileX == 0 || saveData.tilemaps[0].tileY == 0) return;
            if (string.IsNullOrEmpty(fileNameInput.text)) return;

            var data = Json.ObjectToJson(saveData);

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.dataPath + $"/Resources/Map/{fileNameInput.text}.dat");
            bf.Serialize(file, data);
            file.Close();
            Debug.Log("Game data saved!");
        }

        private void OnLoadButton()
        {
            if (string.IsNullOrEmpty(fileNameInput.text))
                return;

            Root.SceneLoader.TileMapManager.MapSetting(fileNameInput.text);
            cameraBounds.Setting((int)Root.SceneLoader.TileMapManager.TileSize.x, (int)Root.SceneLoader.TileMapManager.TileSize.y);
            sizeContainer.SetText((int)Root.SceneLoader.TileMapManager.TileSize.x, (int)Root.SceneLoader.TileMapManager.TileSize.y);

            WATP.ECS.ECSController.ServiceEvents.Emit(new WATP.ECS.EventDeleteRoutine() { isAll = true });
        }

        private void OnTabMenu(int index)
        {
            selectIndex = index;

            switch(selectIndex)
            {
                case 0:
                    OnHideEvent?.Invoke();

                    OnHideEvent = tileContainer.OnHide;
                    OnClickEvent = tileContainer.TileClickEvent;
                    break;
                case 1:
                    OnHideEvent?.Invoke();

                    OnHideEvent = objectContainer.OnHide;
                    OnClickEvent = objectContainer.TileClickEvent;
                    break;
                case 2:
                    OnHideEvent?.Invoke();

                    OnHideEvent = attributeContainer.OnHide;
                    OnClickEvent = attributeContainer.TileClickEvent;
                    break;
                case 3:
                    OnHideEvent?.Invoke();

                    OnHideEvent = eraseContainer.OnHide;
                    OnClickEvent = eraseContainer.TileClickEvent;
                    break;
            }
        }

        private void OnSizeEvent(int x, int y)
        {
            backImage.rectTransform.sizeDelta = new Vector2(x * 40, y * 40);
        }

        private void OnBackClickEvent(PointerEventData data)
        {
            var localCursor = Input.mousePosition;
            localCursor.z = 10.0f;
            localCursor = Camera.main.ScreenToWorldPoint(localCursor);

            Debug.Log("LocalCursor:" + localCursor);

            OnClickEvent?.Invoke(layerContainer.Layer, (int)localCursor.x, (int)localCursor.y);
        }

        #endregion

    }
}
