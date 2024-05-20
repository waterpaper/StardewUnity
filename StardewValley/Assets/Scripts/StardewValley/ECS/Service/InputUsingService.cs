using System.Collections.Generic;
using UnityEngine;

namespace WATP.ECS
{
    /// <summary>
    /// 도구 사용 서비스 로직
    /// 해당하는 도구에 맞는 로직을 처리한다.
    /// </summary>
    public class InputUsingService : IService
    {
        public List<ITransformComponent> transformComponents = new();
        public List<IUsingInputComponent> usingInputComponents = new();

        public void Add(IEntity entity)
        {
            if (entity is ITransformComponent)
            {
                transformComponents.Add(entity as ITransformComponent);
            }
            if (entity is not IUsingInputComponent) return;

            usingInputComponents.Add(entity as IUsingInputComponent);
        }

        public void Remove(IEntity entity)
        {
            if (entity is ITransformComponent)
            {
                transformComponents.Remove(entity as ITransformComponent);
            }
            if (entity is not IUsingInputComponent) return;

            usingInputComponents.Remove(entity as IUsingInputComponent);
        }

        public void Clear()
        {
            usingInputComponents.Clear();
            transformComponents.Clear();
        }

        public void Update(double frameTime)
        {
            foreach (var inputComponent in usingInputComponents)
            {
                if (inputComponent.UsingInputComponent.isAction)
                {
                    ActionUpdate(inputComponent, (float)frameTime);
                    return;
                }

                if (!inputComponent.UsingInputComponent.isEnable) continue;
                if (Input.GetMouseButtonDown(0) == false) continue;

                var item = Root.State.inventory.SelectItem;
                if (item == null) continue;

                var itemData = Root.GameDataManager.TableData.GetItemTableData(item.itemId);
                if (itemData.Type == (int)ECategory.CATEGORY_TOOL)
                    ToolAction(item.itemId, inputComponent);
                else if (itemData.Type == (int)ECategory.CATEGORY_SEED)
                    SeedAction(item.itemId, inputComponent);
            }
        }

        private void ToolAction(int itemId, IUsingInputComponent inputComponent)
        {
            if (Root.State.player.actingPower.Value < 2) return;
            var toolData = Root.GameDataManager.TableData.GetToolTableData(itemId);

            inputComponent.UsingInputComponent.isAction = true;
            inputComponent.UsingInputComponent.actionType = toolData.Type;
            inputComponent.UsingInputComponent.targetPos = TargetPos(inputComponent);
            Root.State.player.actingPower.Value -= 2;

            var angle = 360 - Quaternion.FromToRotation(Vector3.up, inputComponent.UsingInputComponent.targetPos - inputComponent.TransformComponent.position).eulerAngles.z;

            if (angle > 45 && angle <= 135)
                inputComponent.TransformComponent.rotation = Vector3.right;
            else if (angle > 135 && angle <= 225)
                inputComponent.TransformComponent.rotation = Vector3.down;
            else if (angle > 225 && angle <= 315)
                inputComponent.TransformComponent.rotation = Vector3.left;
            else
                inputComponent.TransformComponent.rotation = Vector3.up;

            inputComponent.EventComponent?.onEvent("Direction");
        }

        private Vector3 TargetPos(IUsingInputComponent inputComponent)
        {
            Vector3 target = Vector3.zero;
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Root.WorldCamera.farClipPlane;

            var targetPos = Root.WorldCamera.ScreenToWorldPoint(mousePos);
            targetPos.z = 0;

            if (Vector3.Distance(targetPos, inputComponent.TransformComponent.position) <= 1)
                target = targetPos;
            else
            {
                Vector3 norVec = (targetPos - inputComponent.TransformComponent.position).normalized;
                target = inputComponent.TransformComponent.position + norVec;
            }

            return target;
        }

        private void ActionUpdate(IUsingInputComponent inputComponent, float frameTime)
        {
            inputComponent.UsingInputComponent.actionTimer += frameTime;
            if (inputComponent.UsingInputComponent.actionTimer > 1f)
            {
                inputComponent.UsingInputComponent.actionTimer = 0;
                inputComponent.UsingInputComponent.isAction = false;
                return;
            }

            if (inputComponent.UsingInputComponent.actionType == 0) return;
            if (inputComponent.UsingInputComponent.actionTimer <= 0.5f) return;

            if (inputComponent.UsingInputComponent.actionType == 1)
            {
                var cell = Root.SceneLoader.TileMapManager.GetCell((int)inputComponent.UsingInputComponent.targetPos.x, (int)inputComponent.UsingInputComponent.targetPos.y);

                if (cell != null)
                {
                    if (Root.State.GetObject(Root.SceneLoader.TileMapManager.MapName, cell.Position.x, cell.Position.y) != null) return;

                    if (Root.State.IsHoedirt(Root.SceneLoader.TileMapManager.MapName, cell.Position.x, cell.Position.y))
                    {
                        if (Root.State.IsCrops(Root.SceneLoader.TileMapManager.MapName, cell.Position.x, cell.Position.y))
                        {

                            for (int i = 0; i < transformComponents.Count; i++)
                            {
                                if (transformComponents[i] is ICropsDataComponent
                                    && transformComponents[i].TransformComponent.position.x == cell.Position.x && transformComponents[i].TransformComponent.position.y == cell.Position.y)
                                {
                                    (transformComponents[i] as IEntity).DeleteReservation = true;
                                    break;
                                }
                            }

                            Root.State.RemoveCrops(Root.SceneLoader.TileMapManager.MapName, cell.Position.x, cell.Position.y);
                        }
                    }
                    else
                    {
                        var hoedirt = new WATP.ECS.HoedirtEntity.HoedirtEntityBuilder()
                            .SetPos(cell.Position)
                            .SetisAdd(true);

                        WATP.ECS.ECSController.ServiceEvents.Emit(new WATP.ECS.EventAddEntity(hoedirt));
                        Root.State.AddHoedirt(Root.SceneLoader.TileMapManager.MapName, cell.Position.x, cell.Position.y);
                    }
                }

                Root.SoundManager.PlaySound(SoundTrack.SFX, "hoe");
            }
            else if (inputComponent.UsingInputComponent.actionType == 2 || inputComponent.UsingInputComponent.actionType == 3 || inputComponent.UsingInputComponent.actionType == 5)
            {
                var cell = Root.SceneLoader.TileMapManager.GetCell((int)inputComponent.UsingInputComponent.targetPos.x, (int)inputComponent.UsingInputComponent.targetPos.y);

                if (cell != null)
                {
                    IMapObjectDataComponent mapComponent = null;

                    foreach (var trsComponent in transformComponents)
                    {
                        if (trsComponent is not IMapObjectDataComponent) continue;
                        mapComponent = trsComponent as IMapObjectDataComponent;

                        var tableData = Root.GameDataManager.TableData.GetObjectTableData(mapComponent.MapObjectDataComponent.id);
                        if (tableData.ToolsType != inputComponent.UsingInputComponent.actionType)
                        {
                            mapComponent = null;
                            continue;
                        }

                        float targetHelfWidth = mapComponent.ColliderComponent.areaWidth / 2;
                        float targetHelfHeight = mapComponent.ColliderComponent.areaHeight / 2;

                        if (cell.Position.x < mapComponent.TransformComponent.position.x + targetHelfWidth
                            && cell.Position.x > mapComponent.TransformComponent.position.x - targetHelfWidth
                            && cell.Position.y > mapComponent.TransformComponent.position.y - targetHelfHeight
                            && cell.Position.y < mapComponent.TransformComponent.position.y + targetHelfHeight)
                        {
                            break;
                        }

                        mapComponent = null;
                    }

                    if (mapComponent != null)
                    {
                        mapComponent.MapObjectDataComponent.hp -= 1;
                        mapComponent.EventComponent.onEvent?.Invoke("Hit");
                        if (mapComponent.MapObjectDataComponent.hp <= 0)
                        {
                            mapComponent.DelayDeleteComponent.isEnable = true;
                            mapComponent.EventComponent.onEvent?.Invoke("Destroy");
                            Root.State.RemoveObject(Root.SceneLoader.TileMapManager.MapName, mapComponent.TransformComponent.position.x, mapComponent.TransformComponent.position.y);

                            var tableData = Root.GameDataManager.TableData.GetObjectTableData(mapComponent.MapObjectDataComponent.id);

                            for (int i = 0; i < tableData.ItemIds.Length; i++)
                                Root.State.inventory.AddInventory(tableData.ItemIds[i], tableData.ItemCounts[i]);
                        }
                    }
                }

                Root.SoundManager.PlaySound(SoundTrack.SFX, "smash");
            }
            else if (inputComponent.UsingInputComponent.actionType == 4)
            {
                var cell = Root.SceneLoader.TileMapManager.GetCell((int)inputComponent.UsingInputComponent.targetPos.x, (int)inputComponent.UsingInputComponent.targetPos.y);

                if (cell != null)
                {
                    if (Root.State.IsHoedirt(Root.SceneLoader.TileMapManager.MapName, cell.Position.x, cell.Position.y))
                    {
                        foreach (var trs in transformComponents)
                        {
                            if (trs is not IHoedirtDataComponent) continue;

                            var hoedirtDataComponent = trs as IHoedirtDataComponent;
                            if (hoedirtDataComponent.TransformComponent.position.x == cell.Position.x && hoedirtDataComponent.TransformComponent.position.y == cell.Position.y)
                            {
                                hoedirtDataComponent.HoedirtDataComponent.watering = true;
                                hoedirtDataComponent.EventComponent.onEvent?.Invoke("Watering");
                                Root.State.GetHoedirt(Root.SceneLoader.TileMapManager.MapName, cell.Position.x, cell.Position.y).watering = true;
                            }
                        }
                    }
                }

                Root.SoundManager.PlaySound(SoundTrack.SFX, "watering");
            }

            inputComponent.UsingInputComponent.actionType = 0;
            inputComponent.UsingInputComponent.targetPos = Vector3.zero;
        }

        private void SeedAction(int itemId, IUsingInputComponent inputComponent)
        {
            var cropsData = Root.GameDataManager.TableData.GetCropsTableData(itemId);

            if (cropsData.Month != Root.State.month.Value)
                return;

            var pos = TargetPos(inputComponent);
            var cell = Root.SceneLoader.TileMapManager.GetCell((int)pos.x, (int)pos.y);

            if (cell == null) return;

            if (Root.State.IsHoedirt(Root.SceneLoader.TileMapManager.MapName, cell.Position.x, cell.Position.y)
                && !Root.State.IsCrops(Root.SceneLoader.TileMapManager.MapName, cell.Position.x, cell.Position.y))
            {

                var crops = new WATP.ECS.CropsEntity.CropsEntityBuilder()
                .SetPos(new Vector2(cell.Position.x, cell.Position.y))
                .SetId(itemId);

                WATP.ECS.ECSController.ServiceEvents.Emit(new WATP.ECS.EventAddEntity(crops));

                Root.State.inventory.RemoveInventory(itemId, 1);
                Root.State.AddCrops(Root.SceneLoader.TileMapManager.MapName, itemId, cell.Position.x, cell.Position.y);
            }
        }
    }
}