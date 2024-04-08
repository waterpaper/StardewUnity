using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WATP.ECS;


namespace WATP
{
    public class MapObjectManager
    {
        public IEntity player;
        public List<IEntity> entities = new List<IEntity>();

        public void Init()
        {
            Bind();
        }

        public void Dispose()
        {
            UnBind();
        }

        private void Bind()
        {
            Root.SceneLoader.onSceneChangeStart += OnSceenChangeStart;
            Root.SceneLoader.TileMapManager.onMapChangeEnd += OnMapChange;

            Root.State.day.onChange += OnDayUpdate;

            ECSController.ServiceEvents.On<EventCreateEntity>(OnObjectCreate);
            ECSController.ServiceEvents.On<EventDeleteEntity>(OnObjectRemove);
        }
        private void UnBind()
        {
            Root.SceneLoader.onSceneChangeStart -= OnSceenChangeStart;
            Root.SceneLoader.TileMapManager.onMapChangeEnd -= OnMapChange;

            Root.State.day.onChange -= OnDayUpdate;

            ECSController.ServiceEvents.Off<EventCreateEntity>(OnObjectCreate);
            ECSController.ServiceEvents.Off<EventDeleteEntity>(OnObjectRemove);

        }

        public void Clear(bool isPlayer)
        {
            foreach (var entity in entities)
            {
                entity.DeleteReservation = true;
            }

            if (player != null)
                player.DeleteReservation = isPlayer;

            ECSController.ServiceEvents.Emit(new EventDeleteRoutine());
        }

        public void OnSceenChangeStart(SceneKind sceneKind)
        {
            if (sceneKind == SceneKind.Ingame)
            {
                var farmer = new WATP.ECS.FarmerEntity.FarmerEntityBuilder()
                    .SetPos(Vector2.one * 5);

                WATP.ECS.ECSController.ServiceEvents.Emit(new WATP.ECS.EventAddEntity(farmer));
            }
            else
            {
                Clear(true);
            }
        }

        public void OnMapChange(string map)
        {
            Clear(false);
            NpcLoad(map);
            AnimalLoad(map);
            ObjectLoad(map);
            FarmLoad(map);
        }

        private void NpcLoad(string mapName)
        {
            var list = Root.GameDataManager.TableData.GetNPCPosTableDatas();

            foreach (var item in list)
            {
                if (item.Map == mapName)
                {
                    var npc = new WATP.ECS.NpcEntity.NpcEntityBuilder()
                        .SetPos(new Vector2(item.PosX, item.PosY))
                        .SetId(item.Id);

                    WATP.ECS.ECSController.ServiceEvents.Emit(new WATP.ECS.EventAddEntity(npc));
                }
            }
        }

        private void AnimalLoad(string mapName)
        {
            if (mapName == "Coop")
            {
                var list = Root.State.GetAnimals(1);
                int i = 0;
                foreach (var item in list)
                {
                    var animal = new WATP.ECS.AnimalEntity.AnimalEntityBuilder()
                        .SetPos(Vector2.right * (i + 3) + Vector2.up * (i + 3))
                        .SetId(item.id)
                        .SetDataUid(item.animalUid);

                    WATP.ECS.ECSController.ServiceEvents.Emit(new WATP.ECS.EventAddEntity(animal));
                    i++;
                }
            }
            else if (mapName == "Barn")
            {
                for (int i = 1; i < 50; i++)
                {
                    var animal = new WATP.ECS.AnimalEntity.AnimalEntityBuilder()
                        .SetPos(Vector2.right * 5 + Vector2.up * (i * 2) + Vector2.one * 0.5f)
                        .SetId(100002);

                    WATP.ECS.ECSController.ServiceEvents.Emit(new WATP.ECS.EventAddEntity(animal));
                }

                for (int i = 1; i < 50; i++)
                {
                    var animal = new WATP.ECS.AnimalEntity.AnimalEntityBuilder()
                        .SetPos(Vector2.right * 10 + Vector2.up * (i * 2) + Vector2.one * 0.5f)
                        .SetId(100002);

                    WATP.ECS.ECSController.ServiceEvents.Emit(new WATP.ECS.EventAddEntity(animal));
                }

                for (int i = 1; i < 50; i++)
                {
                    var animal = new WATP.ECS.AnimalEntity.AnimalEntityBuilder()
                        .SetPos(Vector2.right * 15 + Vector2.up * (i * 2) + Vector2.one * 0.5f)
                        .SetId(100003);

                    WATP.ECS.ECSController.ServiceEvents.Emit(new WATP.ECS.EventAddEntity(animal));
                }


                for (int i = 1; i < 50; i++)
                {
                    var animal = new WATP.ECS.AnimalEntity.AnimalEntityBuilder()
                        .SetPos(Vector2.right * 20 + Vector2.up * (i * 2) + Vector2.one * 0.5f)
                        .SetId(100003);

                    WATP.ECS.ECSController.ServiceEvents.Emit(new WATP.ECS.EventAddEntity(animal));
                }


                for (int i = 1; i < 50; i++)
                {
                    var animal = new WATP.ECS.AnimalEntity.AnimalEntityBuilder()
                        .SetPos(Vector2.right * 25 + Vector2.up * (i * 2) + Vector2.one * 0.5f)
                        .SetId(100004);

                    WATP.ECS.ECSController.ServiceEvents.Emit(new WATP.ECS.EventAddEntity(animal));
                }


                for (int i = 1; i < 50; i++)
                {
                    var animal = new WATP.ECS.AnimalEntity.AnimalEntityBuilder()
                        .SetPos(Vector2.right * 30 + Vector2.up * (i * 2) + Vector2.one * 0.5f)
                        .SetId(100004);

                    WATP.ECS.ECSController.ServiceEvents.Emit(new WATP.ECS.EventAddEntity(animal));
                }


                for (int i = 1; i < 50; i++)
                {
                    var animal = new WATP.ECS.AnimalEntity.AnimalEntityBuilder()
                        .SetPos(Vector2.right * 35 + Vector2.up * (i * 2) + Vector2.one * 0.5f)
                        .SetId(100006);

                    WATP.ECS.ECSController.ServiceEvents.Emit(new WATP.ECS.EventAddEntity(animal));
                }


                for (int i = 1; i < 50; i++)
                {
                    var animal = new WATP.ECS.AnimalEntity.AnimalEntityBuilder()
                        .SetPos(Vector2.right * 40 + Vector2.up * (i * 2) + Vector2.one * 0.5f)
                        .SetId(100006);

                    WATP.ECS.ECSController.ServiceEvents.Emit(new WATP.ECS.EventAddEntity(animal));
                }
            }
        }


        private void ObjectLoad(string mapName)
        {
            if (mapName == "Shop")
            {
                var shopObj = new WATP.ECS.ShopObjectEntity.ShopObjectEntityBuilder()
                .SetPos(new Vector2(6.5f, 13f));

                WATP.ECS.ECSController.ServiceEvents.Emit(new WATP.ECS.EventAddEntity(shopObj));
            }

            if(Root.State.objectLoadMaps.Find(data => data == mapName) == null)
            {
                var list = Root.SceneLoader.TileMapManager.SaveObjectInfos;

                foreach (var data in list)
                    Root.State.AddObject(mapName, data.objectId, data.posX, data.posY, Root.GameDataManager.TableData.GetObjectTableData(data.objectId).Hp);
            }

            var objectList = Root.State.GetObjects(mapName);
            foreach (var data in objectList)
            {
                var mapObject = new WATP.ECS.MapObjectEntity.MapObjectEntityBuilder()
                    .SetPos(new Vector2(data.posX, data.posY))
                    .SetId(data.id)
                    .SetHp(data.hp);

                WATP.ECS.ECSController.ServiceEvents.Emit(new WATP.ECS.EventAddEntity(mapObject));
            }
        }

        private void FarmLoad(string mapName)
        {
            var hoedirtList = Root.State.GetHoedirts(mapName);
            if (hoedirtList != null)
            {
                foreach (var item in hoedirtList)
                {
                    var hoedirt = new WATP.ECS.HoedirtEntity.HoedirtEntityBuilder()
                            .SetPos(new Vector2(item.posX, item.posY));

                    WATP.ECS.ECSController.ServiceEvents.Emit(new WATP.ECS.EventAddEntity(hoedirt));
                }
            }

            var cropsList = Root.State.GetCrops(mapName);
            if (cropsList != null)
            {
                foreach (var item in cropsList)
                {
                    var crops = new WATP.ECS.CropsEntity.CropsEntityBuilder()
                            .SetPos(new Vector2(item.posX, item.posY))
                            .SetId(item.id)
                            .SetDay(item.day);

                    WATP.ECS.ECSController.ServiceEvents.Emit(new WATP.ECS.EventAddEntity(crops));
                }
            }
        }

        private void OnObjectCreate(EventCreateEntity e)
        {
            if (e.Entity is IPlayerComponent && (e.Entity as IPlayerComponent).PlayerComponent.value)
            {
                player = e.Entity;
                return;
            }

            if (e.Entity is IHoedirtDataComponent)
                OnHoedirtSetting(e.Entity as IHoedirtDataComponent);

            entities.Add(e.Entity);
        }

        private void OnHoedirtSetting(IHoedirtDataComponent hoedirt)
        {
            foreach (var entity in entities)
            {
                if (entity is not IHoedirtDataComponent) continue;
                var hoeEntity = entity as IHoedirtDataComponent;

                if (entity.TransformComponent.position.x - 1 == hoedirt.TransformComponent.position.x && entity.TransformComponent.position.y == hoedirt.TransformComponent.position.y)
                {
                    hoedirt.HoedirtDataComponent.right = true;
                    hoeEntity.HoedirtDataComponent.left = true;
                    hoeEntity.EventComponent.onEvent?.Invoke(hoeEntity.HoedirtDataComponent.watering ? "Watering" : "Normal");
                }
                else if (entity.TransformComponent.position.x + 1 == hoedirt.TransformComponent.position.x && entity.TransformComponent.position.y == hoedirt.TransformComponent.position.y)
                {
                    hoedirt.HoedirtDataComponent.left = true;
                    hoeEntity.HoedirtDataComponent.right = true;
                    hoeEntity.EventComponent.onEvent?.Invoke(hoeEntity.HoedirtDataComponent.watering ? "Watering" : "Normal");
                }
                else if (entity.TransformComponent.position.y - 1 == hoedirt.TransformComponent.position.y && entity.TransformComponent.position.x == hoedirt.TransformComponent.position.x)
                {
                    hoedirt.HoedirtDataComponent.up = true;
                    hoeEntity.HoedirtDataComponent.down = true;
                    hoeEntity.EventComponent.onEvent?.Invoke(hoeEntity.HoedirtDataComponent.watering ? "Watering" : "Normal");
                }
                else if (entity.TransformComponent.position.y + 1 == hoedirt.TransformComponent.position.y && entity.TransformComponent.position.x == hoedirt.TransformComponent.position.x)
                {
                    hoedirt.HoedirtDataComponent.down = true;
                    hoeEntity.HoedirtDataComponent.up = true;
                    hoeEntity.EventComponent.onEvent?.Invoke(hoeEntity.HoedirtDataComponent.watering ? "Watering" : "Normal");
                }

            }
            hoedirt.EventComponent.onEvent?.Invoke("Normal");
        }

        private void OnDayUpdate(int day)
        {
            OnCropsUpdate();
            OnHoedirtUpdate();
        }

        private void OnHoedirtUpdate()
        {
            IHoedirtDataComponent hoedirt = null;
            foreach (var entity in entities)
            {
                if (entity is not IHoedirtDataComponent) continue;
                hoedirt = entity as IHoedirtDataComponent;

                var hoedirtData = Root.State.GetHoedirt(Root.SceneLoader.TileMapManager.MapName, hoedirt.TransformComponent.position.x, hoedirt.TransformComponent.position.y);
                if (hoedirtData != null)
                {
                    hoedirt.HoedirtDataComponent.watering = hoedirtData.watering;
                    hoedirt.EventComponent.onEvent?.Invoke("Normal");
                }
                else
                {
                    (hoedirt as IEntity).DeleteReservation = true;
                }
            }
        }

        private void OnCropsUpdate()
        {
            ICropsDataComponent crops = null;
            foreach (var entity in entities)
            {
                if (entity is not ICropsDataComponent) continue;
                crops = entity as ICropsDataComponent;

                var cropsData = Root.State.GetCrops(Root.SceneLoader.TileMapManager.MapName, crops.TransformComponent.position.x, crops.TransformComponent.position.y);
                if (cropsData != null)
                {
                    crops.CropsDataComponent.day = cropsData.day;
                    crops.EventComponent.onEvent?.Invoke("Day");
                }
                else
                {
                    (crops as IEntity).DeleteReservation = true;
                }

            }
        }

        private void OnObjectRemove(EventDeleteEntity e)
        {
            entities.Remove(e.Entity);
        }
    }
}
