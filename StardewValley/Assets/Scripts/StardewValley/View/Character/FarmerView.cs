using UnityEngine;
using WATP.Data;
using WATP.ECS;

namespace WATP.View
{
    public class FarmerView : View<FarmerAspect>, IGridView
    {
        protected bool isHide = false;

        protected float multiply = 1;
        protected int nowState;
        protected int actionItemId;
        protected int actionType;
        protected bool actionSpriteUpdate = false;

        protected SpriteRenderer rectRenderer;
        protected SpriteRenderer bodySpriteRenderer;
        protected SpriteRenderer armSpriteRenderer;
        protected SpriteRenderer hairSpriteRenderer;
        protected SpriteRenderer shirtsSpriteRenderer;
        protected SpriteRenderer pantsSpriteRenderer;

        protected SpriteRenderer itemRenderer;
        protected SpriteRenderer toolRenderer;
        protected SpriteRenderer targetRenderer;

        protected Animator bodyAnim;
        protected Animator armAnim;
        protected Animator pantsAnim;
        protected Animator toolsAnim;

        protected AudioSource moveSource;
        protected EventActionComponent eventActionComponent;


        public FarmerView(FarmerAspect unit, EventActionComponent eventActionComponent, Transform parent)
        {
            entity = unit;
            Parent = parent;

            PrefabPath = $"Address/Prefab/Character/Farmer.prefab";
            this.eventActionComponent = eventActionComponent;
            this.uid = entity.Index;
        }

        protected override void OnLoad()
        {
            Transform.gameObject.name = "Farmer";
            Transform.position = entity.Position;

            bodySpriteRenderer = Transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
            bodyAnim = Transform.GetChild(0).GetChild(0).GetComponent<Animator>();

            armSpriteRenderer = Transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>();
            armAnim = Transform.GetChild(0).GetChild(1).GetComponent<Animator>();

            hairSpriteRenderer = Transform.GetChild(0).GetChild(2).GetComponent<SpriteRenderer>();

            shirtsSpriteRenderer = Transform.GetChild(0).GetChild(3).GetComponent<SpriteRenderer>();

            pantsSpriteRenderer = Transform.GetChild(0).GetChild(4).GetComponent<SpriteRenderer>();
            pantsAnim = Transform.GetChild(0).GetChild(4).GetComponent<Animator>();

            rectRenderer = Transform.GetChild(0).GetChild(5).GetComponent<SpriteRenderer>();

            itemRenderer = Transform.GetChild(0).GetChild(6).GetComponent<SpriteRenderer>();
            toolRenderer = Transform.GetChild(0).GetChild(7).GetComponent<SpriteRenderer>();
            toolsAnim = Transform.GetChild(0).GetChild(7).GetComponent<Animator>();

            targetRenderer = Transform.GetChild(0).GetChild(8).GetComponent<SpriteRenderer>();

            Root.State.inventory.selectIndex.onChange += OnSelectItem;

            SetPlayerCustom();
            SetAnimMultiply(multiply);
            rectRenderer.gameObject.SetActive(Root.GameDataManager.Preferences.IsGrid);
            eventActionComponent.OnEvent += StateAction;
        }

        protected override void OnDestroy()
        {
            Root.State.inventory.selectIndex.onChange -= OnSelectItem;
            eventActionComponent.OnEvent -= StateAction;
            entity = default;
            moveSource = null;
            eventActionComponent = null;
        }

        protected override void OnRender()
        {
            if (entity.Equals(default)) return;
            if (Transform == null) return;

            Transform.position = entity.Position;

            if (nowState == (int)EventKind.Action && actionSpriteUpdate == false)
            {
                if (actionType == 1 || actionType == 2 || actionType == 3)
                {
                    if(toolsAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >  0.4f)
                    {
                        var itemTable = Root.GameDataManager.TableData.GetItemTableData(actionItemId);
                        var toolData = Root.GameDataManager.TableData.GetToolTableData(actionItemId);
                        int minus = 0;
                        if ((Vector3)entity.Rotation == Vector3.up)
                            minus = 1;
                        else if ((Vector3)entity.Rotation == Vector3.down)
                            minus = 4;
                        else
                            minus = 3;
                        toolRenderer.sprite = Root.GameDataManager.AtlasContainer.GetSheetSprite(itemTable.ImagePath, $"{itemTable.ImageName}_{itemTable.Index - minus}");
                        actionSpriteUpdate = true;
                    }
                }
                else if (actionType == 4)
                {
                    if (toolsAnim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.33f)
                    {
                        var itemTable = Root.GameDataManager.TableData.GetItemTableData(actionItemId);
                        var toolData = Root.GameDataManager.TableData.GetToolTableData(actionItemId);
                        int minus = 0;
                        if ((Vector3)entity.Rotation == Vector3.up)
                            minus = 49;
                        else if ((Vector3)entity.Rotation == Vector3.down)
                            minus = 52;
                        else
                            minus = 50;
                        toolRenderer.sprite = Root.GameDataManager.AtlasContainer.GetSheetSprite(itemTable.ImagePath, $"{itemTable.ImageName}_{itemTable.Index - minus}");
                        actionSpriteUpdate = true;
                    }
                }

            }
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
            if (entity.Equals(default) || entity.DeleteReservation || isPrefab == false) return;

            switch (state)
            {
                case (int)EventKind.Idle:
                    bodyAnim.SetFloat("Speed", 0);
                    armAnim.SetFloat("Speed", 0);
                    pantsAnim.SetFloat("Speed", 0);

                    if (moveSource != null)
                    {
                        moveSource.Stop();
                        moveSource = null;
                    }
                    break;
                case (int)EventKind.Move:
                    bodyAnim.SetFloat("Speed", entity.Speed);
                    armAnim.SetFloat("Speed", entity.Speed);
                    pantsAnim.SetFloat("Speed", entity.Speed);
                    if (moveSource == null)
                        moveSource = Root.SoundManager.PlaySound(SoundTrack.SFX, "move_soil", true);
                    break;
                case (int)EventKind.Direction:
                    int direction = 1;

                    if ((Vector3)entity.Rotation == Vector3.up)
                    {
                        direction = (int)EDirection.DIRECTION_UP;
                        hairSpriteRenderer.sprite = Root.GameDataManager.AtlasContainer.GetSheetSprite(Root.GameDataManager.AtlasContainer.FARMER_HAIR_PATH, $"FarmerHairs_{Root.State.player.hairIndex.Value - 1 + Config.CUSTOM_HAIR_MAX * 3}");
                        shirtsSpriteRenderer.sprite = Root.GameDataManager.AtlasContainer.GetSheetSprite(Root.GameDataManager.AtlasContainer.FARMER_SHIRTS_PATH, $"FarmerShirts_{Root.State.player.clothsIndex.Value - 1 + Config.CUSTOM_CLOTHS_MAX * 3}");

                        toolRenderer.sortingOrder = -1;
                    }
                    else if ((Vector3)entity.Rotation == Vector3.down)
                    {
                        direction = (int)EDirection.DIRECTION_DOWN;
                        hairSpriteRenderer.sprite = Root.GameDataManager.AtlasContainer.GetSheetSprite(Root.GameDataManager.AtlasContainer.FARMER_HAIR_PATH, $"FarmerHairs_{Root.State.player.hairIndex.Value - 1}");
                        shirtsSpriteRenderer.sprite = Root.GameDataManager.AtlasContainer.GetSheetSprite(Root.GameDataManager.AtlasContainer.FARMER_SHIRTS_PATH, $"FarmerShirts_{Root.State.player.clothsIndex.Value - 1}");

                        toolRenderer.sortingOrder = 11;
                    }
                    else if ((Vector3)entity.Rotation == Vector3.left)
                    {
                        direction = (int)EDirection.DIRECTION_LEFT;
                        hairSpriteRenderer.sprite = Root.GameDataManager.AtlasContainer.GetSheetSprite(Root.GameDataManager.AtlasContainer.FARMER_HAIR_PATH, $"FarmerHairs_{Root.State.player.hairIndex.Value - 1 + Config.CUSTOM_HAIR_MAX * 2}");
                        shirtsSpriteRenderer.sprite = Root.GameDataManager.AtlasContainer.GetSheetSprite(Root.GameDataManager.AtlasContainer.FARMER_SHIRTS_PATH, $"FarmerShirts_{Root.State.player.clothsIndex.Value - 1 + Config.CUSTOM_CLOTHS_MAX * 2}");

                        toolRenderer.sortingOrder = 11;
                    }
                    else if ((Vector3)entity.Rotation == Vector3.right)
                    {
                        direction = (int)EDirection.DIRECTION_RIGHT;
                        hairSpriteRenderer.sprite = Root.GameDataManager.AtlasContainer.GetSheetSprite(Root.GameDataManager.AtlasContainer.FARMER_HAIR_PATH, $"FarmerHairs_{Root.State.player.hairIndex.Value - 1 + Config.CUSTOM_HAIR_MAX}");
                        shirtsSpriteRenderer.sprite = Root.GameDataManager.AtlasContainer.GetSheetSprite(Root.GameDataManager.AtlasContainer.FARMER_SHIRTS_PATH, $"FarmerShirts_{Root.State.player.clothsIndex.Value - 1 + Config.CUSTOM_CLOTHS_MAX}");

                        toolRenderer.sortingOrder = 11;
                    }

                    bodyAnim.SetInteger("Direction", direction);
                    armAnim.SetInteger("Direction", direction);
                    pantsAnim.SetInteger("Direction", direction);
                    toolsAnim.SetInteger("Direction", direction);
                    break;
                case (int)EventKind.MoveEnd:
                    bodyAnim.SetFloat("Speed", 0);
                    armAnim.SetFloat("Speed", 0);
                    pantsAnim.SetFloat("Speed", 0);
                    break;
                case (int)EventKind.Action:
                    toolRenderer.sprite = null;
                    var item = Root.State.inventory.SelectItem;
                    var itemTable = Root.GameDataManager.TableData.GetItemTableData(item.itemId);
                    var toolData = Root.GameDataManager.TableData.GetToolTableData(item.itemId);
                    actionType = toolData.Type;
                    actionItemId = item.itemId;

                    var cell = Root.SceneLoader.TileMapManager.GetCell((int)entity.TargetPostion.x, (int)entity.TargetPostion.y);

                    if (cell != null)
                    {
                        targetRenderer.transform.position = cell.Position;
                        targetRenderer.gameObject.SetActive(true);
                    }

                    if (actionType == 1 || actionType == 2 || actionType == 3)
                    {
                        bodyAnim.SetBool("IsSmash", true);
                        armAnim.SetBool("IsSmash", true);
                        pantsAnim.SetBool("IsSmash", true);
                        toolsAnim.SetBool("IsSmash", true);

                        int minus = 0;
                        if ((Vector3)entity.Rotation == Vector3.up)
                            minus = 2;
                        else if ((Vector3)entity.Rotation == Vector3.down)
                            minus = 5;
                        else
                            minus = 3;
                        toolRenderer.sprite = Root.GameDataManager.AtlasContainer.GetSheetSprite(itemTable.ImagePath, $"{itemTable.ImageName}_{itemTable.Index - minus}");
                    }
                    else if (actionType == 4)
                    {
                        bodyAnim.SetBool("IsWatering", true);
                        armAnim.SetBool("IsWatering", true);
                        pantsAnim.SetBool("IsWatering", true);
                        toolsAnim.SetBool("IsWatering", true);
                        int minus = 0;
                        if ((Vector3)entity.Rotation == Vector3.up)
                            minus = 49;
                        else if ((Vector3)entity.Rotation == Vector3.down)
                            minus = 53;
                        else
                            minus = 51;
                        toolRenderer.sprite = Root.GameDataManager.AtlasContainer.GetSheetSprite(itemTable.ImagePath, $"{itemTable.ImageName}_{itemTable.Index - minus}");
                    }
                    else if (actionType == 5)
                    {
                        bodyAnim.SetBool("IsAttack", true);
                        armAnim.SetBool("IsAttack", true);
                        pantsAnim.SetBool("IsAttack", true);
                        toolsAnim.SetBool("IsAttack", true);
                        toolRenderer.sprite = Root.GameDataManager.AtlasContainer.GetSheetSprite(itemTable.ImagePath, $"{itemTable.ImageName}_{itemTable.Index}");
                        actionSpriteUpdate = false;
                    }
                    break;
                case (int)EventKind.ActionEnd:
                    if (actionType == 1 || actionType == 2 || actionType == 3)
                    {
                        bodyAnim.SetBool("IsSmash", false);
                        armAnim.SetBool("IsSmash", false);
                        pantsAnim.SetBool("IsSmash", false);
                        toolsAnim.SetBool("IsSmash", false);
                    }
                    else if (actionType == 4)
                    {
                        bodyAnim.SetBool("IsWatering", false);
                        armAnim.SetBool("IsWatering", false);
                        pantsAnim.SetBool("IsWatering", false);
                        toolsAnim.SetBool("IsWatering", false);
                    }
                    else if (actionType == 5)
                    {
                        bodyAnim.SetBool("IsAttack", false);
                        armAnim.SetBool("IsAttack", false);
                        pantsAnim.SetBool("IsAttack", false);
                        toolsAnim.SetBool("IsAttack", false);
                    }
                    actionItemId = 0;
                    actionType = 0;
                    actionSpriteUpdate = false;
                    toolRenderer.sprite = null;
                    targetRenderer.gameObject.SetActive(false);
                    break;
                case (int)EventKind.Take:
                    bodyAnim.SetBool("IsTake", true);
                    armAnim.SetBool("IsTake", true);
                    pantsAnim.SetBool("IsTake", true);
                    break;
                case (int)EventKind.TakeEnd:
                    bodyAnim.SetBool("IsTake", false);
                    armAnim.SetBool("IsTake", false);
                    pantsAnim.SetBool("IsTake", false);
                    break;
            }

            nowState = state;
        }

        public void SetPlayerCustom()
        {
            hairSpriteRenderer.sprite = Root.GameDataManager.AtlasContainer.GetSheetSprite(Root.GameDataManager.AtlasContainer.FARMER_HAIR_PATH, $"FarmerHairs_{Root.State.player.hairIndex.Value - 1}");
            shirtsSpriteRenderer.sprite = Root.GameDataManager.AtlasContainer.GetSheetSprite(Root.GameDataManager.AtlasContainer.FARMER_SHIRTS_PATH, $"FarmerShirts_{Root.State.player.clothsIndex.Value - 1}");

            hairSpriteRenderer.color = new Color(Root.State.player.hairColor.Value.x / 255f, Root.State.player.hairColor.Value.y / 255f, Root.State.player.hairColor.Value.z / 255f);
            pantsSpriteRenderer.color = new Color(Root.State.player.clothsColor.Value.x / 255f, Root.State.player.clothsColor.Value.y / 255f, Root.State.player.clothsColor.Value.z / 255f);
        }

        public void SetAnimMultiply(float multiply)
        {
            this.multiply = multiply;
            if (Transform == null)
                return;

            bodyAnim.speed = multiply;
            armAnim.speed = multiply;
            pantsAnim.speed = multiply;
        }

        private void OnSelectItem(int itemIndex)
        {
            var selectItem = Root.State.inventory.SelectItem;
            if (selectItem == null)
            {
                StateAction((int)EventKind.TakeEnd);
                itemRenderer.gameObject.SetActive(false);
                itemRenderer.sprite = null;
                return;
            }

            var itemTable = Root.GameDataManager.TableData.GetItemTableData(selectItem.itemId);
            if (itemTable.Type == (int)ECategory.CATEGORY_TOOL)
            {
                StateAction((int)EventKind.TakeEnd);
                itemRenderer.gameObject.SetActive(false);
                itemRenderer.sprite = null;
                return;
            }

            StateAction((int)EventKind.Take);
            itemRenderer.gameObject.SetActive(true);
            itemRenderer.sprite = Root.GameDataManager.AtlasContainer.GetSheetSprite(itemTable.ImagePath, $"{itemTable.ImageName}_{itemTable.Index}");
        }

        public void SetGridView(bool view)
        {
            rectRenderer.gameObject.SetActive(view);
        }
    }
}
