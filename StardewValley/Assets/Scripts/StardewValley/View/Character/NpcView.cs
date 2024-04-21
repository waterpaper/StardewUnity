using UnityEngine;
using WATP.Data;
using WATP.ECS;

namespace WATP.View
{
    public class NpcView : View<NpcAspect>, IGridView
    {
        protected bool isHide = false;

        protected float multiply = 1;
        protected string beforeState;

        protected SpriteRenderer bodySpriteRenderer;

        protected Animator bodyAnim;
        protected SpriteRenderer rect;
        protected EventActionComponent eventActionComponent;


        public NpcView(NpcAspect npc, EventActionComponent eventActionComponent, Transform parent)
        {
            entity = npc;
            Parent = parent;

            PrefabPath = $"Address/Prefab/Character/NPC.prefab";
            this.eventActionComponent = eventActionComponent;
            this.uid = entity.Index;
        }

        protected override void OnLoad()
        {
            Transform.position = entity.Position;

            bodySpriteRenderer = Transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
            bodyAnim = Transform.GetChild(0).GetChild(0).GetComponent<Animator>();

            rect = Transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>();

            var tablaData = Root.GameDataManager.TableData.GetNPCTableData(this.entity.DataComponent.id);
            Transform.gameObject.name = tablaData.Name_En;

            bodyAnim.runtimeAnimatorController = AssetLoader.Load<RuntimeAnimatorController>($"Address/Anim/NPC/{tablaData.Name_En}Body.overrideController");
            bodyAnim.SetInteger("Direction", 2);
            SetAnimMultiply(multiply);
            rect.gameObject.SetActive(Root.GameDataManager.Preferences.IsGrid);
            eventActionComponent.OnEvent += StateAction;
        }

        protected override void OnDestroy()
        {
            eventActionComponent.OnEvent -= StateAction;
            eventActionComponent = null;
            var tablaData = Root.GameDataManager.TableData.GetNPCTableData(this.entity.DataComponent.id);
            AssetLoader.Unload($"Address/Anim/NPC/{tablaData.Name_En}Body.controller", bodyAnim.runtimeAnimatorController);

            entity = default;
        }

        protected override void OnRender()
        {
            if (entity.Equals(default)) return;
            if (Transform == null) return;

            Transform.position = entity.Position;
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

        void StateAction(int state)
        {
            if (entity.Equals(default) || entity.DeleteComponent.isDelate || isPrefab == false) return;

            switch (state)
            {
                case (int)EventKind.Idle:
                    bodyAnim.SetFloat("Speed", 0);
                    break;
                case (int)EventKind.Move:
                    bodyAnim.SetFloat("Speed", entity.Speed);
                    break;
                case (int)EventKind.Direction:
                    int direction = 1;

                    if ((Vector3)entity.Rotation == Vector3.up)
                        direction = (int)EDirection.DIRECTION_UP;
                    else if ((Vector3)entity.Rotation == Vector3.down)
                        direction = (int)EDirection.DIRECTION_DOWN;
                    else if ((Vector3)entity.Rotation == Vector3.left)
                        direction = (int)EDirection.DIRECTION_LEFT;
                    else if ((Vector3)entity.Rotation == Vector3.right)
                        direction = (int)EDirection.DIRECTION_RIGHT;

                    bodyAnim.SetInteger("Direction", direction);
                    break;
                case (int)EventKind.MoveEnd:
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
