using UnityEngine;
using WATP.ECS;

namespace WATP.View
{
    public class MapObjectView : View<MapObjectAspect>, IGridView
    {
        protected float multiply = 1;

        protected SpriteRenderer spriteRenderer;
        protected Animator destroyAnim;
        protected SpriteRenderer rect;
        protected EventActionComponent eventActionComponent;


        public MapObjectView(MapObjectAspect mapObject, EventActionComponent eventActionComponent, Transform parent)
        {
            entity = mapObject;
            Parent = parent;

            PrefabPath = $"Address/Prefab/MapObject.prefab";
            this.eventActionComponent = eventActionComponent;
            this.uid = entity.Index;
        }

        protected override void OnLoad()
        {
            this.uid = entity.Index;

            Transform.position = entity.Position;
            spriteRenderer = Transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
            destroyAnim = Transform.GetChild(0).GetChild(1).GetComponent<Animator>();
            rect = Transform.GetChild(0).GetChild(2).GetComponent<SpriteRenderer>();
            eventActionComponent.OnEvent += StateUpdate;

            destroyAnim.SetInteger("Id", entity.MapObjectDataComponent.id);

            var tableData = Root.GameDataManager.TableData.GetObjectTableData(entity.MapObjectDataComponent.id);
            spriteRenderer.sprite = Root.GameDataManager.AtlasContainer.GetSheetSprite(tableData.ImagePath, $"{tableData.ImageName}_{tableData.Index}");

            spriteRenderer.transform.localPosition = new Vector3(tableData.ViewX, tableData.ViewY, 0);

            rect.transform.localScale = new Vector3(tableData.Width, tableData.Height, 1);
            rect.gameObject.SetActive(Root.GameDataManager.Preferences.IsGrid);
        }

        protected override void OnDestroy()
        {
            eventActionComponent.OnEvent -= StateUpdate;
            eventActionComponent = null;
            entity = default;
        }

        protected override void OnRender()
        {
        }

        void StateUpdate(int state)
        {
            if (entity.Equals(default) || entity.DeleteComponent.isDelate || isPrefab == false) return;
            switch (state)
            {
                case (int)EventKind.Hit:
                    break;
                case (int)EventKind.Destroy:
                    spriteRenderer.gameObject.SetActive(false);
                    destroyAnim.SetTrigger("Start");

                    var tableData = Root.GameDataManager.TableData.GetObjectTableData(entity.MapObjectDataComponent.id);
                    if (tableData.ToolsType == 1) return;
                    else if (tableData.ToolsType == 2)
                        Root.SoundManager.PlaySound(SoundTrack.SFX, "removeRock");
                    else if (tableData.ToolsType == 3)
                        Root.SoundManager.PlaySound(SoundTrack.SFX, "removeTree");
                    else if (tableData.ToolsType == 5)
                        Root.SoundManager.PlaySound(SoundTrack.SFX, "removeGrass");
                    break;
            }
        }
        public void SetGridView(bool view)
        {
            rect.gameObject.SetActive(view);
        }
    }
}
