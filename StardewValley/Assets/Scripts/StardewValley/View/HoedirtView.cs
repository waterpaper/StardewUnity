using UnityEngine;
using WATP.ECS;

namespace WATP.View
{
    public class HoedirtView : View<HoedirtAspect>
    {
        private bool isWatering =false;
        protected float multiply = 1;

        protected SpriteRenderer spriteRenderer;
        protected Animator dirtAnim;
        protected Animator wateringAnim;
        protected EventActionComponent eventActionComponent;


        public HoedirtView(HoedirtAspect hoedirt, EventActionComponent eventActionComponent, Transform parent)
        {
            entity = hoedirt;
            Parent = parent;

            PrefabPath = $"Address/Prefab/Hoedirt.prefab";
            this.eventActionComponent = eventActionComponent;
            this.uid = entity.Index;
        }

        protected override void OnLoad()
        {
            this.uid = entity.Index;

            Transform.position = entity.Position;
            spriteRenderer = Transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
            dirtAnim = Transform.GetChild(0).GetChild(1).GetComponent<Animator>();
            wateringAnim = Transform.GetChild(0).GetChild(2).GetComponent<Animator>();

            dirtAnim.gameObject.SetActive(entity.HoedirtDataComponent.add);
            eventActionComponent.OnEvent += StateUpdate;
            StateUpdate(entity.HoedirtDataComponent.watering ? (int)EventKind.Watering : (int)EventKind.Normal);
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
            int index = 0;
            switch (state)
            {
                case (int)EventKind.Normal:
                    if (entity.HoedirtDataComponent.up)
                    {
                        if (entity.HoedirtDataComponent.down)
                        {
                            if (entity.HoedirtDataComponent.left)
                            {
                                if (entity.HoedirtDataComponent.right)
                                    index = 10;
                                else
                                    index = 11;
                            }
                            else if (entity.HoedirtDataComponent.right)
                                index = 9;
                            else
                                index = 16;
                        }
                        else if (entity.HoedirtDataComponent.left)
                        {
                            if (entity.HoedirtDataComponent.right)
                                index = 18;
                            else
                                index = 19;
                        }
                        else if (entity.HoedirtDataComponent.right)
                        {
                            index = 17;
                        }
                        else
                        {
                            index = 24;
                        }
                    }
                    else if (entity.HoedirtDataComponent.down)
                    {
                        if (entity.HoedirtDataComponent.left)
                        {
                            if (entity.HoedirtDataComponent.right)
                                index = 2;
                            else
                                index = 3;
                        }
                        else if (entity.HoedirtDataComponent.right)
                            index = 1;
                        else
                            index = 8;

                    }
                    else if (entity.HoedirtDataComponent.left)
                    {
                        if (entity.HoedirtDataComponent.right)
                            index = 26;
                        else
                            index = 27;

                    }
                    else if (entity.HoedirtDataComponent.right)
                    {
                        index = 25;
                    }
                    else
                    {
                        index = 0;
                    }

                    isWatering = false;
                    break;
                case (int)EventKind.Watering:
                    if (entity.HoedirtDataComponent.up)
                    {
                        if (entity.HoedirtDataComponent.down)
                        {
                            if (entity.HoedirtDataComponent.left)
                            {
                                if (entity.HoedirtDataComponent.right)
                                    index = 14;
                                else
                                    index = 15;
                            }
                            else if (entity.HoedirtDataComponent.right)
                                index = 13;
                            else
                                index = 20;
                        }
                        else if (entity.HoedirtDataComponent.left)
                        {
                            if (entity.HoedirtDataComponent.right)
                                index = 22;
                            else
                                index = 23;
                        }
                        else if (entity.HoedirtDataComponent.right)
                        {
                            index = 13;
                        }
                        else
                        {
                            index = 28;
                        }
                    }
                    else if (entity.HoedirtDataComponent.down)
                    {
                        if (entity.HoedirtDataComponent.left)
                        {
                            if (entity.HoedirtDataComponent.right)
                                index = 6;
                            else
                                index = 7;
                        }
                        else if (entity.HoedirtDataComponent.right)
                                index = 5;
                        else
                            index = 12;

                    }
                    else if (entity.HoedirtDataComponent.left)
                    {
                        if (entity.HoedirtDataComponent.right)
                            index = 30;
                        else
                            index = 31;

                    }
                    else if (entity.HoedirtDataComponent.right)
                    {
                        index = 29;
                    }
                    else
                    {
                        index = 4;
                    }

                    if (isWatering == false)
                    {
                        wateringAnim.gameObject.SetActive(false);
                        wateringAnim.gameObject.SetActive(true);
                        isWatering = true;
                    }
                    break;
            }

            spriteRenderer.sprite = Root.GameDataManager.AtlasContainer.GetSheetSprite(Root.GameDataManager.AtlasContainer.HOEDIRT, $"Hoedirt_{index}");
        }
    }
}
