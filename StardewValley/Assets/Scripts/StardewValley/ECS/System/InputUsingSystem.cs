using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace WATP.ECS
{
    /// <summary>
    /// input 이벤트중 아이템 사용에 대한 이벤트를 처리한다.
    /// </summary>
    [RequireMatchingQueriesForUpdate]
    [UpdateBefore(typeof(PhysicsCollisionSystem))]
    [UpdateAfter(typeof(InputInteractionSystem))]
    public partial class InputUsingSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<NormalSystemsCompTag>();
        }
        protected override void OnUpdate()
        {
            foreach (var tag in SystemAPI.Query<RefRO<PauseSystemsCompTag>>())
                return;
            float frameTime = SystemAPI.Time.DeltaTime; 

            Entities
                .WithAll<TransformComponent, UsingInputComponent, EventComponent>()
                .ForEach(
                    (ref TransformComponent transform, ref UsingInputComponent usingInput, ref EventComponent eventComponent) =>
                    {
                        if (usingInput.isAction)
                        {
                            ActionUpdate(ref transform, ref usingInput, frameTime);
                            return;
                        }

                        if (!usingInput.isEnable) return;
                        if (Input.GetMouseButtonDown(0) == false) return;

                        var item = Root.State.inventory.SelectItem;
                        if (item == null) return;

                        var itemData = Root.GameDataManager.TableData.GetItemTableData(item.itemId);
                        if (itemData.Type == (int)ECategory.CATEGORY_TOOL)
                            ToolAction(item.itemId, ref transform, ref usingInput, ref eventComponent);
                        else if (itemData.Type == (int)ECategory.CATEGORY_SEED)
                            SeedAction(item.itemId, transform);
                    }
                )
                .WithoutBurst().Run();
        }

        private void ToolAction(int itemId, ref TransformComponent transform, ref UsingInputComponent inputComponent, ref EventComponent eventComponent)
        {
            if (Root.State.player.actingPower.Value < 2) return;
            var toolData = Root.GameDataManager.TableData.GetToolTableData(itemId);

            inputComponent.isAction = true;
            inputComponent.actionTimer = 0;
            inputComponent.actionType = toolData.Type;
            inputComponent.targetPos = TargetPos(transform);
            Root.State.player.actingPower.Value -= 2;

            var angle = 360 - Quaternion.FromToRotation(Vector3.up, inputComponent.targetPos - transform.position).eulerAngles.z;

            if (angle > 45 && angle <= 135)
                transform.rotation = new float3(1, 0, 0);
            else if (angle > 135 && angle <= 225)
                transform.rotation = new float3(0, -1, 0);
            else if (angle > 225 && angle <= 315)
                transform.rotation = new float3(-1, 0, 0);
            else
                transform.rotation = new float3(0, 1, 0);

            eventComponent.events.Add(new EventBuffer() { value = (int)EventKind.Direction });
        }

        private float3 TargetPos(TransformComponent transform)
        {
            float3 target = float3.zero;
            float3 mousePos = Input.mousePosition;
            mousePos.z = Root.WorldCamera.farClipPlane;

            float3 targetPos = Root.WorldCamera.ScreenToWorldPoint(mousePos);
            targetPos.z = 0;

            if (math.distance(targetPos, transform.position) <= 1)
                target = targetPos;
            else
            {
                float3 norVec = math.normalize(targetPos - transform.position);
                target = transform.position + norVec;
            }

            return target;
        }

        private void ActionUpdate(ref TransformComponent transform, ref UsingInputComponent inputComponent, float frameTime)
        {
            inputComponent.actionTimer += frameTime;
            if (inputComponent.actionTimer > 1f)
            {
                inputComponent.actionTimer = 0;
                inputComponent.isAction = false;
                return;
            }

            if (inputComponent.actionType == 0) return;
            if (inputComponent.actionTimer <= 0.5f) return;

            if(inputComponent.actionType == 1)
            {
                var cell = Root.SceneLoader.TileMapManager.GetCell((int)inputComponent.targetPos.x, (int)inputComponent.targetPos.y);

                if (cell != null)
                {
                    if (Root.State.GetObject(Root.SceneLoader.TileMapManager.MapName, cell.Position.x, cell.Position.y) != null) return;

                    if (Root.State.IsHoedirt(Root.SceneLoader.TileMapManager.MapName, cell.Position.x, cell.Position.y))
                    {
                        if (Root.State.IsCrops(Root.SceneLoader.TileMapManager.MapName, cell.Position.x, cell.Position.y))
                        {
                            foreach (var (croptransform, cropsdata, delete) in SystemAPI.Query<RefRO<TransformComponent>, RefRO<CropsDataComponent>, RefRW<DeleteComponent>>())
                            {
                                if (croptransform.ValueRO.position.x == cell.Position.x && croptransform.ValueRO.position.y == cell.Position.y)
                                {
                                    delete.ValueRW.isEnable = true;
                                    delete.ValueRW.isDelate = true;
                                    break;
                                }
                            }

                            Root.State.RemoveCrops(Root.SceneLoader.TileMapManager.MapName, cell.Position.x, cell.Position.y);
                        }
                    }
                    else
                    {
                        if (cell.GetCellType() != Map.CellKind.FarmLand) return;
                        var hoedirt = new WATP.ECS.HoedirtAspectBuilder()
                            .SetPos(new float3(cell.Position.x, cell.Position.y, 0))
                            .SetisAdd(true);

                        WATP.ECS.ECSController.ServiceEvents.Emit(new WATP.ECS.EventAddEntity(hoedirt));
                        Root.State.AddHoedirt(Root.SceneLoader.TileMapManager.MapName, cell.Position.x, cell.Position.y);
                    }
                }

                Root.SoundManager.PlaySound(SoundTrack.SFX, "hoe");
            }
            else if (inputComponent.actionType == 2 || inputComponent.actionType == 3 || inputComponent.actionType == 5)
            {
                var cell = Root.SceneLoader.TileMapManager.GetCell((int)inputComponent.targetPos.x, (int)inputComponent.targetPos.y);

                if (cell != null)
                {
                    foreach (var (mapobjecttransform, mapobjectdata, collider, delete, eventData) in SystemAPI.Query<RefRO<TransformComponent>, RefRW<MapObjectDataComponent>, RefRO<ColliderComponent>, RefRW<DeleteComponent>, RefRW<EventComponent>>())
                    {
                        var tableData = Root.GameDataManager.TableData.GetObjectTableData(mapobjectdata.ValueRO.id);
                        if (tableData.ToolsType != inputComponent.actionType)
                            continue;

                        float targetHelfWidth = collider.ValueRO.areaWidth / 2;
                        float targetHelfHeight = collider.ValueRO.areaHeight / 2;

                        if (cell.Position.x < mapobjecttransform.ValueRO.position.x + targetHelfWidth
                            && cell.Position.x > mapobjecttransform.ValueRO.position.x - targetHelfWidth
                            && cell.Position.y > mapobjecttransform.ValueRO.position.y - targetHelfHeight
                            && cell.Position.y < mapobjecttransform.ValueRO.position.y + targetHelfHeight)
                        {
                            mapobjectdata.ValueRW.hp -= 1;
                            eventData.ValueRW.events.Add(new EventBuffer() { value = (int)EventKind.Hit });
                            if (mapobjectdata.ValueRO.hp <= 0)
                            {
                                delete.ValueRW.isEnable = true;
                                delete.ValueRW.isTimer = true;
                                eventData.ValueRW.events.Add(new EventBuffer() { value = (int)EventKind.Destroy });
                                Root.State.RemoveObject(Root.SceneLoader.TileMapManager.MapName, mapobjecttransform.ValueRO.position.x, mapobjecttransform.ValueRO.position.y);

                                var objectTableData = Root.GameDataManager.TableData.GetObjectTableData(mapobjectdata.ValueRO.id);

                                for (int i = 0; i < objectTableData.ItemIds.Length; i++)
                                    Root.State.inventory.AddInventory(objectTableData.ItemIds[i], objectTableData.ItemCounts[i]);
                            }

                            break;
                        }
                    }
                }

                Root.SoundManager.PlaySound(SoundTrack.SFX, "smash");
            }
            else if (inputComponent.actionType == 4)
            {
                var cell = Root.SceneLoader.TileMapManager.GetCell((int)inputComponent.targetPos.x, (int)inputComponent.targetPos.y);

                if (cell != null)
                {
                    if (Root.State.IsHoedirt(Root.SceneLoader.TileMapManager.MapName, cell.Position.x, cell.Position.y))
                    {
                        foreach (var (hoedirttransform, hoedirtdata, eventData) in SystemAPI.Query<RefRO<TransformComponent>, RefRW<HoedirtDataComponent>, RefRW<EventComponent>>())
                        {
                            if (hoedirttransform.ValueRO.position.x == cell.Position.x && hoedirttransform.ValueRO.position.y == cell.Position.y)
                            {
                                hoedirtdata.ValueRW.watering = true;
                                eventData.ValueRW.events.Add(new EventBuffer() { value = (int)EventKind.Watering });
                                Root.State.GetHoedirt(Root.SceneLoader.TileMapManager.MapName, cell.Position.x, cell.Position.y).watering = true;
                            }
                        }
                    }
                }

                Root.SoundManager.PlaySound(SoundTrack.SFX, "watering");
            }

            inputComponent.actionType = 0;
            inputComponent.targetPos = float3.zero;
        }

        private void SeedAction(int itemId, TransformComponent transform)
        {
            var cropsData = Root.GameDataManager.TableData.GetCropsTableData(itemId);

            if (cropsData.Month != Root.State.month.Value)
                return;

            var pos = TargetPos(transform);
            var cell = Root.SceneLoader.TileMapManager.GetCell((int)pos.x, (int)pos.y);

            if (cell == null) return;

            if (Root.State.IsHoedirt(Root.SceneLoader.TileMapManager.MapName, cell.Position.x, cell.Position.y)
                && !Root.State.IsCrops(Root.SceneLoader.TileMapManager.MapName, cell.Position.x, cell.Position.y))
            {

                  var crops = new WATP.ECS.CropsAspectBuilder()
                 .SetPos(new float3(cell.Position.x, cell.Position.y, 0))
                 .SetId(itemId);

                 WATP.ECS.ECSController.ServiceEvents.Emit(new WATP.ECS.EventAddEntity(crops));

                Root.State.inventory.RemoveInventory(itemId, 1);
                Root.State.AddCrops(Root.SceneLoader.TileMapManager.MapName, itemId, cell.Position.x, cell.Position.y);
            }
        }

        private static float3 ToEulerAngles(quaternion q)
        {
            float3 angles;

            // roll (x-axis rotation)
            double sinr_cosp = 2 * (q.value.w * q.value.x + q.value.y * q.value.z);
            double cosr_cosp = 1 - 2 * (q.value.x * q.value.x + q.value.y * q.value.y);
            angles.x = (float)math.atan2(sinr_cosp, cosr_cosp);

            // pitch (y-axis rotation)
            double sinp = 2 * (q.value.w * q.value.y - q.value.z * q.value.x);
            if (math.abs(sinp) >= 1)
                angles.y = (float)(math.abs(math.PI / 2) * math.sign(sinp)); // use 90 degrees if out of range
            else
                angles.y = (float)math.asin(sinp);

            // yaw (z-axis rotation)
            double siny_cosp = 2 * (q.value.w * q.value.z + q.value.x * q.value.y);
            double cosy_cosp = 1 - 2 * (q.value.y * q.value.y + q.value.z * q.value.z);
            angles.z = (float)math.atan2(siny_cosp, cosy_cosp);

            return angles;
        }
    }
}