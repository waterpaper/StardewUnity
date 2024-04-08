using System;
using UnityEngine;
using WATP.Data;
using WATP.ECS;

namespace WATP.View
{
    public class MapObjectView : View<MapObjectEntity>, IGridView
    {
        protected float multiply = 1;

        protected SpriteRenderer spriteRenderer;
        protected Animator destroyAnim;
        protected SpriteRenderer rect;


        public MapObjectView(MapObjectEntity mapObject, Transform parent)
        {
            entity = mapObject;
            Parent = parent;

            PrefabPath = $"Address/Prefab/MapObject.prefab";
        }

        protected override void OnLoad()
        {
            this.uid = entity.UID;

            Transform.position = entity.TransformComponent.position;
            spriteRenderer = Transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
            destroyAnim = Transform.GetChild(0).GetChild(1).GetComponent<Animator>();
            rect = Transform.GetChild(0).GetChild(2).GetComponent<SpriteRenderer>();
            entity.EventComponent.onEvent += StateUpdate;

            destroyAnim.SetInteger("Id", entity.MapObjectDataComponent.id);

            var tableData = Root.GameDataManager.TableData.GetObjectTableData(entity.MapObjectDataComponent.id);
            spriteRenderer.sprite = Root.GameDataManager.AtlasContainer.GetSheetSprite(tableData.ImagePath, $"{tableData.ImageName}_{tableData.Index}");

            spriteRenderer.transform.localPosition = new Vector3(tableData.ViewX, tableData.ViewY, 0);

            rect.transform.localScale = new Vector3(tableData.Width, tableData.Height, 1);
            rect.gameObject.SetActive(Root.GameDataManager.Preferences.IsGrid);

#if UNITY_EDITOR
            /*  var giz = trs.gameObject.AddComponent<PRSUnitGizmo>();
              giz.unit = smlUnit;*/
#endif
        }

        protected override void OnDestroy()
        {
            entity.EventComponent.onEvent -= StateUpdate;
            entity = null;
            //EventManager.Instance.SendEvent(new SoundDefaultEvent("UnitDeath"));
        }

        protected override void OnRender()
        {
        }

        void StateUpdate(string str)
        {
            if (entity == null || entity.DeleteReservation || isPrefab == false) return;
            switch (str)
            {
                case "Hit":
                    break;
                case "Destroy":
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
