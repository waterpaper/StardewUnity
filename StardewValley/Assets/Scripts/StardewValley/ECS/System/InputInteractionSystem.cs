using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace WATP.ECS
{
    /// <summary>
    /// input 이벤트중 상호작용에 대한 이벤트를 처리한다.
    /// </summary>
    [RequireMatchingQueriesForUpdate]
    [UpdateBefore(typeof(InputUsingSystem))]
    [UpdateAfter(typeof(InputMoveSystem))]
    public partial class InputInteractionSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<NormalSystemsCompTag>();
        }
        protected override void OnUpdate()
        {
            if (Input.GetMouseButtonDown(1) == false) return;

            Entities
               .WithAll<TransformComponent, SleepComponent>()
               .ForEach(
                   (ref TransformComponent transform, ref InteractionInputComponent interactionInput) =>
                   {
                       if (interactionInput.isEnable == false) return;
                       float3 pos = Input.mousePosition;
                       pos.z = Root.WorldCamera.farClipPlane;

                       float3 targetPos = Root.WorldCamera.ScreenToWorldPoint(pos);
                       targetPos.z = 0;

                       ConversationUpdate(transform, targetPos);
                       ShopObjectUpdate(transform, targetPos);
                       CropsUpdate(transform, targetPos);

                   }
               )
               .WithoutBurst().Run();
        }

        private void ConversationUpdate(TransformComponent transform, float3 targetPos)
        {
            foreach (var (otherTransform, dataComponent) in SystemAPI.Query<RefRO<TransformComponent>, RefRO<DataComponent>>().WithAll<InteractionComponent, ConversationComponent>())
            {
                if (math.distance(targetPos, otherTransform.ValueRO.position) < 0.5f &&
                    math.distance(transform.position, otherTransform.ValueRO.position) < 1.5f)
                {
                    var selectItem = Root.State.inventory.SelectItem;
                    if (selectItem == null)
                    {
                        foreach (var (uiDialog, entity) in SystemAPI.Query<RefRW<UIDialogComponent>>().WithEntityAccess().WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
                        {
                            uiDialog.ValueRW.dataId = dataComponent.ValueRO.id;
                            uiDialog.ValueRW.dialogType = 1;
                            World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentEnabled<UIDialogComponent>(entity, true);
                        }
                        return;
                    }

                    var itemTable = Root.GameDataManager.TableData.GetItemTableData(selectItem.itemId);
                    if (itemTable.Type == (int)ECategory.CATEGORY_TOOL)
                    {
                        foreach (var (uiDialog, entity) in SystemAPI.Query<RefRW<UIDialogComponent>>().WithEntityAccess().WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
                        {
                            uiDialog.ValueRW.dataId = dataComponent.ValueRO.id;
                            uiDialog.ValueRW.dialogType = 1;
                            World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentEnabled<UIDialogComponent>(entity, true);
                        }
                    }
                    else
                    {
                        var type = Root.State.NPCLikeAbiltiy(dataComponent.ValueRO.id, selectItem.itemId);
                        Root.State.inventory.RemoveInventory(selectItem.itemId, 1);

                        foreach (var (uiDialog, entity) in SystemAPI.Query<RefRW<UIDialogComponent>>().WithEntityAccess().WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
                        {
                            uiDialog.ValueRW.dataId = dataComponent.ValueRO.id;
                            uiDialog.ValueRW.dialogType = type;
                            World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentEnabled<UIDialogComponent>(entity, true);
                        }
                    }


                    break;
                }
            }
        }

        private void ShopObjectUpdate(TransformComponent transform, float3 targetPos)
        {
            foreach (var (otherTransform, entity) in SystemAPI.Query<RefRW<TransformComponent>>().WithAll<InteractionComponent, ShopObjectComponent>().WithEntityAccess())
            {
                if (math.distance(targetPos, otherTransform.ValueRO.position) < 0.5f &&
                    math.distance(transform.position, otherTransform.ValueRO.position) < 1.5f)
                {
                    foreach (var (uiShop, uientity) in SystemAPI.Query<RefRO<UIShopComponent>>().WithEntityAccess().WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
                        World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentEnabled<UIShopComponent>(uientity, true);
                }
            }
        }

        private void CropsUpdate(TransformComponent transform, float3 targetPos)
        {
            foreach (var (delete, eventComponent, otherTransform, otherInteraction, cropsData) in SystemAPI.Query<RefRW<DeleteComponent>, RefRW<EventComponent>, RefRO<TransformComponent>, RefRO<InteractionComponent>, RefRO<CropsDataComponent>>())
            {
                if (math.distance(targetPos, otherTransform.ValueRO.position) < 0.5f &&
                    math.distance(transform.position, otherTransform.ValueRO.position) < 1.5f)
                {
                    var tableData = Root.GameDataManager.TableData.GetCropsTableData(cropsData.ValueRO.id);
                    if (tableData == null || cropsData.ValueRO.day < tableData.LastDay) return;

                    eventComponent.ValueRW.events.Add(new EventBuffer() { value = (int)EventKind.End });
                    delete.ValueRW.isTimer = true;

                    Root.State.inventory.AddInventory(tableData.ItemId, 1);
                    Root.State.RemoveCrops(Root.SceneLoader.TileMapManager.MapName, otherTransform.ValueRO.position.x, otherTransform.ValueRO.position.y);
                    break;
                }
            }
        }
    }
}

