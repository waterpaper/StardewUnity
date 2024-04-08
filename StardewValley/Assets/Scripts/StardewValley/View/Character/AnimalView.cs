using UnityEngine;
using WATP.ECS;

namespace WATP.View
{
    public class AnimalView : View<AnimalEntity>, IGridView
    {
        protected bool isHide = false;

        protected float multiply = 1;
        protected string beforeState;

        protected SpriteRenderer bodySpriteRenderer;

        protected Animator bodyAnim;
        protected SpriteRenderer rect;


        public AnimalView(AnimalEntity animal, Transform parent)
        {
            entity = animal;
            Parent = parent;

            PrefabPath = $"Address/Prefab/Character/Animal.prefab";
            entity.EventComponent.onEvent += StateAction;
        }

        protected override void OnLoad()
        {
            this.uid = entity.UID;

            Transform.position = entity.TransformComponent.position;

            bodySpriteRenderer = Transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
            bodyAnim = Transform.GetChild(0).GetChild(0).GetComponent<Animator>();
            rect = Transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>();
            SetAnimMultiply(multiply);

#if UNITY_EDITOR
            /*  var giz = trs.gameObject.AddComponent<PRSUnitGizmo>();
              giz.unit = smlUnit;*/
#endif
            var tablaData = Root.GameDataManager.TableData.GetAnimalTableData(this.entity.DataComponent.id);
            Transform.gameObject.name = tablaData.Name_En;

            bodyAnim.runtimeAnimatorController = AssetLoader.Load<RuntimeAnimatorController>($"Address/Anim/Animal/{tablaData.Name_En}Body.overrideController");
            bodyAnim.SetInteger("Direction", 2);

            rect.transform.localScale = new Vector3(tablaData.Width, tablaData.Height, 1);
            rect.gameObject.SetActive(Root.GameDataManager.Preferences.IsGrid);
        }

        protected override void OnDestroy()
        {
            entity.EventComponent.onEvent -= StateAction;
            var tablaData = Root.GameDataManager.TableData.GetAnimalTableData(this.entity.DataComponent.id);
            AssetLoader.Unload($"Address/Anim/Animal/{tablaData.Name_En}Body.controller", bodyAnim.runtimeAnimatorController);

            entity = null;

            //EventManager.Instance.SendEvent(new SoundDefaultEvent("UnitDeath"));
        }

        protected override void OnRender()
        {
            if (entity == null) return;
            if (Transform == null) return;

            Transform.position = entity.TransformComponent.position;
        }

        public override void SetMultiply(float multiply)
        {
            SetAnimMultiply(multiply);
        }

        public override void SetContinue(float multiply)
        {
            SetAnimMultiply(multiply);
        }

        public override void SetPause()
        {
            SetAnimMultiply(0);
        }

        void StateAction(string str)
        {
            if (entity == null || entity.DeleteReservation || isPrefab == false) return;

            switch (str)
            {
                case "Idle":
                    bodyAnim.SetFloat("Speed", 0);
                    break;
                case "Move":
                    bodyAnim.SetFloat("Speed", entity.MoveComponent.speed);
                    break;
                case "Direction":
                    int direction = 1;

                    if (entity.TransformComponent.rotation == Vector3.up)
                        direction = (int)EDirection.DIRECTION_UP;
                    else if (entity.TransformComponent.rotation == Vector3.down)
                        direction = (int)EDirection.DIRECTION_DOWN;
                    else if (entity.TransformComponent.rotation == Vector3.left)
                        direction = (int)EDirection.DIRECTION_LEFT;
                    else if (entity.TransformComponent.rotation == Vector3.right)
                        direction = (int)EDirection.DIRECTION_RIGHT;

                    bodyAnim.SetInteger("Direction", direction);
                    break;
                case "MoveEnd":
                    bodyAnim.SetFloat("Speed", 0);
                    break;
            }
        }

        public void SetAnimMultiply(float multiply)
        {
            this.multiply = multiply;
            if (Transform == null)
                return;

            bodyAnim.speed = multiply;
        }
        public void SetGridView(bool view)
        {
            rect.gameObject.SetActive(view);
        }
    }
}
