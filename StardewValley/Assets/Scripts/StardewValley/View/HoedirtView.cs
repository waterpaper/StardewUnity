using UnityEngine;
using WATP.ECS;

namespace WATP.View
{
    public class HoedirtView : View<HoedirtEntity>
    {
        protected float multiply = 1;

        protected SpriteRenderer spriteRenderer;
        protected Animator dirtAnim;
        protected Animator wateringAnim;


        public HoedirtView(HoedirtEntity hoedirt, Transform parent)
        {
            entity = hoedirt;
            Parent = parent;

            PrefabPath = $"Address/Prefab/Hoedirt.prefab";
        }

        protected override void OnLoad()
        {
            this.uid = entity.UID;

            Transform.position = entity.TransformComponent.position;
            spriteRenderer = Transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
            dirtAnim = Transform.GetChild(0).GetChild(1).GetComponent<Animator>();
            wateringAnim = Transform.GetChild(0).GetChild(2).GetComponent<Animator>();
            entity.EventComponent.onEvent += StateUpdate;

            dirtAnim.gameObject.SetActive(entity.HoedirtDataComponent.add);

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
            int index = 0;
            switch (str)
            {
                case "Normal":
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
                    break;
                case "Watering":
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

                    wateringAnim.gameObject.SetActive(false);
                    wateringAnim.gameObject.SetActive(true);
                    break;
            }

            spriteRenderer.sprite = Root.GameDataManager.AtlasContainer.GetSheetSprite(Root.GameDataManager.AtlasContainer.HOEDIRT, $"Hoedirt_{index}");
        }
    }
}
