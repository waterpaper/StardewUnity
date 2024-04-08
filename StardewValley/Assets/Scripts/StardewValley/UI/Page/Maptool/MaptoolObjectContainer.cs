using System;
using UnityEngine;
using UnityEngine.UI;

namespace WATP.UI
{
    public class MaptoolObjectContainer : UIElement
    {
        private int index = 1001;
        private Image objectImg;
        private Button leftButton;
        private Button rightButton;


        public override void Initialize(RectTransform rect)
        {
            base.Initialize(rect);

            objectImg = rectTransform.RecursiveFindChild("Img_Icon").GetComponent<Image>();
            leftButton = rectTransform.RecursiveFindChild("Bt_Left").GetComponent<Button>();
            rightButton = rectTransform.RecursiveFindChild("Bt_Right").GetComponent<Button>();
            Bind();
            SetImageSetting();
        }

        public override void Dispose()
        {
            UnBind();
            base.Dispose();
        }

        public void OnHide()
        {
            index = 1001;
            SetImageSetting();
        }

        void Bind()
        {
            leftButton.onClick.AddListener(OnLeftButton);
            rightButton.onClick.AddListener(OnRightButton);
        }

        void UnBind()
        {
            leftButton.onClick.RemoveAllListeners();
            rightButton.onClick.RemoveAllListeners();
        }

        public void TileClickEvent(int layer, int x, int y)
        {
            var objData = Root.SceneLoader.TileMapManager.TileObjectAdd(x, y, index);

            var mapObject = new WATP.ECS.MapObjectEntity.MapObjectEntityBuilder()
                .SetPos(new Vector2(objData.posX, objData.posY))
                .SetId(objData.objectId);

            WATP.ECS.ECSController.ServiceEvents.Emit(new WATP.ECS.EventAddEntity(mapObject));
            WATP.ECS.ECSController.ServiceEvents.Emit(new WATP.ECS.EventCreateRoutine());
        }

        void SetImageSetting()
        {
            var tableData = Root.GameDataManager.TableData.GetObjectTableData(index);
            objectImg.sprite = Root.GameDataManager.AtlasContainer.GetSheetSprite(tableData.ImagePath, $"{tableData.ImageName}_{tableData.Index}");

            objectImg.rectTransform.sizeDelta = new Vector2(objectImg.sprite.rect.width * 2, objectImg.sprite.rect.height * 2);
        }

        void OnLeftButton()
        {
            if (index <= 1001)
                return;

            index--;
            SetImageSetting();
        }

        void OnRightButton()
        {
            var tableData = Root.GameDataManager.TableData.GetObjectTableData(index);
            if (tableData == null)
                return;

            index++;
            SetImageSetting();
        }

    }
}
