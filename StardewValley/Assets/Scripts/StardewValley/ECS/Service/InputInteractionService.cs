using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using WATP.Map;
using WATP.UI;

namespace WATP.ECS
{
    /// <summary>
    /// 움직임 컴포넌트
    /// </summary>
    public class InputInteractionService : IService
    {
        public List<IInteractionComponent> interactionComponents = new();
        public List<IInteractionInputComponent> interactionInputComponents = new();

        public void Add(IEntity entity)
        {
            if (entity is IInteractionComponent)
            {
                interactionComponents.Add(entity as IInteractionComponent);
            }

            if (entity is not IInteractionInputComponent) return;

            interactionInputComponents.Add(entity as IInteractionInputComponent);
        }

        public void Remove(IEntity entity)
        {
            if (entity is IInteractionComponent)
            {
                interactionComponents.Remove(entity as IInteractionComponent);
            }

            if (entity is not IInteractionInputComponent) return;

            interactionInputComponents.Remove(entity as IInteractionInputComponent);
        }

        public void Clear()
        {
            interactionInputComponents.Clear();
            interactionComponents.Clear();
        }

        public void Update(double frameTime)
        {
            if (Input.GetMouseButtonDown(1) == false) return;

            foreach (var inputComponent in interactionInputComponents)
            {
                if (inputComponent.InteractionInputComponent.isEnable == false) continue;
                Vector3 pos = Input.mousePosition;
                pos.z = Root.WorldCamera.farClipPlane;

                ITransformComponent trs = inputComponent as ITransformComponent;
                IInteractionComponent target = null;
                var targetPos = Root.WorldCamera.ScreenToWorldPoint(pos);

                for (int i = 0; i < interactionComponents.Count; i++)
                {
                    if (Vector2.Distance(targetPos, interactionComponents[i].TransformComponent.position) < 0.5f &&
                        Vector2.Distance(trs.TransformComponent.position, interactionComponents[i].TransformComponent.position) < 1.5f)
                    {
                        target = interactionComponents[i];
                        break;
                    }
                }

                if (target == null) return;

                if (target is IConversationComponent)
                    ConversationUpdate(target as IConversationComponent);

                if (target is IShopObjectComponent)
                    ShopObjectUpdate(target as IShopObjectComponent);

                if (target is ICropsDataComponent)
                    CropsUpdate(target as ICropsDataComponent);
            }
        }


        private void ConversationUpdate(IConversationComponent entity)
        {
            var selectItem = Root.State.inventory.SelectItem;
            if (selectItem == null)
            {
                OpenDialogPopup(entity.DataComponent.id, 1).Forget();
                return;
            }

            var itemTable = Root.GameDataManager.TableData.GetItemTableData(selectItem.itemId);
            if (itemTable.Type == (int)ECategory.CATEGORY_TOOL)
            {
                OpenDialogPopup(entity.DataComponent.id, 1).Forget();
            }
            else
            {
                var type = Root.State.NPCLikeAbiltiy(entity.DataComponent.id, selectItem.itemId);
                Root.State.inventory.RemoveInventory(selectItem.itemIndex);

                OpenDialogPopup(entity.DataComponent.id, type).Forget();
            }
        }

        private void ShopObjectUpdate(IShopObjectComponent entity)
        {
            OpenShopPage().Forget();
        }

        private void CropsUpdate(ICropsDataComponent entity)
        {
            var tableData = Root.GameDataManager.TableData.GetCropsTableData(entity.CropsDataComponent.id);
            if (tableData == null || entity.CropsDataComponent.day < tableData.LastDay) return;

            entity.EventComponent.onEvent?.Invoke("End");
            var delay = entity as IDelayDeleteComponent;
            delay.DelayDeleteComponent.isEnable = true;

            Root.State.inventory.AddInventory(tableData.ItemId, 1);
            Root.State.RemoveCrops(Root.SceneLoader.TileMapManager.MapName, entity.TransformComponent.position.x, entity.TransformComponent.position.y);
        }

        private async UniTaskVoid OpenDialogPopup(int id, int type)
        {
            var dialogPopup = new DialogPopup();
            dialogPopup = await Root.UIManager.Widgets.CreateAsync<DialogPopup>(dialogPopup, DialogPopup.DefaultPrefabPath);
            dialogPopup.Setting(id, type);
        }

        private async UniTaskVoid OpenShopPage()
        {
            var shoppage = new ShopPage();
            shoppage = await Root.UIManager.Widgets.CreateAsync<ShopPage>(shoppage, ShopPage.DefaultPrefabPath, null, true);
        }
    }
}