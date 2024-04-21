using UnityEngine;
using WATP.ECS;

namespace WATP.View
{
    public class CropsView : View<CropsAspect>
    {
        protected float multiply = 1;

        protected SpriteRenderer spriteRenderer;
        protected Animator anim;
        protected EventActionComponent eventActionComponent;


        public CropsView(CropsAspect crops, EventActionComponent eventActionComponent, Transform parent)
        {
            entity = crops;
            Parent = parent;

            PrefabPath = $"Address/Prefab/Crops.prefab";
            this.eventActionComponent = eventActionComponent;
            this.uid = entity.Index;
        }

        protected override void OnLoad()
        {
            Transform.position = entity.Position;
            spriteRenderer = Transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
            anim = Transform.GetChild(0).GetChild(1).GetComponent<Animator>();

            eventActionComponent.OnEvent += StateUpdate;
            StateUpdate((int)EventKind.Day);
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
                case (int)EventKind.Day:
                    var index = Root.GameDataManager.TableData.GetCropsIndex(entity.CropsDataComponent.id, entity.CropsDataComponent.day);
                    spriteRenderer.sprite = Root.GameDataManager.AtlasContainer.GetSheetSprite(Root.GameDataManager.AtlasContainer.CROPS, $"crops_{index}");
                    break;
                case (int)EventKind.End:
                    spriteRenderer.gameObject.SetActive(false);
                    anim.gameObject.SetActive(true);
                    break;
            }
        }
    }
}
