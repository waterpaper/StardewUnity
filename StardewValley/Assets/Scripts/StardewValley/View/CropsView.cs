using UnityEngine;
using WATP.ECS;

namespace WATP.View
{
    public class CropsView : View<CropsEntity>
    {
        protected float multiply = 1;

        protected SpriteRenderer spriteRenderer;
        protected Animator anim;


        public CropsView(CropsEntity crops, Transform parent)
        {
            entity = crops;
            Parent = parent;

            PrefabPath = $"Address/Prefab/Crops.prefab";
        }

        protected override void OnLoad()
        {
            this.uid = entity.UID;

            Transform.position = entity.TransformComponent.position;
            spriteRenderer = Transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
            anim = Transform.GetChild(0).GetChild(1).GetComponent<Animator>();
            entity.EventComponent.onEvent += StateUpdate;

            StateUpdate("Day");

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
                case "Day":
                    var index = Root.GameDataManager.TableData.GetCropsIndex(entity.CropsDataComponent.id, entity.CropsDataComponent.day);
                    spriteRenderer.sprite = Root.GameDataManager.AtlasContainer.GetSheetSprite(Root.GameDataManager.AtlasContainer.CROPS, $"crops_{index}");
                    break;
                case "End":
                    spriteRenderer.gameObject.SetActive(false);
                    anim.gameObject.SetActive(true);
                    break;
            }
        }
    }
}
